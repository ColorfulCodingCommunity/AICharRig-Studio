using System;
using UnityEngine;

[Serializable]
public class RangeTrackData
{
    public string trackName;
    public AudioClip audioClip;

    public KeyframeData<float> startKeyframe;
    public KeyframeData<float> endKeyframe;

    public bool Contains(KeyframeData<float> keyframe)
    {
        return startKeyframe == keyframe || endKeyframe == keyframe;
    }

    public void SetStartKey(int currentFrame, int audioLength)
    {
        startKeyframe = new KeyframeData<float>() { frameIndex = currentFrame, value = 0 };
        endKeyframe = new KeyframeData<float>() { frameIndex = currentFrame + audioLength, value = 0 };
    }
}
