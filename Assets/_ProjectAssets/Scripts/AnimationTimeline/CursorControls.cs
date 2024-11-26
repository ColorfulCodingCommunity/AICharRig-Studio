using System;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UIElements;

public class CursorControls
{
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
    }

    private void Pause()
    {
        _timelineEditor.isPlaying = false;
    }

    private void Stop()
    {
        _timelineEditor.isPlaying = false;

        _timelineEditor.currentFrame = 0;
        _timelineEditor.SetCursor();
    }
    private void SetMaxFrame(int frame)
    {
        _timelineEditor.maxFrame = frame;
        _timelineEditor.ResetTracks();
    }

    private void SetFPS(int fps)
    {
        _timelineEditor.FPS = fps;
    }
}
