using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TimelineManager : MonoBehaviour
{
    public TrackData trackData;
    
    private TimelineEditor _timeLineEditor;
    private float _timeSinceLastFrame;
    private UIActions _uiActions;
    private AnimationKey _draggedKey;
    private VisualElement _hoveredFrame;

    private void OnEnable()
    {
        _timeLineEditor = GetComponent<UIDocument>().rootVisualElement.Q<TimelineEditor>();

        _uiActions = new UIActions();
        _uiActions.Enable();
        _uiActions.Player.Delete.performed += Delete;
        if (trackData != null)
        {
            foreach (FloatTrackData track in trackData.floatTracks)
            {
                AddTrack(track);
            }
        }
        
    }

    private void Update()
    {
        if (_timeLineEditor.isPlaying)
        {
            float frameInterval = 1.0f / _timeLineEditor.FPS;

            _timeSinceLastFrame += Time.deltaTime;
            if (_timeSinceLastFrame >= frameInterval)
            {
                _timeLineEditor.NextFrame();
                _timeSinceLastFrame -= frameInterval;
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _timeLineEditor.DeselectKeyframe();
            _hoveredFrame = _timeLineEditor.hoveredElement;
            if (_hoveredFrame != null && _timeLineEditor.frameMarkersWrapper.Contains(_hoveredFrame))
            {
                _timeLineEditor.SetCurrentFrame(Int32.Parse(_hoveredFrame.name));
            }else
            if (_hoveredFrame != null && _hoveredFrame.childCount > 0)
            {
                AnimationKey key = _hoveredFrame.Query<AnimationKey>().First();
                key.Select();
                _draggedKey = key;
            }
        }else if (Mouse.current.leftButton.isPressed && _draggedKey != null && _timeLineEditor.hoveredElement !=null && _hoveredFrame != _timeLineEditor.hoveredElement)
        {
            _hoveredFrame = _timeLineEditor.hoveredElement;
            _draggedKey.track.MoveKeyFrame(_draggedKey, Int32.Parse(_hoveredFrame.name) );
        }else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _draggedKey = null;
        }
    }
    
    public void AddTrack(FloatTrackData floatTrackData)
    {
        _timeLineEditor.AddTrack(floatTrackData);
    }

    public void RemoveTrack(FloatTrackData floatTrackData)
    {
        _timeLineEditor.RemoveTrack(floatTrackData);
    }

    public void AddKeyframe(FloatTrackData floatTrackData, KeyframeData<float> keyframe)
    {
        _timeLineEditor.AddKeyframe(floatTrackData, keyframe);
    }
    
    private void Delete(InputAction.CallbackContext obj)
    {
        _timeLineEditor.DeleteKeyframe();
    }
}

