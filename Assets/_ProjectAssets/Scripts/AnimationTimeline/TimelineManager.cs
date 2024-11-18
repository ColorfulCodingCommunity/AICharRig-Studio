using System;
using System.Dynamic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TimelineManager : MonoBehaviour
{
    private TimelineEditor _timeLineEditor;
    private float _timeSinceLastFrame;
    private UIActions _uiActions;

    private void OnEnable()
    {
        _timeLineEditor = GetComponent<UIDocument>().rootVisualElement.Q<TimelineEditor>();

        _uiActions = new UIActions();
        _uiActions.Enable();
        _uiActions.Player.Delete.performed += Delete;
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
    }
    //TODO:
    public void AddTrack(FloatTrackData floatTrackData)
    {
        throw new NotImplementedException();
    }

    public void RemoveTrack(FloatTrackData floatTrackData)
    {
        throw new NotImplementedException();
    }

    public void AddKeyframe(FloatTrackData floatTrackData, KeyframeData<float> keyframe)
    {
        throw new NotImplementedException();
    }

    //public void MoveKeyframe(...)
    //{
    //    throw new NotImplementedException();
    //}

    private void Delete(InputAction.CallbackContext obj)
    {
        _timeLineEditor.DeleteKeyframe();
    }
}

