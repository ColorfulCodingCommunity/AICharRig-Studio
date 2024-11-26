using NativeWebSocket;
using System;
using UnityEngine;

public class WebsocketManager : MonoSingleton<WebsocketManager>
{
    public event Action onConnect;
    public event Action onDisconnect;

    public event Action<Texture> onTextureReceived;

    [SerializeField]
    private string url = "ws://127.0.0.1:9870";

    [HideInInspector]
    public bool isConnected;
    [HideInInspector]
    public bool isSendingMessage;

    private WebSocket _websocket;

    public async void Start()
    {
        _websocket = new WebSocket(url);
        _websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            isConnected = true;

            onConnect?.Invoke();

            Reset();
        };

        _websocket.OnError += (e) =>
        {
            isConnected = false;
            onDisconnect?.Invoke();
            Debug.Log("Error! " + e);
        };

        _websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            isConnected = false;

            onDisconnect?.Invoke();
        };

        _websocket.OnMessage += (bytes) =>
        {
            onTextureReceived?.Invoke(BytesToTexture(bytes));
            isSendingMessage = false;
        };
        await _websocket.Connect();
    }

    public void Update()
    {
        if (!isConnected)
        {
            return;
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        _websocket.DispatchMessageQueue();
#endif

        //if (shouldSendStream && !_waitMessage)
        //{
        //    _waitMessage = true;
        //    SendMessage();
        //}
    }

    public void TryConnect()
    {
        if (isConnected) { return; }
        _websocket.Connect();
    }

    public void Reset()
    {
        _websocket.SendText("RESET");
    }

    public void SendSourceImage(Texture2D sourceAsset)
    {
        Debug.Log("Sending Source Image...");
        var bytes = sourceAsset.EncodeToJPG();

        //Append 0 to the beginning of the byte array to indicate that this is a source image
        byte[] newBytes = new byte[bytes.Length + 1];
        newBytes[0] = 0;
        Array.Copy(bytes, 0, newBytes, 1, bytes.Length);

        _websocket.Send(newBytes);
    }

    public void SendDrivingImage(RenderTexture drivingImage)
    {
        Debug.Log("Sending Driving Image...");
        isSendingMessage = true;

        var bytes = TextureToBytes(drivingImage);

        //Append 1 to the beginning of the byte array to indicate that this is a driving image
        byte[] newBytes = new byte[bytes.Length + 1];
        newBytes[0] = 1;
        Array.Copy(bytes, 0, newBytes, 1, bytes.Length);

        _websocket.Send(newBytes);
    }

    private byte[] TextureToBytes(RenderTexture drivingTex)
    {
        Texture2D tex = new Texture2D(drivingTex.width, drivingTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = drivingTex;
        tex.ReadPixels(new Rect(0, 0, drivingTex.width, drivingTex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToJPG();

        return bytes;
    }

    private Texture BytesToTexture(byte[] stream)
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
        if (texture.LoadImage(stream))
        {
            return texture;
        }

        Debug.LogError("Failed to load texture from Base64 string.");
        return null;
    }

}
