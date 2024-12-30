using SimpleFileBrowser;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RenderComponent : MonoBehaviour
{
    [SerializeField]
    private CommonRenderer commonRenderer;

    private VisualElement _topBar;

    void Start()
    {
        _topBar = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Top");
        var renderBtn = _topBar.Q<Button>("RenderBtn");
        renderBtn.clicked += Render;
    }

    private void Render()
    {

        FileBrowser.SetFilters(true);
        FileBrowser.ShowLoadDialog(
            (path) =>
            {
                commonRenderer.Render(path[0]);
            }, null, FileBrowser.PickMode.Folders, false, null, null, "Save to Folder", "Select");

        //if (screenModesController.screenMode == ScreenModeEnum.AudioLipsync)
        //{
        //    FileBrowser.SetFilters(true);
        //    FileBrowser.ShowLoadDialog(
        //        (path) =>
        //        {
        //            audioLipsyncRenderer.Render(path[0]);
        //        }, null, FileBrowser.PickMode.Folders, false, null, null, "Save to Folder", "Select");
        //}else if(screenModesController.screenMode == ScreenModeEnum.BlendShapeDriver)
        //{
        //    FileBrowser.SetFilters(true);
        //    FileBrowser.ShowLoadDialog(
        //        (path) =>
        //        {
        //            blendshapesRenderer.Render(path[0]);
        //        }, null, FileBrowser.PickMode.Folders, false, null, null, "Save to Folder", "Select");
        //}
    }
}
