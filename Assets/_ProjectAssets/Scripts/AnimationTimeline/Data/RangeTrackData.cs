using System;

[Serializable]
public class RangeTrackData
{
    public KeyframeData<float> startKeyframe;
    public KeyframeData<float> endKeyframe;
    
    
    public bool Contains(KeyframeData<float> keyframe)
    {
        return startKeyframe == keyframe || endKeyframe == keyframe;
    }
}
