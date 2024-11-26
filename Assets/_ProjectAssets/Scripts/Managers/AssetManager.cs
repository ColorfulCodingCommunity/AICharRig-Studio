using SimpleFileBrowser;
using System;
using UnityEngine;

public class AssetManager : MonoSingleton<AssetManager>
{
    public event Action onSourceAssetLoaded;

    [HideInInspector]
    public Texture2D sourceAsset;

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

    private Texture2D LoadImage(string path)
    {
        if(!CheckImage(path)) return null;

        byte[] fileData = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        return texture;
    }

    private bool CheckImage(string path)
    {
        return (path.EndsWith(".jpg") || path.EndsWith(".png"));
    }
}
