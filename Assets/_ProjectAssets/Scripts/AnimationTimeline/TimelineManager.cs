using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TimelineManager : MonoBehaviour
{
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
    
    private VisualElement GetHoveredFrame()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        mousePosition.y = Screen.height - mousePosition.y;

        List<VisualElement> picked = new List<VisualElement>();
        // Get all elements at the mouse position
        GetComponent<UIDocument>().rootVisualElement.panel.PickAll(mousePosition, picked);
        foreach (var elem in picked)
        {
            if (elem.ClassListContains("frame"))
            {
                Debug.Log($"Hovered frame: {elem.name}, WorldBound: {elem.worldBound}");
                return elem;
            }
        }

        Debug.Log("No frame hovered.");
        return null;
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

