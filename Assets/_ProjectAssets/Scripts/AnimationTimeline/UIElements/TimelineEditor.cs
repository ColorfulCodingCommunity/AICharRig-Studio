using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("TimelineEditor")]
public partial class TimelineEditor : VisualElement
{
    public event Action<AnimationTrack> OnTrackTryDelete;
    public event Action<int> OnCursorMoved;

    [UxmlAttribute("currentFrame")]
    public int currentFrame = 0;

    [UxmlAttribute("maxFrame")]
    public int maxFrame = 100;

    [UxmlAttribute("FPS")]
    public int FPS = 24;

    public bool isPlaying;

    public AnimationTrack frameMarkersWrapper;

    [HideInInspector]
    public CursorControls cursorControls;

    private VisualElement _cursor;
    private VisualElement _animationTracksWrapper;
    private Label _currentFrameLabel;
    private AnimationKey selectedKeyframe = null;

    private float _currentZoom = 5f;
    private float _frameRatio;
    private float _leftPadding;

    private bool _isDraggingKey;
    private bool _isDraggingCursor;

    #region VisualElement

    public TimelineEditor()
    {
        var visualTree =
            Resources.Load<VisualTreeAsset>("UI/UIDocs/TimelineEditor");
        visualTree.CloneTree(this);

        var styleSheet = Resources.Load<StyleSheet>("UI/USS/TimelineEditor");
        styleSheets.Add(styleSheet);
        
        _cursor = this.Q<VisualElement>("cursorWrapper");
        _animationTracksWrapper = this.Q<VisualElement>("animationTracksWrapper");
        _currentFrameLabel = this.Q<Label>("currentFrameLabel");

        RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);

        cursorControls = new CursorControls();
        cursorControls.Init(this);
    }

    private void OnAttachedToPanel(AttachToPanelEvent evt)
    {
        cursorControls.OnAttachPanel();

        ResetTracks();

        RegisterCallback<MouseOverEvent>(OnMouseOver, TrickleDown.TrickleDown);

        _animationTracksWrapper.Clear();

        CreateFrameMarkers();
        SetDefaultValuesAsync();
    }

    private void OnMouseOver(MouseOverEvent evt)
    {
        //Drag keyframe logic
        if (_isDraggingKey && selectedKeyframe != null)
        {
            var mousePosX = evt.localMousePosition.x;

            VisualElement frame = GetFrame(mousePosX, out int idx);
            selectedKeyframe.track.MoveKeyFrame(selectedKeyframe, idx - 2);
        }
    }

    public void OnMouseReleased()
    {
        _isDraggingKey = false;
        _isDraggingCursor = false;
    }

    private async void SetDefaultValuesAsync()
    {
        await Task.Delay(500);

        SetZoom(_currentZoom);
        SetCursor();
    }
    #endregion

    #region PUBLIC_METHODS

    public async void AddTrack(FloatTrackData trackData)
    {
        var track = new AnimationTrack(trackData.trackName, this);
        _animationTracksWrapper.Add(track);

        track.OnTrackDeleteButtonPressed += (track) => OnTrackTryDelete?.Invoke(track);
        track.OnKeyClicked += SelectKeyframe;
        track.OnKeyDragStart += StartKeyDrag;

        ResetTracks();

        await UniTask.WaitForEndOfFrame();
        SetZoom(_currentZoom);
    }

    public void AddKeyToTrack(string trackName, KeyframeData<float> key)
    {
        foreach (AnimationTrack track in _animationTracksWrapper.Children())
        {
            if (track.trackName == trackName)
            {
                track.AddKeyFrame(key);
                break;
            }
        }
    }

    public void SetCursorToNextFrame()
    {
        currentFrame++;
        Debug.Log($"FRAME {currentFrame}");
        SetCursor();
    }

    public void SetCursor()
    {
        if (currentFrame > maxFrame)
        {
            currentFrame = 0;
        }

        _currentFrameLabel.text = currentFrame.ToString();
        _cursor.style.left = frameMarkersWrapper[currentFrame + 1].resolvedStyle.left;

        OnCursorMoved?.Invoke(currentFrame);
    }

    public void SetZoom(float value)
    {
        currentFrame = 0;

        _currentZoom = value;
        foreach (AnimationTrack track in _animationTracksWrapper.Children())
        {
            track.SetFrameWidth(value);
        }
        _cursor.style.width = value;
        SetCursor();
    }
    public void ResetTracks()
    {
        foreach (AnimationTrack track in _animationTracksWrapper.Children())
        {
            track.ResetFrames();
        }
    }

    #endregion

    #region TRACKS_PRIVATE_METHODS

    private void CreateFrameMarkers()
    {
        frameMarkersWrapper = new AnimationTrack("", this, true);
        frameMarkersWrapper.name = "frameMarkersWrapper";
        _animationTracksWrapper.Add(frameMarkersWrapper);

        frameMarkersWrapper.RegisterCallback<MouseDownEvent>(StartDraggingCursor);
        frameMarkersWrapper.RegisterCallback<MouseOverEvent>(OnCursorDrag);
    }

    private void StartDraggingCursor(MouseDownEvent evt)
    {
        isPlaying = false;
        _isDraggingCursor = true;

        SetCursorToPosition(evt.localMousePosition.x);
    }

    private void OnCursorDrag(MouseOverEvent evt)
    {
        if(!_isDraggingCursor) return;

        SetCursorToPosition(evt.localMousePosition.x);
    }

    private void SetCursorToPosition(float x)
    {
        GetFrame(x, out int idx);

        currentFrame = Mathf.Max(0, idx - 1);
        SetCursor();
    }

    #endregion

    #region KEYFRAMES

    public void SelectKeyframe(AnimationKey key)
    {
        DeselectCurrentKeyframe();
        selectedKeyframe = key;
        selectedKeyframe.Select();
    }

    public void DeselectCurrentKeyframe()
    {
        if (selectedKeyframe != null)
        {
            selectedKeyframe.Deselect();
        }
    }

    public AnimationKey DeleteKeyframe()
    {
        if (selectedKeyframe != null)
        {
            selectedKeyframe.Delete();

            var removedKey = selectedKeyframe;
            selectedKeyframe = null;
            return removedKey;
        }

        return null;
    }

    private void StartKeyDrag(AnimationKey key)
    {
        _isDraggingKey = true;
    }

    //public void AddKeyframe(FloatTrackData floatTrackData, KeyframeData<float> keyframeData)
    //{
    //    foreach (AnimationTrack track in _animationTracksWrapper.Children())
    //    {
    //        if (track.IsSameTrack(floatTrackData))
    //        {
    //            track.AddKeyFrame(keyframeData);
    //            floatTrackData.keyframes.Add(keyframeData);
    //            break;
    //        }
    //    }
    //}
    #endregion

    #region UTILS
    private VisualElement GetFrame(float localX, out int idx)
    {
        idx = 0;
        foreach (var frameMarker in frameMarkersWrapper.Children())
        {
            if (frameMarker.localBound.x < localX && frameMarker.localBound.x + frameMarker.localBound.width > localX)
            {
                return frameMarker;
            }
            idx++;
        }

        idx = 0;
        return frameMarkersWrapper[0];
    }

    #endregion

}