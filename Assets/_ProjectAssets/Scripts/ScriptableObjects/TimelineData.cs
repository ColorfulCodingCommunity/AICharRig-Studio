using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TimelineData
{
    [SerializeField]
    public List<FloatTrackData> floatTracks = new List<FloatTrackData>();

    public bool HasTrack(string trackName, out FloatTrackData trackInfo)
    {
        foreach (FloatTrackData track in floatTracks)
        {
            if (track.trackName == trackName)
            {
                trackInfo = track;
                return true;
            }
        }

        trackInfo = null;
        return false;
    }

    public void RemoveTrack(string trackName)
    {
        foreach (FloatTrackData track in floatTracks)
        {
            if (track.trackName == trackName)
            {
                floatTracks.Remove(track);
                return;
            }
        }
    }

    public void RemoveKey(KeyframeData<float> keyframeData)
    {
        foreach(FloatTrackData track in floatTracks)
        {
            if (track.keyframes.Remove(keyframeData))
            {
                return;
            }
        }
    }
}

