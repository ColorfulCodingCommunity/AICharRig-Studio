using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using uLipSync;
using UnityEngine;

public class CommonRenderer : MonoBehaviour
{
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

    [SerializeField]
    private LivePortraitManager livePortraitManager;
    [SerializeField]
    private WebsocketManager websocketManager;
    [SerializeField]
    private RenderingEngine renderingEngine;
    [SerializeField]
    private TimelineManager timelineManager;

    [Header("Lipsync")]
    [SerializeField]
    private AudioPlayerComponent audioPlayer;
    [SerializeField]
    private SkinnedMeshRenderer drivingFaceMeshRenderer;
    [SerializeField]
    private uLipSyncBlendShape lipSyncBlendShape;

    private const int targetFrameRate = 30;

    //Common
    private string _targetPath;

    //Lipsync recording
    private BlendShapesRecording _recording;
    private bool _isRecordingBlendshapes;
    private float _startTime;
    private float _timeSinceLastRecording;

    //Image Sequence Rendering
    private int _frameIndex;
    private string _imageSequenceTargetPath;
    private string _auxiliaryOutFolder;
    private bool _isWaitingForServerResponse;


    public void Render(string path)
    {
        audioPlayer.audioSource.Stop();
        audioPlayer.audioSource.loop = false;
        audioPlayer.shouldAutoUpdate = false;

        LoadingScreen.Instance.Show();
        LoadingScreen.Instance.SetState(0, "Simulating face expressions...");

        _targetPath = path;

        if (AssetManager.Instance.drivingAudio == null)
        {
            RenderTimelineKeyframes();
        }
        else
        {
            StartRecordingBlenshapesFromAudio();
        }
    }

    private void Update()
    {
        if (_isRecordingBlendshapes && audioPlayer.audioSource.isPlaying)
        {
            if ((Time.time - _timeSinceLastRecording) < 1f / targetFrameRate)
            {
                return;
            }

            _timeSinceLastRecording = Time.time;

            RecordBlendshapeFrameFromAudio();
            return;
        }
        else if (_isRecordingBlendshapes && !audioPlayer.audioSource.isPlaying)
        {
            _isRecordingBlendshapes = false;
            RenderTimelineKeyframes();
            return;
        }
    }

    private async void RenderTimelineKeyframes()
    {
        _frameIndex = 0;
        websocketManager.onTextureReceived += SaveTexture;

        _imageSequenceTargetPath = CreateCleanFolder("raw");
        _auxiliaryOutFolder = CreateCleanFolder("auxiliary");

        LoadingScreen.Instance.SetState(0, $"Rendering Images ({0}/{timelineManager.timeLineEditor.maxFrame})");

        await UniTask.WaitForSeconds(0.5f);
        lipSyncBlendShape.enabled = false;

        if (_recording == null)
        {
            await RenderAnimationWithNoAudio();
        }
        else
        {
            await RenderAnimationWithAudio();
        }

        renderingEngine.ImageSequenceToVideoAndAudio(_targetPath, _imageSequenceTargetPath, _auxiliaryOutFolder);
        ResetState();
    }

    private async UniTask RenderAnimationWithAudio()
    {
        foreach (var keys in _recording.blendShapesKeys)
        {
            foreach (var state in keys.blendShapes)
            {
                drivingFaceMeshRenderer.SetBlendShapeWeight(state.idx, state.weight);
            }

            await RenderImageBasedOnTimeline();
        }
    }

    private async UniTask RenderAnimationWithNoAudio()
    {
        for (int i = 0; i < timelineManager.timeLineEditor.maxFrame; i++)
        {
            await RenderImageBasedOnTimeline();
        }
    }

    private async UniTask RenderImageBasedOnTimeline()
    {
        timelineManager.timeLineEditor.SetCursorWithoutNotification(_frameIndex);

        _isWaitingForServerResponse = true;
        livePortraitManager.TrySendImageRequest(0);

        await UniTask.WaitUntil(() => !_isWaitingForServerResponse);

        LoadingScreen.Instance.SetState(_frameIndex * 1.0f / timelineManager.timeLineEditor.maxFrame,
            $"Rendering Images ({_frameIndex}/{timelineManager.timeLineEditor.maxFrame})");
    }

    private void SaveTexture(Texture texture)
    {
        var fileName = "frame_" + _frameIndex++ + ".png";

        var bytes = ((Texture2D)texture).EncodeToPNG();
        File.WriteAllBytes(Path.Combine(_imageSequenceTargetPath, fileName), bytes);

        _isWaitingForServerResponse = false;
    }

    private string CreateCleanFolder(string subfolder)
    {
        string path = Path.Combine(_targetPath, subfolder);
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        Directory.CreateDirectory(path);
        return path;
    }

    private void StartRecordingBlenshapesFromAudio()
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
    private void RecordBlendshapeFrameFromAudio()
    {
        LoadingScreen.Instance.SetState(audioPlayer.audioSource.time / audioPlayer.audioSource.clip.length, "Simulating face expressions...");

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

        _recording = null;
    }
}
