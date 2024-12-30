using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CursorControls
{
    public event Action OnPlay;
    public event Action OnPause;
    public event Action OnStop;

    private TimelineEditor _timelineEditor;

    public void Init(TimelineEditor timelineEditor)
    {
        _timelineEditor = timelineEditor;

        timelineEditor.Q<Button>("playButton").clickable.clicked += () => Play();
        timelineEditor.Q<Button>("pauseButton").clickable.clicked += () => Pause();
        timelineEditor.Q<Button>("stopButton").clickable.clicked += () => Stop();

        timelineEditor.Q<Slider>("zoomSlider").RegisterValueChangedCallback(evt => { timelineEditor.SetZoom(evt.newValue); });

        timelineEditor.Q<IntegerField>("fpsValue").RegisterValueChangedCallback(evt => { SetFPS(evt.newValue); });
        timelineEditor.Q<IntegerField>("maxFrame").RegisterValueChangedCallback(evt => { SetMaxFrame(evt.newValue); });
    
        OnAttachPanel();
    }

    public void OnAttachPanel()
    {
        _timelineEditor.Q<IntegerField>("fpsValue").value = _timelineEditor.FPS;
        _timelineEditor.Q<IntegerField>("maxFrame").value = _timelineEditor.maxFrame;
    }

    private void Play()
    {
        _timelineEditor.isPlaying = true;
        OnPlay?.Invoke();
    }

    private void Pause()
    {
        _timelineEditor.isPlaying = false;
        OnPause?.Invoke();
    }

    public void Stop()
    {
        _timelineEditor.isPlaying = false;
        OnStop?.Invoke();

        _timelineEditor.currentFrame = 0;
        _timelineEditor.SetCursor();
    }
    public void SetMaxFrame(int frame)
    {
        _timelineEditor.maxFrame = frame;
        _timelineEditor.Q<IntegerField>("maxFrame").value = frame;
        _timelineEditor.ResetTracks();
    }

    private void SetFPS(int fps)
    {
        _timelineEditor.FPS = fps;
    }

    public void SetMaxFramesEditable(bool val)
    {
        _timelineEditor.Q<IntegerField>("maxFrame").SetEnabled(val);
    }
}
