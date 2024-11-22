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
    private Dictionary<int, VisualElement> frames = new Dictionary<int, VisualElement>();
    private bool _isMarker = false;

    public AnimationTrack(string trackName, TimelineEditor editor, bool isMarker = false)
    {
        this.trackName = trackName;
        _editor = editor;
        this.AddToClassList("animationTrack");
        this.AddToClassList("smallText");
        
        AddFramesAndTitle();

        if (isMarker)
        {
            _isMarker = true;
            SetFrameMarkers();
        }
    }

    public AnimationTrack()
    {
        this.AddToClassList("animationTrack");
        this.AddToClassList("smallText");
        var title = new Label(trackName);
        title.AddToClassList("marginZero");
        this.Add(title);
    }

    private void SetFrameMarkers()
    {
        for (int i = 1; i < this.childCount; i+=10)
        {
            Label marker = new Label(i-1+_editor.minFrame + "");
            this[i].Add(marker);
            VisualElement markerLine = new VisualElement();
            markerLine.AddToClassList("markerLine");
            this[i].Add(markerLine);
        }
    }

    private void AddFramesAndTitle()
    {
        this.Clear();
        frames.Clear();
        var title = new Label(trackName);
        title.AddToClassList("trackName");
        this.Add(title);
        for(int i = _editor.minFrame; i < _editor.maxFrame+1; i++)
        {
            VisualElement frame = new VisualElement();
            frame.AddToClassList("frame");
            frame.RegisterCallback<MouseEnterEvent>(evt => { _editor.hoveredElement = frame; });
            frame.RegisterCallback<MouseLeaveEvent>(evt => { if (_editor.hoveredElement == frame )_editor.hoveredElement = null; });
            frame.name = i+"";
            this.Add(frame);
            frames.Add(i, frame);
        }

    }
    
    public void ResetFrames()
    {
        if (_editor == null)
        {
            return;
        }
        AddFramesAndTitle();
        if (_isMarker)
        {
            SetFrameMarkers();
            return;
        }
        foreach (AnimationKey key in keys)
        {
            if (frames.ContainsKey(key.frame))
            {
                frames[key.frame].Add(key);
            }
        }
    }

    public void MoveKeyFrame(AnimationKey key, int newFrame)
    {
        if (frames.ContainsKey(newFrame))
        {
            frames[key.frame].Remove(key);
            key.frame = newFrame;
            frames[newFrame].Add(key);
        }
    }
    public void AddKeyFrame(int frame)
    {
        if (frames.ContainsKey(frame))
        {
            var key = new AnimationKey(frame, this);
            key.AddToClassList("keyFrame");
            keys.Add(key);
            frames[frame].Add(key);
        }
        
    }
    
    public void SetFrameWidth(float width)
    {
        foreach (VisualElement frame in this.Query<VisualElement>(className: "frame").ToList())
        {
            frame.style.width = new Length(width, LengthUnit.Pixel);
        }
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
                keys[i].RemoveFromHierarchy();
                keys.RemoveAt(i);
                break;
            }
        }
    }

}
