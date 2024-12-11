using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public enum KeyStatus
{
    NoKey = 0,
    KeyExists = 1,
    KeySelected = 2
}

[UxmlElement("KeyableSlider")]
public partial class KeyableSlider : VisualElement
{
    public event Action<float> OnValueChanged;
    public event Action OnKeyButtonClickedEvent;


    [UxmlAttribute("label")]
    public string labelText = "Test";

    [UxmlAttribute("startValue")]
    public int startValue = 50;

    //Key status - 0: No key, 1: Key exists, 2: Key selected
    private KeyStatus _keyStatus;

    private Slider _slider;
    private Label _label;
    private Label _valueLabel;
    private Button _key;
    private AnimationKey _animationKey;

    public KeyableSlider()
    {
        var visualTree =
            Resources.Load<VisualTreeAsset>("UI/UIDocs/KeyableSlider");
        visualTree.CloneTree(this);

        var styleSheet = Resources.Load<StyleSheet>("UI/USS/TimelineEditor");
        styleSheets.Add(styleSheet);

        this.AddToClassList("keyable-slider");

        _slider = this.Q<Slider>();
        _label = _slider.Query<Label>().First();

        _valueLabel = _slider.Q<Label>("ValueLabel");

        _slider.RegisterValueChangedCallback(evt => {
            _valueLabel.text = GetValue().ToString("F2");
            OnValueChanged?.Invoke(evt.newValue / 100.0f);
        });
        RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);

        _key = this.Q<Button>("Key");
        _key.clicked += OnKeyButtonClicked;
        _animationKey = _key.Q<AnimationKey>();
    }

    private void OnKeyButtonClicked()
    {
        OnKeyButtonClickedEvent?.Invoke();
    }

    private void OnAttachedToPanel(AttachToPanelEvent evt)
    {
        _label.text = labelText;
        _slider.value = startValue;
        _valueLabel.text = GetValue().ToString("F2");
    }

    public float GetValue()
    {
        return _slider.value / 100f;
    }

    public void SetValue(float value)
    {
        _slider.value = Mathf.Round(value * 100);
    }

    public void SetKeySelectedStatus()
    {
        _keyStatus = KeyStatus.KeySelected;
        _animationKey.RemoveFromClassList("empty");
        _animationKey.RemoveFromClassList("key-exists");
    }

    public void SetKeyExistsStatus()
    {
        _keyStatus = KeyStatus.KeyExists;
        _animationKey.RemoveFromClassList("empty");
        _animationKey.AddToClassList("key-exists");
    }

    public void SetKeyNoStatus()
    {
        _keyStatus = KeyStatus.NoKey;
        _animationKey.AddToClassList("empty");
        _animationKey.RemoveFromClassList("key-exists");
    }
}
