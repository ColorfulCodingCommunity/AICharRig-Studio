using SimpleFileBrowser;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RenderComponent : MonoBehaviour
{
    [SerializeField]
    private AudioLipsyncRenderer audioLipsyncRenderer;
    [SerializeField]
    private ScreenModesController screenModesController;

    private VisualElement _topBar;

    void Start()
    {
        _topBar = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Top");
        var renderBtn = _topBar.Q<Button>("RenderBtn");
        renderBtn.clicked += Render;
    }

    private void Render()
    {
        Debug.Log("Trying to render " + screenModesController.screenMode);
        if (screenModesController.screenMode == ScreenModeEnum.AudioLipsync)
        {
            FileBrowser.SetFilters(true);
            FileBrowser.ShowLoadDialog(
                (path) =>
                {
                    audioLipsyncRenderer.Render(path[0]);
                }, null, FileBrowser.PickMode.Folders, false, null, null, "Save to Folder", "Select");
            
        }
    }
}
