using SimpleFileBrowser;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class AssetManager : MonoSingleton<AssetManager>
{
    public event Action onSourceAssetLoaded;

    public VideoPlayer sourceAssetVideoPlayer;
    public RenderTexture sourceAssetVideoRT;

    private Texture2D sourceAsset_image;

    [HideInInspector]
    public AudioClip drivingAudio;

    public void GetSourceAssetFile(Action<Texture2D> callbackImage, Action<RenderTexture> callbackVideo)
    {
        FileBrowser.SetFilters(false, new string[] { ".jpg", ".png", ".mp4" });
        FileBrowser.ShowLoadDialog(
            (path) =>
        {
            if (path[0].EndsWith(".mp4"))
            {
                sourceAssetVideoPlayer.url = path[0];
                sourceAssetVideoPlayer.playbackSpeed = 0;
                callbackVideo(sourceAssetVideoRT);
            }
            else
            {
                sourceAsset_image = LoadImage(path[0]);
                callbackImage(sourceAsset_image);
            }

            onSourceAssetLoaded?.Invoke();
        }, null, FileBrowser.PickMode.Files, false, null, null, "Load asset", "Select");
    }

    public override void OnDestroy()
    {
        RemoveSourceAsset();
        RemoveDrivingAudio();
    }

    public void RemoveSourceAsset()
    {
        sourceAsset_image = null;

        sourceAssetVideoPlayer.url = "";
        sourceAssetVideoPlayer.Stop();

        sourceAssetVideoRT.Release();
    }

    public Texture2D GetSourceAssetImage(int frameIdx = 0)
    {
        if(sourceAsset_image != null)
        {
            return sourceAsset_image;
        }

        if (!string.IsNullOrEmpty(sourceAssetVideoPlayer.url))
        {
            return GetVideoFrame(frameIdx);
        }

        return null;
    }

    public Texture2D GetVideoFrame(int frameIdx)
    {
        sourceAssetVideoPlayer.Pause();
        sourceAssetVideoPlayer.frame = frameIdx;
        sourceAssetVideoPlayer.Play();

        Texture2D tex = new Texture2D(sourceAssetVideoRT.width, sourceAssetVideoRT.height, TextureFormat.RGBA32, false, true);
        RenderTexture.active = sourceAssetVideoRT;
        tex.ReadPixels(new Rect(0, 0, sourceAssetVideoRT.width, sourceAssetVideoRT.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        return tex;
    }

    public void GetDrivingAudioFile(Action<AudioClip, string> callback)
    {
        FileBrowser.SetFilters(false, new string[] { ".mp3", ".wav" });
        FileBrowser.ShowLoadDialog(
            (path) =>
            {
                drivingAudio = LoadAudio(path[0]);
                string filename = System.IO.Path.GetFileName(path[0]);
                callback(drivingAudio, filename);
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
