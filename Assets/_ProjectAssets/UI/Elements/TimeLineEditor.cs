using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("TimeLineEditor")]
public partial class TimeLineEditor : VisualElement
{
    [UxmlAttribute("currentFrame")]
    public int currentFrame = 0;
    
    [UxmlAttribute("minFrame")]
    public int minFrame = 0;
    
    [UxmlAttribute("maxFrame")]
    public int maxFrame = 100;
    
    [UxmlAttribute("FPS")]
    public int FPS = 24;
    
    [UxmlAttribute("isPlaying")]
    public bool isPlaying;
    
    [UxmlAttribute("zoomValue")]
    public float zoomValue = 20;
    
    private VisualElement frameMarkers;
    private VisualElement cursor;
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
        
        //SetAnimationKeys
        frameMarkers = this.Q<VisualElement>("frameMarkersWrapper");
        SetTimeAnchors();
        
        //SetButtons
        this.Q<Button>("playButton").clickable.clicked += () => Play();
        this.Q<Button>("pauseButton").clickable.clicked += () => Pause();
        this.Q<Button>("stopButton").clickable.clicked += () => Stop();
        
        zoomValue = this.Q<Slider>("zoomSlider").value;
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
        
        cursor = this.Q<VisualElement>("cursor");
        
        SetCursor();
        
       
    }

    private void SetTimeAnchors()
    {
        frameMarkers.Clear();

        for (int i = minFrame; i < maxFrame/10+1; i++)
        {
            var key = new VisualElement();
            key.AddToClassList("frameMarker");
            key.Add(new Label((i*10).ToString()));
            var verticalLine = new VisualElement();
            verticalLine.AddToClassList("verticalLine");
            key.Add(verticalLine);
            frameMarkers.Add(key);
        }
        frameMarkers.style.width = new Length(animationKeyWidth * (zoomValue/200) * frameMarkers.childCount, LengthUnit.Pixel);
    }

    public void SetCurrentFrame(int frame)
    {
        currentFrame = frame;
    }
    
    public void SetMinFrame(int frame)
    {
        minFrame = frame;
        SetTimeAnchors();
    }
    
    public void SetMaxFrame(int frame)
    {
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
        SetCursor();
    }
    
    private void SetCursor()
    {
        if (currentFrame > maxFrame)
        {
            currentFrame = minFrame;
        }
        
        if (frameMarkers.childCount >= 2)
        {
            var firstKey = frameMarkers[0];
            var lastKey = frameMarkers[frameMarkers.childCount-1];
            
            int firstFrameNumber = int.Parse(firstKey.Q<Label>().text);
            int lastFrameNumber = int.Parse(lastKey.Q<Label>().text);
            
            float firstKeyPos = GetGlobalLeft(firstKey[1]);
            float lastKeyPos = GetGlobalLeft(lastKey[1]);
            
            int numberRatio = lastFrameNumber - firstFrameNumber;
            float posRatio = lastKeyPos - firstKeyPos;
            float frameRatio = posRatio / numberRatio;

            cursor.style.left = new Length(((currentFrame-firstFrameNumber) * frameRatio)+firstKeyPos/2, LengthUnit.Pixel);
        }
        
    }
    
    private float GetGlobalLeft(VisualElement element)
    {
        float left = element.resolvedStyle.left;
        if (element.parent != null && element.parent!= this)
        {
            left += GetGlobalLeft(element.parent);
        }
        return left;
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
            animationKeyWidth = frameMarkers.resolvedStyle.width;
        }
        zoomValue = value;
        if (frameMarkers != null)
        {
            frameMarkers.style.width = new Length(animationKeyWidth * (zoomValue/200) * frameMarkers.childCount, LengthUnit.Pixel);
        }
        SetCursor();
    }
    
    
}


