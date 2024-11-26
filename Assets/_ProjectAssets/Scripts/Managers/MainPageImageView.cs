using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainPageImageView : MonoBehaviour
{
    private VisualElement _wrapper;
    private VisualElement _uploadSourceAsset;

    private VisualElement _uploadBut;
    private Button _closeBut;

    public void Start()
    {
        _wrapper = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Center");
        _uploadSourceAsset = _wrapper.Q<VisualElement>("SourceAsset");

        _uploadBut = _uploadSourceAsset.Q<VisualElement>("UploadBut");
        _uploadBut.RegisterCallback<MouseUpEvent>(evt => AssetManager.Instance.GetSourceAssetFile(tex =>
        {
            _closeBut.style.display = DisplayStyle.Flex;
            SetSourceAsset(tex);
        }));

        _closeBut = _uploadSourceAsset.Q<Button>("CloseBut");
        _closeBut.clicked += RemoveSourceAsset;

        _closeBut.style.display = DisplayStyle.None;
    }

    public void SetSourceAsset(Texture2D tex)
    {
        _uploadSourceAsset.style.backgroundImage = new StyleBackground(tex);
        float width = _uploadSourceAsset.resolvedStyle.height * tex.width * 1.0f / tex.height;

        _uploadSourceAsset.style.width = new StyleLength(width);

        _closeBut.style.display = DisplayStyle.Flex;
        _uploadBut.style.display = DisplayStyle.None;
    }

    private void RemoveSourceAsset()
    {
        AssetManager.Instance.RemoveSourceAsset();

        _uploadSourceAsset.style.backgroundImage = new StyleBackground();
        _uploadSourceAsset.style.width = _uploadSourceAsset.resolvedStyle.height;

        _uploadBut.style.display = DisplayStyle.Flex;
        _closeBut.style.display = DisplayStyle.None;
    }
}
