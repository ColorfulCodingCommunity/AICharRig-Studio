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
    private VisualElement animationTracks;
    private AnimationKey selectedKey = new AnimationKey(-1, null, null);
    private Label currentFrameLabel;


    private float frameRatio;
    private float leftPadding;

    #region VisualElement

    public TimeLineEditor()
    {
        var visualTree =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_ProjectAssets/UI/UIDocs/TimelineEditor 1.uxml");
        visualTree.CloneTree(this);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_ProjectAssets/UI/USS/TimeLineEditor 1.uss");
        styleSheets.Add(styleSheet);

        frameMarkers = this.Q<VisualElement>("frameMarkersWrapper");
        cursor = this.Q<VisualElement>("cursor");
        animationTracks = this.Q<VisualElement>("animationTracksWrapper");
        currentFrameLabel = this.Q<Label>("currentFrameLabel");

        // Register callback for when the element is added to the panel
        RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
    }


    private void OnAttachedToPanel(AttachToPanelEvent evt)
    {
        SetTimeMarkers();

        //SetButtons
        this.Q<Button>("playButton").clickable.clicked += () => Play();
        this.Q<Button>("pauseButton").clickable.clicked += () => Pause();
        this.Q<Button>("stopButton").clickable.clicked += () => Stop();

        zoomValue = this.Q<Slider>("zoomSlider").value;
        this.Q<Slider>("zoomSlider").RegisterValueChangedCallback(evt => { SetZoom(evt.newValue); });

        //SetValues
        this.Q<IntegerField>("fpsValue").value = FPS;
        this.Q<IntegerField>("fpsValue").RegisterValueChangedCallback(evt => { SetFPS(evt.newValue); });

        var minFrameField = this.Q<IntegerField>("minFrame");
        minFrameField.value = minFrame;
        minFrameField.RegisterValueChangedCallback(evt => { SetMinFrame(evt.newValue); });

        this.Q<IntegerField>("maxFrame").value = maxFrame;
        this.Q<IntegerField>("maxFrame").RegisterValueChangedCallback(evt => { SetMaxFrame(evt.newValue); });

        SetCursor();
        SetTestAnimatonTracks();
    }

    #endregion

    private void SetTestAnimatonTracks()
    {
        animationTracks.Clear();
        for (int i = 0; i < 10; i++)
        {
            var track = new AnimationTrack("Track " + i, this);
            animationTracks.Add(track);
            track.AddKeyFrame(Random.Range(minFrame, maxFrame));
        }

        SetAllKeyframesPosition();
    }
    
    private void SetTimeMarkers()
    {
        frameMarkers.Clear();

        for (int i = minFrame; i < maxFrame / 10 + 1; i++)
        {
            var marker = new VisualElement();
            marker.AddToClassList("frameMarker");
            marker.Add(new Label((i * 10).ToString()));
            var verticalLine = new VisualElement();
            verticalLine.AddToClassList("verticalLine");
            marker.Add(verticalLine);
            frameMarkers.Add(marker);
        }

        frameMarkers.style.width =
            new Length(animationKeyWidth * (zoomValue / 200) * frameMarkers.childCount, LengthUnit.Pixel);
    }
    
    private void SetAllKeyframesPosition()
    {
        ReadFrameMarkers();
        
        foreach (AnimationTrack track in animationTracks.Children())
        {
            foreach (AnimationKey key in track.GetKeyFrames())
            {
                key.key.style.left = new Length(
                    ((key.frame - minFrame) * frameRatio) + leftPadding / 2 -
                    key.key.style.width.value.value / 2,
                    LengthUnit.Pixel);
            }
        }
    }


    public void NextFrame()
    {
        currentFrame++;
        SetCursor();
    }

    private void SetCursor()
    {
        if (currentFrame > maxFrame)
        {
            currentFrame = minFrame;
        }


        ReadFrameMarkers();

        cursor.style.left = new Length(((currentFrame - minFrame) * frameRatio) + leftPadding / 2,
            LengthUnit.Pixel);
        currentFrameLabel.text = currentFrame.ToString();
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
            frameMarkers.style.width = new Length(animationKeyWidth * (zoomValue / 200) * frameMarkers.childCount,
                LengthUnit.Pixel);
        }

        SetCursor();
        SetAllKeyframesPosition();
    }

    #region KeyFrames

    private void DeselectKeyframe()
    {
        if (selectedKey.frame != -1)
        {
            selectedKey.Deselect();
        }
    }


    public void SelectKeyframe(AnimationKey key)
    {
        DeselectKeyframe();
        selectedKey = key;
        
    }

    public void DeleteKeyFrame()
    {
        if (selectedKey.frame != -1)
        {
            selectedKey.Delete();
            selectedKey.frame = -1;
        }
    }

    #endregion

    #region PlayerControls

    private void Play()
    {
        SetIsPlaying(true);
    }

    private void Pause()
    {
        SetIsPlaying(false);
    }

    public void Stop()
    {
        isPlaying = false;
        currentFrame = minFrame;
        SetCursor();
    }

    #endregion

    #region FrameControls

    private void SetMinFrame(int frame)
    {
        minFrame = frame;
        SetTimeMarkers();
    }

    private void SetMaxFrame(int frame)
    {
        maxFrame = frame;
        SetTimeMarkers();
    }

    private void SetFPS(int fps)
    {
        FPS = fps;
    }

    #endregion
    
    private void ReadFrameMarkers()
    {
        var firstKey = frameMarkers[0];
        var lastKey = frameMarkers[frameMarkers.childCount - 1];

        int firstFrameNumber = int.Parse(firstKey.Q<Label>().text);
        int lastFrameNumber = int.Parse(lastKey.Q<Label>().text);

        float firstKeyPos = GetGlobalLeft(firstKey[1]);
        float lastKeyPos = GetGlobalLeft(lastKey[1]);

        int numberRatio = lastFrameNumber - firstFrameNumber;
        float posRatio = lastKeyPos - firstKeyPos;
        frameRatio = posRatio / numberRatio;
        leftPadding = firstKeyPos;
    }
    
    public int GetFrameFromPosition(float position)
    {
        ReadFrameMarkers();
        return (int) ((position - leftPadding) / frameRatio) + minFrame;
    }
    
    private float GetGlobalLeft(VisualElement element)
    {
        float left = element.resolvedStyle.left;
        if (element.parent != null && element.parent != this)
        {
            left += GetGlobalLeft(element.parent);
        }

        return left;
    }
    
    private void SetIsPlaying(bool playing)
    {
        isPlaying = playing;
    }
    
}