using System;
using uLipSync;
using UnityEngine;
using UnityEngine.UIElements;

public class LipSyncControllers : MonoBehaviour
{
    [SerializeField]
    private uLipSync.uLipSyncBlendShape _lipSyncBlendShape;

    private VisualElement _slidersWrapper;


    void Start()
    {
        _slidersWrapper = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("DrivingVideo");

        var lipsyncSmoothnessSlider = _slidersWrapper.Q<Slider>("LipsyncSmoothnessSlider");
        var lipsyncIntensitySlider = _slidersWrapper.Q<Slider>("LipsyncIntensitySlider");

        lipsyncSmoothnessSlider.RegisterValueChangedCallback(OnSmoothnessChanged);
        lipsyncIntensitySlider.RegisterValueChangedCallback(OnIntensityChanged);

        lipsyncSmoothnessSlider.value = _lipSyncBlendShape.smoothness * 500.0f;
        lipsyncIntensitySlider.value = 100;

    }

    private void OnSmoothnessChanged(ChangeEvent<float> evt)
    {
        _lipSyncBlendShape.smoothness = evt.newValue / 500.0f;
    }

    private void OnIntensityChanged(ChangeEvent<float> evt)
    {
        foreach (var blendShape in _lipSyncBlendShape.blendShapes)
        {
            blendShape.maxWeight = evt.newValue / 100.0f;
        }
    }
}
