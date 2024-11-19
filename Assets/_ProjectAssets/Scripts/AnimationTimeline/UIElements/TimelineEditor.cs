using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("TimelineEditor")]
public partial class TimelineEditor : VisualElement
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

    private VisualElement _frameMarkersWrapper;
    private float _frameMarkerWidth = -1;

    private VisualElement _cursor;
    private VisualElement _animationTracksWrapper;
    private Label _currentFrameLabel;

    private AnimationKey selectedKeyframe = new AnimationKey(-1, null, null);

    private float _frameRatio;
    private float _leftPadding;

    #region VisualElement

    public TimelineEditor()
    {
        var visualTree =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_ProjectAssets/UI/UIDocs/TimelineEditor.uxml");
        visualTree.CloneTree(this);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_ProjectAssets/UI/USS/TimelineEditor.uss");
        styleSheets.Add(styleSheet);

        _frameMarkersWrapper = this.Q<VisualElement>("frameMarkersWrapper");
        _cursor = this.Q<VisualElement>("cursor");
        _animationTracksWrapper = this.Q<VisualElement>("animationTracksWrapper");
        _currentFrameLabel = this.Q<Label>("currentFrameLabel");

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
        _animationTracksWrapper.Clear();
        for (int i = 0; i < 10; i++)
        {
            var track = new AnimationTrack("Track " + i, this);
            _animationTracksWrapper.Add(track);
            track.AddKeyFrame(Random.Range(minFrame, maxFrame));
        }

        SetAllKeyframesPosition();
    }
    
    private void SetTimeMarkers()
    {
        _frameMarkersWrapper.Clear();

        for (int i = minFrame; i < maxFrame / 10 + 1; i++)
        {
            var marker = new VisualElement();
            marker.pickingMode = PickingMode.Ignore;

            marker.AddToClassList("frameMarker");
            marker.Add(new Label((i * 10).ToString()));

            var verticalLine = new VisualElement();
            verticalLine.pickingMode = PickingMode.Ignore;
            verticalLine.AddToClassList("verticalLine");

            marker.Add(verticalLine);
            _frameMarkersWrapper.Add(marker);
        }

        _frameMarkersWrapper.style.width =
            new Length(_frameMarkerWidth * (zoomValue / 200) * _frameMarkersWrapper.childCount, LengthUnit.Pixel);
    }
    
    private void SetAllKeyframesPosition()
    {
        ReadFrameMarkers();
        
        foreach (AnimationTrack track in _animationTracksWrapper.Children())
        {
            foreach (AnimationKey key in track.GetKeyFrames())
            {
                key.key.style.left = new Length(
                    ((key.frame - minFrame) * _frameRatio) + _leftPadding / 2 -
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

        _cursor.style.left = new Length(((currentFrame - minFrame) * _frameRatio) + _leftPadding / 2,
            LengthUnit.Pixel);
        _currentFrameLabel.text = currentFrame.ToString();
    }
    
    public void SetZoom(float value)
    {
        if (_frameMarkerWidth == -1)
        {
            _frameMarkerWidth = _frameMarkersWrapper.resolvedStyle.width;
        }

        zoomValue = value;
        if (_frameMarkersWrapper != null)
        {
            _frameMarkersWrapper.style.width = new Length(_frameMarkerWidth * (zoomValue / 200) * _frameMarkersWrapper.childCount,
                LengthUnit.Pixel);
        }

        SetCursor();
        SetAllKeyframesPosition();
    }

    #region Keyframes

    private void DeselectKeyframe()
    {
        if (selectedKeyframe.frame != -1)
        {
            selectedKeyframe.Deselect();
        }
    }


    public void SelectKeyframe(AnimationKey key)
    {
        DeselectKeyframe();
        selectedKeyframe = key;
        
    }

    public void DeleteKeyframe()
    {
        if (selectedKeyframe.frame != -1)
        {
            selectedKeyframe.Delete();
            selectedKeyframe.frame = -1;
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
        var firstKey = _frameMarkersWrapper[0];
        var lastKey = _frameMarkersWrapper[_frameMarkersWrapper.childCount - 1];

        int firstFrameNumber = int.Parse(firstKey.Q<Label>().text);
        int lastFrameNumber = int.Parse(lastKey.Q<Label>().text);

        float firstKeyPos = GetGlobalLeft(firstKey[1]);
        float lastKeyPos = GetGlobalLeft(lastKey[1]);

        int numberRatio = lastFrameNumber - firstFrameNumber;
        float posRatio = lastKeyPos - firstKeyPos;
        _frameRatio = posRatio / numberRatio;
        _leftPadding = firstKeyPos;
    }
    
    public int GetFrameFromPosition(float position)
    {
        ReadFrameMarkers();
        return (int) ((position - _leftPadding) / _frameRatio) + minFrame;
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