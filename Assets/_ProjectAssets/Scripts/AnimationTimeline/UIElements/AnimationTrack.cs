using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[UxmlElement("AnimationTrack")]
public partial class AnimationTrack : VisualElement
{
    public event Action<AnimationTrack> OnTrackDeleteButtonPressed;
    public event Action<AnimationKey> OnKeyClicked;
    public event Action<AnimationKey> OnKeyDragStart;
    public event Action<AnimationKey> OnKeyDragEnd;

    [HideInInspector]
    public string trackName;

    private List<AnimationKey> _keys = new List<AnimationKey>();

    private TimelineEditor _editor;
    private List<VisualElement> _frames = new List<VisualElement>();

    //private List<RangeTrackData> _rangeKeyframes = new List<RangeTrackData>();

    private bool _isMarker = false;

    public AnimationTrack(string trackName, TimelineEditor editor, bool isMarker = false)
    {
        _editor = editor;
        this.trackName = trackName;

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

        var title = new Label("Track Name");
        title.AddToClassList("marginZero");
        this.Add(title);
    }

    private void SetFrameMarkers()
    {
        for (int i = 1; i < this.childCount; i += 10)
        {
            Label marker = new Label(i - 1 + "");
            this[i].Add(marker);
            VisualElement markerLine = new VisualElement();
            markerLine.AddToClassList("markerLine");
            this[i].Add(markerLine);
        }
    }
    private void AddFramesAndTitle()
    {
        this.Clear();
        _frames.Clear();

        SetTitle(trackName);

        for (int i = 0; i < _editor.maxFrame + 1; i++)
        {
            VisualElement frame = new VisualElement();
            frame.AddToClassList("frame");
            frame.name = i + "";

            this.Add(frame);
            _frames.Add(frame);
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

        foreach (AnimationKey key in _keys)
        {
            var frame = GetFrame(key.keyframeData.frameIndex);

            if (frame != null)
            {
                frame.Add(key);
            }
        }
    }
    public void MoveKeyFrame(AnimationKey key, int targetFrameIdx)
    {
        var targetFrame = GetFrame(targetFrameIdx);
        if (targetFrame == null)
        {
            return;
        }

        var oldFrame = GetFrame(key.keyframeData.frameIndex);
        oldFrame.Remove(key);

        key.keyframeData.frameIndex = targetFrameIdx;
        targetFrame.Add(key);

        //foreach (var range in _rangeKeyframes)
        //{
        //    if (range.Contains(key.keyframeData))
        //    {
        //        SetRangeLines();
        //        return;
        //    }
        //}
    }
    public void AddKeyFrame(KeyframeData<float> floatKey)
    {
        var targetFrame = GetFrame(floatKey.frameIndex);
        if (targetFrame == null)
        {
            return;
        }

        var key = new AnimationKey(floatKey, this);

        _keys.Add(key);
        targetFrame.Add(key);

        key.OnKeyClicked += () => OnKeyClicked?.Invoke(key);
        key.OnKeyDragStart += () => OnKeyDragStart?.Invoke(key);
        key.OnKeyDragEnd += () => OnKeyDragEnd?.Invoke(key);
    }

    //private void SetRangeLines()
    //{

    //    foreach (var line in this.Query<VisualElement>().Class("rangeLine").ToList())
    //    {
    //        line.RemoveFromHierarchy();
    //    }

    //    foreach (var range in _rangeKeyframes)
    //    {
    //        for (int i = range.startKeyframe.frameIndex+1; i < range.endKeyframe.frameIndex; i++)
    //        {
    //            var line = new VisualElement();
    //            line.AddToClassList("rangeLine");
    //            _frames[i].Add(line);
    //        }
    //    }
    //}

    //public void AddRangeKeyFrame(RangeTrackData rangeKey)
    //{
    //    if (_frames.ContainsKey(rangeKey.startKeyframe.frameIndex) && _frames.ContainsKey(rangeKey.endKeyframe.frameIndex))
    //    {
    //        var startKey = new AnimationKey(rangeKey.startKeyframe, this);
    //        startKey.AddToClassList("keyFrame");
    //        _keys.Add(startKey);
    //        _frames[rangeKey.startKeyframe.frameIndex].Add(startKey);

    //        var endKey = new AnimationKey(rangeKey.endKeyframe, this);
    //        endKey.AddToClassList("keyFrame");
    //        _keys.Add(endKey);
    //        _frames[rangeKey.endKeyframe.frameIndex].Add(endKey);

    //        for (int i = rangeKey.startKeyframe.frameIndex+1; i < rangeKey.endKeyframe.frameIndex; i++)
    //        {
    //            var line = new VisualElement();
    //            line.AddToClassList("rangeLine");
    //            _frames[i].Add(line);
    //        }

    //        _rangeKeyframes.Add(rangeKey);
    //    }

    //}

    public void SetFrameWidth(float width)
    {
        foreach (VisualElement frame in this.Query<VisualElement>(className: "frame").ToList())
        {
            frame.style.width = new Length(width, LengthUnit.Pixel);
        }
    }

    private void SetTitle(string trackName)
    {
        var titleWrapper = new VisualElement();
        titleWrapper.AddToClassList("trackName");

        var title = new Label(trackName);
        titleWrapper.Add(title);

        if (trackName != "")
        {
            var xButton = new Button();
            xButton.text = "x";
            titleWrapper.Add(xButton);

            xButton.clicked += () => OnTrackDeleteButtonPressed?.Invoke(this);
        }

        this.Add(titleWrapper);
    }

    private VisualElement GetFrame(int frameIndex)
    {
        int clampedIdx = Mathf.Clamp(frameIndex, 0, _editor.maxFrame - 1);
        return _frames[clampedIdx];
    }
}
