using SimpleFileBrowser;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Networking;

public class AssetManager : MonoSingleton<AssetManager>
{
    public event Action onSourceAssetLoaded;

    [HideInInspector]
    public Texture2D sourceAsset;

    [HideInInspector]
    public AudioClip drivingAudio;

    public void GetSourceAssetFile(Action<Texture2D> callback)
    {
        FileBrowser.SetFilters(true, new string[] { ".jpg", ".png", ".mp4" });
        FileBrowser.ShowLoadDialog(
            (path) =>
        {
            sourceAsset = LoadImage(path[0]);
            callback(sourceAsset);
            onSourceAssetLoaded?.Invoke();
        }, null, FileBrowser.PickMode.Files, false, null, null, "Load asset", "Select");
    }

    public void RemoveSourceAsset()
    {
        sourceAsset = null;
    }

    public void GetDrivingAudioFile(Action<AudioClip> callback)
    {
        FileBrowser.SetFilters(true, new string[] { ".mp3", ".wav" });
        FileBrowser.ShowLoadDialog(
            (path) =>
            {
                drivingAudio = LoadAudio(path[0]);
                callback(drivingAudio);
            }, null, FileBrowser.PickMode.Files, false, null, null, "Load Audio", "Select");


    }

    public void RemoveDrivingAudio()
    {
        drivingAudio = null;
    }

    private Texture2D LoadImage(string path)
    {
        if (!CheckImage(path)) return null;

        byte[] fileData = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        return texture;
    }

    private bool CheckImage(string path)
    {
        return (path.EndsWith(".jpg") || path.EndsWith(".png"));
    }

    private AudioClip LoadAudio(string path)
    {
        AudioType audioType;
        if(path.EndsWith(".mp3"))
        {
            audioType = AudioType.MPEG;
        }
        else if (path.EndsWith(".wav"))
        {
            audioType = AudioType.WAV;
        }
        else
        {
            return null;
        }

        using (var uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType))
        {
            uwr.SendWebRequest();
            while (!uwr.isDone) { }
            return DownloadHandlerAudioClip.GetContent(uwr);
        }
    }
}
