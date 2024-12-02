using System;
using Unity.VisualScripting;
using UnityEngine;

public class LivePortraitManager : MonoSingleton<LivePortraitManager>
{
    [SerializeField]
    private TimelineManager timelineManager;
    [SerializeField]
    private WebsocketManager websocketManager;
    [SerializeField]
    private MainPageImageView mainPageImageView;
    [SerializeField]
    private SlidersManager slidersManager;

    [Space]
    [SerializeField]
    private RenderTexture drivingImage;

    private bool isFrameQueued = false;

    void Start()
    {
        timelineManager.OnCursorMovedEvt += OnCursorMoved;
        websocketManager.onTextureReceived += OnTextureReceived;
        slidersManager.OnValuesChanged += SendImageRequest;
    }

    private void Update()
    {
        //if(isFrameQueued && !websocketManager.isSendingMessage)
        //{
        //    websocketManager.SendMessage(drivingImage);
        //    isFrameQueued = false;
        //}
    }

    public void Reset()
    {
        if(!websocketManager.isConnected)
        {
            Debug.LogWarning("Websocket is not connected");
            return;
        }

        if (AssetManager.Instance.sourceAsset == null)
        {
            Debug.LogWarning("Source asset is not set");
            return;
        }

        websocketManager.Reset();
        SendImageRequest();
    }

    private void OnCursorMoved(int _, TimelineData __)
    {
        SendImageRequest();
    }

    private void SendImageRequest()
    {
        if (!websocketManager.isConnected)
        {
            Debug.LogWarning("Websocket is not connected");
            return;
        }

        if (websocketManager.isSendingMessage)
        {
            Debug.LogWarning("Websocket is already sending a message. Message queued!");
            isFrameQueued = true;
            return;
        }

        if (AssetManager.Instance.sourceAsset == null)
        {
            Debug.LogWarning("Source asset is not set");
            return;
        }

        websocketManager.SendSourceImage(AssetManager.Instance.sourceAsset);
        websocketManager.SendDrivingImage(drivingImage);
    }

    private void OnTextureReceived(Texture texture)
    {
        mainPageImageView.SetSourceAsset((Texture2D)texture);
    }
}
