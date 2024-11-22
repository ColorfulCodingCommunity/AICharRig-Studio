using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrackData", menuName = "TrackData", order = 1)]
public class TrackData : ScriptableObject
{
    public List<FloatTrackData> floatTracks = new List<FloatTrackData>();
}

