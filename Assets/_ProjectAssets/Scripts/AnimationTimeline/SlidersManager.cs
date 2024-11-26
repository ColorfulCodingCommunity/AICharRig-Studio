using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SlidersManager : MonoBehaviour
{
    [SerializeField]
    private TimelineManager timelineManager;

    [SerializeField]
    private DrivingFaceControls drivingFaceControls;

    private VisualElement _slidersWrapper;
    private List<KeyableSlider> _sliders;

    private Button _resetSlidersBtn;

    void Start()
    {
        _slidersWrapper = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("ControlsWrapper");

        _sliders = _slidersWrapper.Query<KeyableSlider>().ToList();
        foreach (var slider in _sliders)
        {
            slider.OnKeyButtonClickedEvent += () => OnKeyedSliderPressed(slider);
        }

        _resetSlidersBtn = _slidersWrapper.Q<Button>("ResetSlidersBtn");
        _resetSlidersBtn.clicked += () => drivingFaceControls.Reset();

        timelineManager.OnTrackDeleted += OnTrackDeleted;
        timelineManager.OnCursorMovedEvt += OnCursorMoved;

        SetupSliderEvents();
    }

    private void SetupSliderEvents()
    {
        _slidersWrapper.Q<KeyableSlider>("LeftBrowSlider").OnValueChanged
            += (val) => drivingFaceControls.SetLeftBrow(val);
        _slidersWrapper.Q<KeyableSlider>("RightBrowSlider").OnValueChanged
            += (val) => drivingFaceControls.SetRightBrow(val);

        _slidersWrapper.Q<KeyableSlider>("SmileSadLeftSlider").OnValueChanged
            += (val) => drivingFaceControls.SetSmileLeft(val);
        _slidersWrapper.Q<KeyableSlider>("SmileSadRightSlider").OnValueChanged
            += (val) => drivingFaceControls.SetSmileRight(val);

        _slidersWrapper.Q<KeyableSlider>("MouthOpenSlider").OnValueChanged
            += (val) => drivingFaceControls.SetMouthOpen(val);
        _slidersWrapper.Q<KeyableSlider>("MouthPuffSlider").OnValueChanged
            += (val) => drivingFaceControls.SetMouthPuff(val);

        _slidersWrapper.Q<KeyableSlider>("EyesDirectionSlider").OnValueChanged
            += (val) => drivingFaceControls.SetEyesDirection(val);
        _slidersWrapper.Q<KeyableSlider>("EyesBlinkSlider").OnValueChanged
            += (val) => drivingFaceControls.SetBlink(val);

        _slidersWrapper.Q<KeyableSlider>("HeadYawSlider").OnValueChanged
            += (val) => drivingFaceControls.SetHeadYaw(val);
        _slidersWrapper.Q<KeyableSlider>("HeadPitchSlider").OnValueChanged
            += (val) => drivingFaceControls.SetHeadPitch(val);
        _slidersWrapper.Q<KeyableSlider>("HeadRollSlider").OnValueChanged
            += (val) => drivingFaceControls.SetHeadRoll(val);
    }

    public async UniTask<List<KeyableSlider>> GetSliders()
    {
        while (_sliders == null)
        {
            await UniTask.NextFrame();
        }

        return _sliders;
    }

    private void OnCursorMoved(int frame, TimelineData data)
    {
        foreach (var track in data.floatTracks)
        {
            float value = track.GetValue(frame, out bool isExactFrame);
            SetSliderValue(track.trackName, value, isExactFrame);
        }

    }

    private void SetSliderValue(string trackName, float value, bool isExactFrame)
    {
        foreach (var slider in _sliders)
        {
            if (slider.labelText == trackName)
            {
                slider.SetValue(value);

                if (isExactFrame)
                {
                    slider.SetKeySelectedStatus();
                }
                else
                {
                    slider.SetKeyExistsStatus();
                }

                return;
            }
        }
    }

    private void OnKeyedSliderPressed(KeyableSlider slider)
    {
        timelineManager.AddKey(slider.labelText, slider.GetValue());
        slider.SetKeySelectedStatus();
    }

    private void OnTrackDeleted(string trackName)
    {
        foreach (var slider in _sliders)
        {
            if (slider.labelText == trackName)
            {
                slider.SetKeyNoStatus();
                return;
            }
        }
    }
}
