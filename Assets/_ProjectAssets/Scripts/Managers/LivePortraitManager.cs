using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LivePortraitManager : MonoSingleton<LivePortraitManager>
{
    [SerializeField]
    private TimelineManager timelineManager;
    [SerializeField]
    private WebsocketManager websocketManager;
    [SerializeField]
    private MainPageImageView mainPageImageView;

    [Space]
    [SerializeField]
    private SlidersManager slidersManager;
    [SerializeField]
    private AudioPlayerComponent audioPlayerComponent;

    [Space]
    [SerializeField]
    private RenderTexture drivingImage;

    public RawImage target;

    private bool isFrameQueued = false;

    void Start()
    {
        timelineManager.OnCursorMovedEvt += OnCursorMoved;
        websocketManager.onTextureReceived += OnTextureReceived;
        slidersManager.OnValuesChanged += SendImageRequest;
        audioPlayerComponent.onAudioChanged += SendImageRequest;
    }

    public override void OnDestroy()
    {
        timelineManager.OnCursorMovedEvt -= OnCursorMoved;
        websocketManager.onTextureReceived -= OnTextureReceived;
        slidersManager.OnValuesChanged -= SendImageRequest;
        audioPlayerComponent.onAudioChanged -= SendImageRequest;

        base.OnDestroy();
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

        if (AssetManager.Instance.GetSourceAssetImage() == null)
        {
            Debug.LogWarning("Source asset is not set");
            return;
        }

        websocketManager.Reset();
        SendImageRequest();
    }

    private void OnCursorMoved(int frameIdx, TimelineData __)
    {
        SendImageRequest(frameIdx);
    }

    private void SendImageRequest(int frameIdx)
    {
        TrySendImageRequest(frameIdx);
    }

    private void SendImageRequest()
    {
        TrySendImageRequest(0);
    }

    public bool TrySendImageRequest(int frameIdx)
    {
        if (!websocketManager.isConnected)
        {
            Debug.LogWarning("Websocket is not connected");
            return false;
        }

        if (websocketManager.isSendingMessage)
        {
            Debug.LogWarning("Websocket is already sending a message. Message queued!");
            isFrameQueued = true;
            return true;
        }

        Texture2D sourceImage = AssetManager.Instance.GetSourceAssetImage(frameIdx);
        if (sourceImage == null)
        {
            Debug.LogWarning("Source asset is not set");
            return false;
        }

        target.texture = sourceImage;
        websocketManager.SendSourceImage(sourceImage);
        websocketManager.SendDrivingImage(drivingImage);
        return true;
    }

    private void OnTextureReceived(Texture texture)
    {
        mainPageImageView.SetSourceAsset((Texture2D)texture);
    }
}
