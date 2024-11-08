using System;
using System.Dynamic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TimeLineManager : MonoBehaviour
{
    private TimeLineEditor _timeLineEditor;
    private float _timeSinceLastFrame;
    private UIInput _uiInput;

    private void OnEnable()
    {
        _timeLineEditor = GetComponent<UIDocument>().rootVisualElement.Q<TimeLineEditor>();
        _uiInput = new UIInput();
        _uiInput.Enable();
        _uiInput.Player.Delete.performed += Delete;
    }

    private void Delete(InputAction.CallbackContext obj)
    {
        _timeLineEditor.DeleteKeyFrame();
    }
    
   
    private void Update()
    {
        if (_timeLineEditor.isPlaying)
        {
            // Calculate the interval between frames based on frames per second
            float frameInterval = 1.0f / _timeLineEditor.FPS;
            
            // Accumulate time
            _timeSinceLastFrame += Time.deltaTime;

            // Check if enough time has passed to advance to the next frame
            if (_timeSinceLastFrame >= frameInterval)
            {
                _timeLineEditor.NextFrame();
                
                // Reset the timer, accounting for any extra time passed over the interval
                _timeSinceLastFrame -= frameInterval;
            }
        }
    }
}

