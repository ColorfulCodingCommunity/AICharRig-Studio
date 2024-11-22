using UnityEngine;
using UnityEngine.UIElements;

public class AnimationKey : VisualElement
{
    public AnimationTrack track;
    public KeyframeData<float> keyframeData;

    public AnimationKey(KeyframeData<float> data, AnimationTrack track)
    {
        this.track = track;
        this.focusable = false;
        keyframeData = data;
    }

    public void Select()
    {
        this.AddToClassList("selectedKeyFrame");
        track.SelectKeyFrame(this);
    }
    
    public void Deselect()
    {
        this.RemoveFromClassList("selectedKeyFrame");
    }
    
    public void Delete()
    {
        track.RemoveKey(keyframeData.frameIndex);
    }
}
