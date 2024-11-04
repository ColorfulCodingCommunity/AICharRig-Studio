using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("TimeLineEditor")]
public partial class TimeLineEditor : VisualElement
{
    [UxmlAttribute("currentFrame")]
    public int currentFrame;
    
    [UxmlAttribute("minFrame")]
    public int minFrame = 1;
    
    [UxmlAttribute("maxFrame")]
    public int maxFrame = 100;
    
    [UxmlAttribute("FPS")]
    public int FPS = 24;
    
    [UxmlAttribute("isPlaying")]
    public bool isPlaying;
    
    [UxmlAttribute("zoomValue")]
    public float zoomValue;
    
    private VisualElement animationKeys;
    private float animationKeyWidth = -1;
    
    public TimeLineEditor()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_ProjectAssets/UI/UIDocs/TimelineEditor 1.uxml");
        visualTree.CloneTree(this);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_ProjectAssets/UI/USS/TimeLineEditor 1.uss");
        styleSheets.Add(styleSheet);
        
        // Register callback for when the element is added to the panel
        RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
    }
    
    private void OnAttachedToPanel(AttachToPanelEvent evt)
    {
        //SetButtons
        this.Q<Button>("playButton").clickable.clicked += () => Play();
        this.Q<Button>("pauseButton").clickable.clicked += () => Pause();
        this.Q<Button>("stopButton").clickable.clicked += () => Stop();
        
        this.Q<Slider>("zoomSlider").RegisterValueChangedCallback(evt =>
        {
            SetZoom(evt.newValue);
        });
        
        //SetValues
        this.Q<IntegerField>("fpsValue").value = FPS;
        this.Q<IntegerField>("fpsValue").RegisterValueChangedCallback(evt =>
        {
            SetFPS(evt.newValue);
        });
        
        var minFrameField = this.Q<IntegerField>("minFrame");
        minFrameField.value = minFrame;
        minFrameField.RegisterValueChangedCallback(evt =>
        {
            SetMinFrame(evt.newValue);
        });
        
        this.Q<IntegerField>("maxFrame").value = maxFrame;
        this.Q<IntegerField>("maxFrame").RegisterValueChangedCallback(evt =>
        {
            SetMaxFrame(evt.newValue);
        });
        
        
        //SetAnimationKeys
        animationKeys = this.Q<VisualElement>("animationKeys");
        SetTimeAnchors();
    }

    private void SetTimeAnchors()
    {
        animationKeys.Clear();
        for (int i = minFrame; i < maxFrame/10+1; i++)
        {
            var key = new VisualElement();
            key.AddToClassList("animationKey");
            key.Add(new Label((i*10).ToString()));
            var verticalLine = new VisualElement();
            verticalLine.AddToClassList("verticalLine");
            key.Add(verticalLine);
            animationKeys.Add(key);
        }
    }
   
    public void SetCurrentFrame(int frame)
    {
        currentFrame = frame;
    }
    
    public void SetMinFrame(int frame)
    {
        Debug.Log("min frame: " + frame);
        minFrame = frame;
        SetTimeAnchors();
        
    }
    
    public void SetMaxFrame(int frame)
    {
        Debug.Log("max frame: " + frame);
        maxFrame = frame;
        SetTimeAnchors();
    }
    
    public void SetFPS(int fps)
    {
        FPS = fps;
    }
    
    public void SetIsPlaying(bool playing)
    {
        isPlaying = playing;
    }

    public void Play()
    {
        Debug.Log("play");
        SetIsPlaying(true);
    }
    
    public void Pause()
    {
        Debug.Log("pause");
        SetIsPlaying(false);
    }
    
    public void Stop()
    {
        Debug.Log("stop");
        isPlaying = false;
        currentFrame = minFrame;
    }
    
    public void SetZoom(float value)
    {
        if (animationKeyWidth == -1)
        {
            animationKeyWidth = animationKeys.resolvedStyle.width;
        }
        zoomValue = value;
        if (animationKeys != null)
        {
            Debug.Log("set zoom");
            animationKeys.style.width = new Length(animationKeyWidth * (zoomValue/200) * animationKeys.childCount, LengthUnit.Pixel);
            Debug.Log(animationKeys.style.width.value);
        }
    }
    
    
}


