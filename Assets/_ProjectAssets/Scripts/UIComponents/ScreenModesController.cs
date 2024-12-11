using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenModesController : MonoBehaviour
{
    public static event Action<ScreenModeEnum> ScreenModeChanged;

    [HideInInspector]
    public ScreenModeEnum screenMode { get; private set; }

    private VisualElement _root;
    private EnumField _screenModesEnumField;

    private List<VisualElement> blendshapeDriverElements = new List<VisualElement>();
    private List<VisualElement> audioLipsyncElements = new List<VisualElement>();
    private List<VisualElement> videoDrivenElements = new List<VisualElement>();

    async void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _screenModesEnumField = _root.Q<EnumField>("ScreenModesEnumField");
        _screenModesEnumField.RegisterCallback<ChangeEvent<string>>(OnScreenModeChanged);


        blendshapeDriverElements.Add(_root.Q<VisualElement>("TimelineEditor"));
        blendshapeDriverElements.Add(_root.Q<VisualElement>("Sliders"));

        audioLipsyncElements.Add(_root.Q<VisualElement>("AudioPlayer"));

        videoDrivenElements.Add(_root.Q<VisualElement>("TODO"));

        await UniTask.WaitForSeconds(0.1f);
        _screenModesEnumField.value = ScreenModeEnum.BlendShapeDriver;
        //OnScreenModeChanged(ScreenModeEnum.BlendShapeDriver);
    }

    private void OnScreenModeChanged(ChangeEvent<string> evt)
    {
        var noSpacesValue = evt.newValue.Replace(" ", "");
        ScreenModeEnum screenMode = (ScreenModeEnum)Enum.Parse(typeof(ScreenModeEnum), noSpacesValue);

        OnScreenModeChanged(screenMode);
    }

    private void OnScreenModeChanged(ScreenModeEnum screenMode)
    {
        this.screenMode = screenMode;
        
        ScreenModeChanged?.Invoke(screenMode);

        foreach (VisualElement blendshapeEl in blendshapeDriverElements)
        {
            blendshapeEl.style.display = screenMode == ScreenModeEnum.BlendShapeDriver ? DisplayStyle.Flex : DisplayStyle.None;
        }

        foreach (VisualElement audiolipsyncEl in audioLipsyncElements)
        {
            audiolipsyncEl.style.display = screenMode == ScreenModeEnum.AudioLipsync ? DisplayStyle.Flex : DisplayStyle.None;
        }

        foreach (VisualElement videoDrivenEl in videoDrivenElements)
        {
            videoDrivenEl.style.display = screenMode == ScreenModeEnum.VideoDriver ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
