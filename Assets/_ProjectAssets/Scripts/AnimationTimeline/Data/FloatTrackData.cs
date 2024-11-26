using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FloatTrackData
{
    [SerializeField]
    public string trackName;
    [SerializeField]
    public List<KeyframeData<float>> keyframes = new List<KeyframeData<float>>();

    public KeyframeData<float> AddKey(int frameIdx, float value, out bool wasOverriden)
    {
        for(int i=0; i< keyframes.Count; i++)
        {
            //Overwrite the value if the keyframe already exists
            if (keyframes[i].frameIndex == frameIdx)
            {
                keyframes[i].value = value;
                wasOverriden = true;
                return keyframes[i];
            }

            //Insert the keyframe at the right position
            if (keyframes[i].frameIndex > frameIdx)
            {
                var keyframe = new KeyframeData<float>() { frameIndex = frameIdx, value = value };
                keyframes.Insert(i, keyframe);
                wasOverriden = false;
                return keyframe;
            }
        }

        //Add at the end
        var newKeyframe = new KeyframeData<float>() { frameIndex = frameIdx, value = value };
        keyframes.Add(newKeyframe);
        wasOverriden = false;
        return newKeyframe;
    }

    public float GetValue(int frame, out bool isExactFrame)
    {
        isExactFrame = false;
        for (int i = 0; i < keyframes.Count; i++)
        {
            //Exact frame found
            if (keyframes[i].frameIndex == frame)
            {
                isExactFrame = true;
                return keyframes[i].value;
            }
            else if (keyframes[i].frameIndex > frame)
            {
                //Before first keyframe
                if (i == 0)
                {
                    return keyframes[0].value;
                }

                //Between two keyframes
                float lerpedVal = Mathf.Lerp(keyframes[i - 1].value, keyframes[i].value, 
                    (frame - keyframes[i - 1].frameIndex) * 1.0f / (keyframes[i].frameIndex - keyframes[i - 1].frameIndex));
                return lerpedVal;
            }
        }

        //After last keyframe
        return keyframes[keyframes.Count - 1].value;
    }
}
