using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenModesController : MonoBehaviour
{
    public static event Action<ScreenModeEnum> ScreenModeChanged;
    private VisualElement _root;
    private EnumField _screenModesEnumField;

    private List<VisualElement> blendshapeDriverElements = new List<VisualElement>();
    private List<VisualElement> audioLipsyncElements = new List<VisualElement>();

    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _screenModesEnumField = _root.Q<EnumField>("ScreenModesEnumField");
        _screenModesEnumField.RegisterCallback<ChangeEvent<string>>(OnScreenModeChanged);

        blendshapeDriverElements.Add(_root.Q<VisualElement>("TimelineEditor"));
        blendshapeDriverElements.Add(_root.Q<VisualElement>("Sliders"));

        audioLipsyncElements.Add(_root.Q<VisualElement>("AudioPlayer"));
    }

    private void OnScreenModeChanged(ChangeEvent<string> evt)
    {
        var noSpacesValue = evt.newValue.Replace(" ", "");
        ScreenModeEnum screenMode = (ScreenModeEnum)Enum.Parse(typeof(ScreenModeEnum), noSpacesValue);

        ScreenModeChanged?.Invoke(screenMode);

        foreach (VisualElement blendshapeEl in blendshapeDriverElements)
        {
            blendshapeEl.style.display = screenMode == ScreenModeEnum.BlendShapeDriver ? DisplayStyle.Flex : DisplayStyle.None;
        }

        foreach(VisualElement audiolipsyncEl in audioLipsyncElements)
        {
            audiolipsyncEl.style.display = screenMode == ScreenModeEnum.AudioLipsync ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
