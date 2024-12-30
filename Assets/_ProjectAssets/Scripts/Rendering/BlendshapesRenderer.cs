//using System;
//using System.IO;
//using UnityEngine;

//public class BlendshapesRenderer : MonoBehaviour
//{
//    public TimelineManager timelineManager;
//    public WebsocketManager websocketManager;
//    public RenderingEngine renderingEngine;

//    private const int targetFrameRate = 30;

//    private bool _isRecording;
//    private int _currentFrame = 0;
//    private float _timeSinceLastRecording;
//    private bool _didServerRespond;
//    private string _targetPath;
//    private string _imageSequenceTargetPath;

//    private void Update()
//    {
//        if (_isRecording)
//        {
//            if (!_didServerRespond)
//            {
//                return;
//            }

//            if ((Time.time - _timeSinceLastRecording) < 1f / targetFrameRate)
//            {
//                return;
//            }
//            _timeSinceLastRecording = Time.time;

//            _didServerRespond = false;
//            websocketManager.onTextureReceived += WaitForServerResponse;

//            timelineManager.timeLineEditor.SetCursorToNextFrame();

//            _currentFrame = timelineManager.timeLineEditor.currentFrame;
//            int maxFrames = timelineManager.timeLineEditor.maxFrame;
//            LoadingScreen.Instance.SetState(_currentFrame * 1.0f / maxFrames, $"Rendering frames ({_currentFrame}/{maxFrames})");

//            if (_currentFrame >= maxFrames)
//            {
//                StopRecording();
//            }
//        }
//    }

//    private void WaitForServerResponse(Texture texture)
//    {
//        var fileName = "frame_" + _currentFrame + ".png";

//        var bytes = ((Texture2D)texture).EncodeToPNG();
//        File.WriteAllBytes(Path.Combine(_imageSequenceTargetPath, fileName), bytes);

//        _didServerRespond = true;
//        websocketManager.onTextureReceived -= WaitForServerResponse;
//    }

//    [ContextMenu("Render")]
//    public void TEST_Render()
//    {
//        Render("");
//    }

//    public void Render(string targetPath)
//    {
//        timelineManager.timeLineEditor.cursorControls.Stop();
//        StartRecording(targetPath);
//    }

//    private void StartRecording(string targetPath)
//    {
//        _targetPath = targetPath;
//        _isRecording = true;
//        _currentFrame = 0;
//        _timeSinceLastRecording = 0f;
//        _didServerRespond = true;


//        LoadingScreen.Instance.Show();

//        _imageSequenceTargetPath = Path.Combine(_targetPath, "raw");
//        if (Directory.Exists(_imageSequenceTargetPath))
//        {
//            Directory.Delete(_imageSequenceTargetPath, true);
//        }

//        Directory.CreateDirectory(_imageSequenceTargetPath);
//    }

//    private void StopRecording()
//    {
//        websocketManager.onTextureReceived -= WaitForServerResponse;
//        _isRecording = false;

//        LoadingScreen.Instance.SetState(1.0f, $"Merging frames into video...");
//        renderingEngine.ImageSequenceToVideo(_imageSequenceTargetPath, _targetPath);


//        if (Directory.Exists(_imageSequenceTargetPath))
//        {
//            Directory.Delete(_imageSequenceTargetPath, true);
//        }
//        LoadingScreen.Instance.Hide();
//    }
//}
