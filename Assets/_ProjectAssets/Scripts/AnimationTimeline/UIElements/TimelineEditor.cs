using System.Threading.Tasks;
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

    public VisualElement hoveredElement;
    public AnimationTrack frameMarkersWrapper;

    private VisualElement _cursor;
    private VisualElement _animationTracksWrapper;
    private Label _currentFrameLabel;

    private AnimationKey selectedKeyframe = new AnimationKey(-1, null);

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
        
        _cursor = this.Q<VisualElement>("cursorWrapper");
        _animationTracksWrapper = this.Q<VisualElement>("animationTracksWrapper");
        _currentFrameLabel = this.Q<Label>("currentFrameLabel");

        RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
    }


    private void OnAttachedToPanel(AttachToPanelEvent evt)
    {
        ResetTracks();

        //SetButtons
        this.Q<Button>("playButton").clickable.clicked += () => Play();
        this.Q<Button>("pauseButton").clickable.clicked += () => Pause();
        this.Q<Button>("stopButton").clickable.clicked += () => Stop();
        
        this.Q<Slider>("zoomSlider").RegisterValueChangedCallback(evt => { SetZoom(evt.newValue); });

        //SetValues
        this.Q<IntegerField>("fpsValue").value = FPS;
        this.Q<IntegerField>("fpsValue").RegisterValueChangedCallback(evt => { SetFPS(evt.newValue); });

        var minFrameField = this.Q<IntegerField>("minFrame");
        minFrameField.value = minFrame;
        minFrameField.RegisterValueChangedCallback(evt => { SetMinFrame(evt.newValue); });

        this.Q<IntegerField>("maxFrame").value = maxFrame;
        this.Q<IntegerField>("maxFrame").RegisterValueChangedCallback(evt => { SetMaxFrame(evt.newValue); });

        _animationTracksWrapper.Clear();
        CreateFrameMarkers();
        SetTestAnimationTracks();
        SetValuesAfterFrame();
    }

    private async void SetValuesAfterFrame()
    {
        await Task.Delay(500);

        SetZoom(5);
        SetCursor();
    }

    #endregion


    #region Tracks

    private void CreateFrameMarkers()
    {
        var track = new AnimationTrack("Name", this, true);
        track.name = "frameMarkersWrapper";
        _animationTracksWrapper.Add(track);
        frameMarkersWrapper = track;
    }

    private void SetTestAnimationTracks()
    {
        for (int i = 0; i < 10; i++)
        {
            var track = new AnimationTrack("Track " + i, this);
            _animationTracksWrapper.Add(track);
            track.AddKeyFrame(Random.Range(minFrame, maxFrame));
        }
    }

    private void ResetTracks()
    {
        foreach (AnimationTrack track in _animationTracksWrapper.Children())
        {
            track.ResetFrames(); 
        }
    }
    #endregion
    private void SetCursor()
    {
        if (currentFrame > maxFrame || currentFrame < minFrame)
        {
            currentFrame = minFrame;
        }
        _currentFrameLabel.text = currentFrame.ToString();
        
        _cursor.style.left = frameMarkersWrapper[currentFrame-minFrame+1].resolvedStyle.left;
        
    }

    public void SetZoom(float value)
    {
        foreach (AnimationTrack track in _animationTracksWrapper.Children())
        {
            track.SetFrameWidth(value);
        }
        _cursor.style.width = value;
        SetCursor();
    }

    #region Keyframes

    public void DeselectKeyframe()
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
        ResetTracks();
    }

    private void SetMaxFrame(int frame)
    {
        maxFrame = frame;
        ResetTracks();
    }

    private void SetFPS(int fps)
    {
        FPS = fps;
    }
    
    public void NextFrame()
    {
        currentFrame++;
        SetCursor();
    }

    public void SetCurrentFrame( int frame)
    {
        currentFrame = frame;
        SetCursor();
    }
    #endregion

    private void SetIsPlaying(bool playing)
    {
        isPlaying = playing;
    }
    
    
}