using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SlidersManager : MonoBehaviour
{
    public event Action OnValuesChanged;

    [SerializeField]
    private TimelineManager timelineManager;

    [SerializeField]
    private DrivingFaceControls drivingFaceControls;

    private VisualElement _slidersWrapper;
    private List<KeyableSlider> _sliders;

    private Button _resetSlidersBtn;

    private bool _valuesChanged = false;

    void Start()
    {
        _slidersWrapper = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("ControlsWrapper");

        _sliders = _slidersWrapper.Query<KeyableSlider>().ToList();
        foreach (var slider in _sliders)
        {
            slider.OnKeyButtonClickedEvent += () => OnKeyedSliderPressed(slider);
        }

        _resetSlidersBtn = _slidersWrapper.Q<VisualElement>("Sliders").Q<Button>("ResetSlidersBtn");
        _resetSlidersBtn.clicked += () => drivingFaceControls.Reset();

        timelineManager.OnTrackDeleted += OnTrackDeleted;
        timelineManager.OnCursorMovedEvt += OnCursorMoved;

        SetupSliderEvents();
    }

    private void Update()
    {
        if (_valuesChanged)
        {
            OnValuesChanged?.Invoke();
            _valuesChanged = false;
        }
    }

    private void SetupSliderEvents()
    {
        _slidersWrapper.Q<KeyableSlider>("LeftBrowSlider").OnValueChanged
            += (val) =>
            {
                _valuesChanged = true;
                drivingFaceControls.SetLeftBrow(val);
            };

        _slidersWrapper.Q<KeyableSlider>("RightBrowSlider").OnValueChanged
            += (val) =>
            {
                _valuesChanged = true;
                drivingFaceControls.SetRightBrow(val);
            };

        _slidersWrapper.Q<KeyableSlider>("SmileSadLeftSlider").OnValueChanged
            += (val) =>
            {
                _valuesChanged = true;
                drivingFaceControls.SetSmileLeft(val);
            };
        _slidersWrapper.Q<KeyableSlider>("SmileSadRightSlider").OnValueChanged
            += (val) =>
            {
                _valuesChanged = true;
                drivingFaceControls.SetSmileRight(val);
            };

        _slidersWrapper.Q<KeyableSlider>("MouthOpenSlider").OnValueChanged
            += (val) =>
            {
                _valuesChanged = true;
                drivingFaceControls.SetMouthOpen(val);
            };

        _slidersWrapper.Q<KeyableSlider>("MouthPuffSlider").OnValueChanged
            += (val) =>
            {
                _valuesChanged = true;
                drivingFaceControls.SetMouthPuff(val);
            };

        _slidersWrapper.Q<KeyableSlider>("EyesDirectionSlider").OnValueChanged
            += (val) =>
            {
                _valuesChanged = true;
                drivingFaceControls.SetEyesDirection(val);
            };
        _slidersWrapper.Q<KeyableSlider>("EyesBlinkSlider").OnValueChanged
            += (val) =>
            {
                _valuesChanged = true;
                drivingFaceControls.SetBlink(val);
            };

        _slidersWrapper.Q<KeyableSlider>("HeadYawSlider").OnValueChanged
            += (val) =>
            {
                _valuesChanged = true;
                drivingFaceControls.SetHeadYaw(val);
            };

        _slidersWrapper.Q<KeyableSlider>("HeadPitchSlider").OnValueChanged
            += (val) =>
            {
                _valuesChanged = true;
                drivingFaceControls.SetHeadPitch(val);
            };
        _slidersWrapper.Q<KeyableSlider>("HeadRollSlider").OnValueChanged
            += (val) =>
            {
                _valuesChanged = true;
                drivingFaceControls.SetHeadRoll(val);
            };
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
