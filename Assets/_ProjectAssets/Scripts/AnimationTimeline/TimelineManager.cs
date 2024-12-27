using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TimelineManager : MonoBehaviour
{
    public event Action<int, TimelineData> OnCursorMovedEvt;
    public event Action<string> OnTrackDeleted;

    public InputAction deleteAction;

    [HideInInspector]
    public TimelineEditor timeLineEditor;

    private TimelineData _timelineData;

    private float _timeSinceLastFrame;

    private void OnEnable()
    {
        timeLineEditor = GetComponent<UIDocument>().rootVisualElement.Q<TimelineEditor>();

        deleteAction.Enable();
        deleteAction.performed += DeleteKey;

        timeLineEditor.OnCursorMoved += OnCursorMoved;
        timeLineEditor.OnTrackTryDelete += TryDeleteTrack;

        _timelineData = new TimelineData();
        //if (trackData != null)
        //{
        //    foreach (FloatTrackData track in trackData.floatTracks)
        //    {
        //        _timeLineEditor.AddTrack(track);
        //    }
        //}
    }

    private void OnCursorMoved(int idx)
    {
        OnCursorMovedEvt?.Invoke(idx, _timelineData);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            timeLineEditor.OnMouseReleased();
        }
        //Cursor play logic
        if (timeLineEditor.isPlaying)
        {
            float frameInterval = 1.0f / timeLineEditor.FPS;

            _timeSinceLastFrame += Time.deltaTime;
            if (_timeSinceLastFrame >= frameInterval)
            {
                timeLineEditor.SetCursorToNextFrame();
                _timeSinceLastFrame -= frameInterval;
            }
        }
    }

    public void AddKey(string trackName, float value)
    {
        FloatTrackData trackInfo;

        if (!_timelineData.HasTrack(trackName, out trackInfo))
        {
            trackInfo = new FloatTrackData()
            {
                trackName = trackName
            };

            _timelineData.floatTracks.Add(trackInfo);
            timeLineEditor.AddTrack(trackInfo);
        }

        var key = trackInfo.AddKey(timeLineEditor.currentFrame, value, out bool wasOverriden);

        if (!wasOverriden)
        {
            timeLineEditor.AddKeyToTrack(trackName, key);
        }
    }

    //public void RemoveTrack(FloatTrackData floatTrackData)
    //{
    //    _timeLineEditor.RemoveTrack(floatTrackData);
    //}

    //public void AddKeyframe(FloatTrackData floatTrackData, KeyframeData<float> keyframe)
    //{
    //    _timeLineEditor.AddKeyframe(floatTrackData, keyframe);
    //}

    private void DeleteKey(InputAction.CallbackContext obj)
    {
        var key = timeLineEditor.DeleteKeyframe();

        if (key != null)
        {
            _timelineData.RemoveKey(key.keyframeData);
        }
    }

    private void TryDeleteTrack(AnimationTrack track)
    {
        track.RemoveFromHierarchy();
        _timelineData.RemoveTrack(track.trackName);

        OnTrackDeleted?.Invoke(track.trackName);
    }
}

