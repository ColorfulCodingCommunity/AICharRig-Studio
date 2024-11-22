using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[UxmlElement("AnimationTrack")]
public partial class AnimationTrack : VisualElement
{
    private List<AnimationKey> keys = new List<AnimationKey>();
    private TimelineEditor _editor;
    private FloatTrackData _trackData;
    private Dictionary<int, VisualElement> frames = new Dictionary<int, VisualElement>();
    private List<RangeTrackData> rangeKeyframes = new List<RangeTrackData>();
    private bool _isMarker = false;
    
    public AnimationTrack(FloatTrackData trackData, TimelineEditor editor, bool isMarker = false)
    {
        _trackData = trackData;
        _editor = editor;
        this.AddToClassList("animationTrack");
        this.AddToClassList("smallText");
        
        AddFramesAndTitle();

        if (isMarker)
        {
            _isMarker = true;
            SetFrameMarkers();
        }
        else
        {
            foreach (var keyframe in trackData.keyframes)
            {
                AddKeyFrame(keyframe);
            }

            foreach (var range in trackData.rangeKeyframes)
            {
                AddRangeKeyFrame(range);
            }
        }
    }

    public AnimationTrack()
    {
        this.AddToClassList("animationTrack");
        this.AddToClassList("smallText");
        var title = new Label("Track Name");
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
        var title = new Label(_trackData.trackName);
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
            if (frames.ContainsKey(key.keyframeData.frameIndex))
            {
                frames[key.keyframeData.frameIndex].Add(key);
            }
        }
    }

    public void MoveKeyFrame(AnimationKey key, int newFrame)
    {
        if (frames.ContainsKey(newFrame) && key != null)
        {
            frames[key.keyframeData.frameIndex].Remove(key);
            key.keyframeData.frameIndex = newFrame;
            frames[newFrame].Add(key);
            foreach (var range in rangeKeyframes)
            {
                if (range.Contains(key.keyframeData))
                {
                    SetRangeLines();
                    return;
                }
            }
        }
    }
    public void AddKeyFrame(KeyframeData<float> floatKey)
    {
        if (frames.ContainsKey(floatKey.frameIndex))
        {
            var key = new AnimationKey(floatKey, this);
            key.AddToClassList("keyFrame");
            keys.Add(key);
            frames[floatKey.frameIndex].Add(key);
            _trackData.keyframes.Add(floatKey);
        }
        
    }

    private void SetRangeLines()
    {

        foreach (var line in this.Query<VisualElement>().Class("rangeLine").ToList())
        {
            line.RemoveFromHierarchy();
        }
        
        foreach (var range in rangeKeyframes)
        {
            for (int i = range.startKeyframe.frameIndex+1; i < range.endKeyframe.frameIndex; i++)
            {
                var line = new VisualElement();
                line.AddToClassList("rangeLine");
                frames[i].Add(line);
            }
        }
    }
    public void AddRangeKeyFrame(RangeTrackData rangeKey)
    {
        if (frames.ContainsKey(rangeKey.startKeyframe.frameIndex) && frames.ContainsKey(rangeKey.endKeyframe.frameIndex))
        {
            var startKey = new AnimationKey(rangeKey.startKeyframe, this);
            startKey.AddToClassList("keyFrame");
            keys.Add(startKey);
            frames[rangeKey.startKeyframe.frameIndex].Add(startKey);
            
            var endKey = new AnimationKey(rangeKey.endKeyframe, this);
            endKey.AddToClassList("keyFrame");
            keys.Add(endKey);
            frames[rangeKey.endKeyframe.frameIndex].Add(endKey);

            for (int i = rangeKey.startKeyframe.frameIndex+1; i < rangeKey.endKeyframe.frameIndex; i++)
            {
                var line = new VisualElement();
                line.AddToClassList("rangeLine");
                frames[i].Add(line);
            }
            
            rangeKeyframes.Add(rangeKey);
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
        _editor.SelectKeyframe(key);
    }

    public void RemoveKey(int frame)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i].keyframeData.frameIndex == frame)
            {
                keys[i].RemoveFromHierarchy();
                _trackData.keyframes.Remove(keys[i].keyframeData);
                keys.RemoveAt(i);
                break;
            }
        }
    }

    
    public bool IsSameTrack(FloatTrackData trackData)
    {
        return _trackData == trackData;
    }
}
