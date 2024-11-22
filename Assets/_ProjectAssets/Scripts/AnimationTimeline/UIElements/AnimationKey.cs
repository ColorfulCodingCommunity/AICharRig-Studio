using UnityEngine;
using UnityEngine.UIElements;

public class AnimationKey : VisualElement
{
    public int frame;
    public AnimationTrack track;

    public AnimationKey(int frame, AnimationTrack track)
    {
        if (frame <0)
        {
            return;
        }
        this.frame = frame;
        this.track = track;
        this.focusable = false;
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
        track.RemoveKey(frame);
    }
}
