using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct animationKey
{
    public int frame;
    public VisualElement key;
    public AnimationTrack track;
    
    public void Select()
    {
        Debug.Log(frame);
        key.AddToClassList("selectedKeyFrame");
    }
    
    public void Deselect()
    {
        key.RemoveFromClassList("selectedKeyFrame");
    }
    
    public void Delete()
    {
        track.RemoveKey(frame);
    }
}

[UxmlElement("AnimationTrack")]
public partial class AnimationTrack : VisualElement
{
    [UxmlAttribute("trackName")]
    public string trackName = "debugName";
    public List<animationKey> keys = new List<animationKey>();
    private TimeLineEditor _editor;
    
    public AnimationTrack(string trackName, TimeLineEditor editor)
    {
        this.trackName = trackName;
        this.AddToClassList("animationTrack");
        this.AddToClassList("smallText");
        var title = new Label(trackName);
        title.AddToClassList("marginZero");
        this.Add(title);
        _editor = editor;
    }
    
    public AnimationTrack()
    {
        this.AddToClassList("animationTrack");
        this.AddToClassList("smallText");
        var title = new Label(trackName);
        title.AddToClassList("marginZero");
        this.Add(title);
    }
    
    public void AddKeyFrame(int frame)
    {
        var key = new VisualElement();
        key.AddToClassList("keyFrame");
        key.pickingMode = PickingMode.Position;
        keys.Add(new animationKey { frame = frame, key = key , track = this});
        this.Add(key);
        key.RegisterCallback<ClickEvent>(evt =>
        {
            SelectFrame(frame);
        });
    }
    
    private void SelectFrame(int frame)
    {
        foreach (var key in keys)
        {
            if (key.frame == frame)
            {
                _editor.SelectKeyframe(key);
                return;
            }
        }
    }
    
    public void DeselectAllFrames()
    {
        foreach (var key in keys)
        {
            key.Deselect();
        }
    }
    
    public void RemoveKey(int frame)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i].frame == frame)
            {
                keys[i].key.RemoveFromHierarchy();
                keys.RemoveAt(i);
                break;
            }
        }
    }
    
    public void ClearKeys()
    {
        foreach (var key in keys)
        {
            key.key.RemoveFromHierarchy();
        }
        keys.Clear();
    }
    
    public List<animationKey> GetKeyFrames()
    {
        return keys;
    }
    
    
}
