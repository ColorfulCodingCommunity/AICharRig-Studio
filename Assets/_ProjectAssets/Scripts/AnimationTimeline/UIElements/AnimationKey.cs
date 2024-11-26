using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("AnimationKey")]
public partial class AnimationKey : VisualElement
{
    public event Action OnKeyClicked;
    public event Action OnKeyDragStart;
    public event Action OnKeyDragEnd;

    public AnimationTrack track;
    public KeyframeData<float> keyframeData;

    public AnimationKey(KeyframeData<float> data, AnimationTrack track)
    {
        this.track = track;
        keyframeData = data;

        Init();
    }

    public AnimationKey()
    {
        Init();
    }

    private void Init()
    {
        AddToClassList("keyFrame");
        this.focusable = false;

        this.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
        this.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    private void OnMouseUp(MouseUpEvent evt)
    {
        OnKeyDragEnd?.Invoke();
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        OnKeyDragStart?.Invoke();
        OnKeyClicked?.Invoke();
    }

    public void Select()
    {
        this.AddToClassList("selectedKeyFrame");
    }
    
    public void Deselect()
    {
        this.RemoveFromClassList("selectedKeyFrame");
    }
    
    public void Delete()
    {
        RemoveFromHierarchy();
    }
}
