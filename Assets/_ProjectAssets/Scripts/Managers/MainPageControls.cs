using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainPageControls : MonoBehaviour
{
    private VisualElement _wrapper;
    private Button _connectBtn;
    private Button _sendBasePoseBtn;

    void Start()
    {
        _wrapper = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("BackEndControls");

        _connectBtn = _wrapper.Q<Button>("ConnectBtn");
        _connectBtn.clicked += () =>
        {
            WebsocketManager.Instance.TryConnect();
        };

        _sendBasePoseBtn = _wrapper.Q<Button>("SendBasePoseBtn");
        _sendBasePoseBtn.SetEnabled(false);

        _sendBasePoseBtn.clicked += () =>
        {
            LivePortraitManager.Instance.Reset();
        };

        WebsocketManager.Instance.onConnect += HandleConnect;
        WebsocketManager.Instance.onDisconnect += HandleConnect;

        AssetManager.Instance.onSourceAssetLoaded += () =>
        {
            _sendBasePoseBtn.SetEnabled(true);
        };

        HandleConnect();
    }

    private void HandleConnect()
    {
        if(WebsocketManager.Instance.isConnected)
        {
            _connectBtn.Q<Label>().text = "Connected";
            _connectBtn.Q<VisualElement>("Icon").style.backgroundColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
            _connectBtn.SetEnabled(false);
        }
        else
        {
            _connectBtn.Q<Label>().text = "Press to Connect to AI Server";
            _connectBtn.Q<VisualElement>("Icon").style.backgroundColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            _connectBtn.SetEnabled(true);
        }
    }
}
