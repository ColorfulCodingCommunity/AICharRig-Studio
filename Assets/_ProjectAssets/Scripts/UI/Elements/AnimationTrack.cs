using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[UxmlElement("AnimationTrack")]
public partial class AnimationTrack : VisualElement
{
    [UxmlAttribute("trackName")]
    public string trackName = "debugName";
    public List<AnimationKey> keys = new List<AnimationKey>();
    public TimelineEditor _editor;
    
    public AnimationTrack(string trackName, TimelineEditor editor)
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
    
    public int GetFrameFromPosition(float position)
    {
        return _editor.GetFrameFromPosition(position);
    }
    
    public void AddKeyFrame(int frame)
    {
        var key = new VisualElement();
        key.AddToClassList("keyFrame");
        key.pickingMode = PickingMode.Position;
        keys.Add(new AnimationKey(frame,key,this));
        this.Add(key);
    }
    
    public void SelectKeyFrame(AnimationKey key)
    {
        if (keys.Contains(key))
        {
            _editor.SelectKeyframe(key);
        }
        else
        {
            Debug.LogError("Key not found in track");
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
    
    
    public List<AnimationKey> GetKeyFrames()
    {
        return keys;
    }
    
    
}
