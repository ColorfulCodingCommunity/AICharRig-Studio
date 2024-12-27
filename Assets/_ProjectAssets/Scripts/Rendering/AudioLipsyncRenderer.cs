using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using uLipSync;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioLipsyncRenderer : MonoBehaviour
{
    public LivePortraitManager livePortraitManager;
    public AudioPlayerComponent audioPlayer;

    public WebsocketManager websocketManager;
    public RenderingEngine renderingEngine;

    [Space]
    public SkinnedMeshRenderer drivingFaceMeshRenderer;
    public uLipSyncBlendShape lipSyncBlendShape;

    private const int targetFrameRate = 30;

    private float _startTime;
    private BlendShapesRecording _recording;
    private bool _isRecordingBlendshapes = false;

    private float _timeSinceLastRecording = 0f;
    private bool _isWaitingForServerResponse = false;
    private int _frameIndex;
    private string _targetPath;
    private string _imageSequenceTargetPath;
    private string _auxiliaryOutFolder;

    private class BlendShapeState
    {
        public int idx;
        public float weight;
    }
    private class BlendShapesKey
    {
        public float timestamp;
        public List<BlendShapeState> blendShapes;

        public BlendShapesKey()
        {
            blendShapes = new List<BlendShapeState>();
        }
    }
    private class BlendShapesRecording
    {
        public List<BlendShapesKey> blendShapesKeys;

        public BlendShapesRecording()
        {
            blendShapesKeys = new List<BlendShapesKey>();
        }
    }

    [ContextMenu("Render")]
    public void Render(string targetPath)
    {
        audioPlayer.audioSource.Stop();
        audioPlayer.audioSource.loop = false;
        audioPlayer.shouldAutoUpdate = false;

        LoadingScreen.Instance.Show();
        LoadingScreen.Instance.SetState(0, "Simulating face expressions...");

        _targetPath = targetPath;
        PlayFrameByFrame();
    }

    private void Update()
    {
        //Handle Blendshapes recording
        if (_isRecordingBlendshapes && audioPlayer.audioSource.isPlaying)
        {
            if ((Time.time - _timeSinceLastRecording) < 1f / targetFrameRate)
            {
                return;
            }

            LoadingScreen.Instance.SetState(audioPlayer.audioSource.time / audioPlayer.audioSource.clip.length, "Simulating face expressions...");

            _timeSinceLastRecording = Time.time;
            float timestamp = Time.time - _startTime;

            BlendShapesKey key = new BlendShapesKey()
            {
                timestamp = timestamp
            };

            for (int i = 17; i <= 22; i++)
            {
                BlendShapeState state = new BlendShapeState()
                {
                    idx = i,
                    weight = drivingFaceMeshRenderer.GetBlendShapeWeight(i)
                };

                key.blendShapes.Add(state);
            }

            _recording.blendShapesKeys.Add(key);
            return;
        }
        if (_isRecordingBlendshapes && !audioPlayer.audioSource.isPlaying)
        {
            _isRecordingBlendshapes = false;

            RecordBlendshapesToFace();
            return;
        }
    }

    [ContextMenu("RecordBlendshapesToFace")]
    public async void RecordBlendshapesToFace()
    {
        _frameIndex = 0;
        websocketManager.onTextureReceived += SaveTexture;

        _imageSequenceTargetPath = Path.Combine(_targetPath, "raw");
        if (Directory.Exists(_imageSequenceTargetPath))
        {
            Directory.Delete(_imageSequenceTargetPath, true);
        }
        Directory.CreateDirectory(_imageSequenceTargetPath);


        _auxiliaryOutFolder = Path.Combine(_targetPath, "output");
        if (Directory.Exists(_auxiliaryOutFolder))
        {
            Directory.Delete(_auxiliaryOutFolder, true);
        }
        Directory.CreateDirectory(_auxiliaryOutFolder);


        LoadingScreen.Instance.SetState(0, $"Rendering Images ({_frameIndex}/{_recording.blendShapesKeys.Count})");
        
        await UniTask.WaitForSeconds(0.5f);
        lipSyncBlendShape.enabled = false;

        foreach (var key in _recording.blendShapesKeys)
        {
            foreach (var state in key.blendShapes)
            {
                drivingFaceMeshRenderer.SetBlendShapeWeight(state.idx, state.weight);
            }

            _isWaitingForServerResponse = true;
            livePortraitManager.TrySendImageRequest(0);

            await UniTask.WaitUntil(() => !_isWaitingForServerResponse);


            LoadingScreen.Instance.SetState(_frameIndex * 1.0f / _recording.blendShapesKeys.Count,
                $"Rendering Images ({_frameIndex}/{_recording.blendShapesKeys.Count})");
        }

        renderingEngine.ImageSequenceToVideoAndAudio(_targetPath, _imageSequenceTargetPath, _auxiliaryOutFolder);
        ResetState();
    }

    private void PlayFrameByFrame()
    {
        if (audioPlayer.audioSource.clip == null)
        {
            ResetState();
            return;
        }

        _startTime = Time.time;
        _recording = new BlendShapesRecording();
        _isRecordingBlendshapes = true;

        audioPlayer.audioSource.Play();
    }

    private void ResetState()
    {

        websocketManager.onTextureReceived -= SaveTexture;

        if (Directory.Exists(_imageSequenceTargetPath))
        {
            Directory.Delete(_imageSequenceTargetPath, true);
        }

        if (Directory.Exists(_auxiliaryOutFolder))
        {
            Directory.Delete(_auxiliaryOutFolder, true);
        }

        LoadingScreen.Instance.Hide();
        audioPlayer.audioSource.loop = true;
        audioPlayer.shouldAutoUpdate = true;

        lipSyncBlendShape.enabled = true;

    }

    private void SaveTexture(Texture texture)
    {
        var fileName = "frame_" + _frameIndex++ + ".png";

        var bytes = ((Texture2D)texture).EncodeToPNG();
        File.WriteAllBytes(Path.Combine(_imageSequenceTargetPath, fileName), bytes);

        _isWaitingForServerResponse = false;
    }
}
