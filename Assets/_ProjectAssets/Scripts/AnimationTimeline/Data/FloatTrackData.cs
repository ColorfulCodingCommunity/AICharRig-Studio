using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FloatTrackData
{
    public string trackName;
    public List<KeyframeData<float>> keyframes;
}
