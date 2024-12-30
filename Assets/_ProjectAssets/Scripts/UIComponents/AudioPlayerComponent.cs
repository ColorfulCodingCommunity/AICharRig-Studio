using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioPlayerComponent : MonoBehaviour
{
    public event Action onAudioChanged;
    public AudioSource audioSource;

    [SerializeField]
    private TimelineManager timelineManager;

    [HideInInspector]
    public bool shouldAutoUpdate = true;

    private VisualElement _audioWrapper;
    private VisualElement _hasAudioWrapper;
    private Button _uploadAudioWrapper;

    void Start()
    {
        _audioWrapper = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("DrivingAudio");
        _hasAudioWrapper = _audioWrapper.Q<VisualElement>("HasAudioWrapper");
        _uploadAudioWrapper = _audioWrapper.Q<Button>("UploadAudioBut");

        _uploadAudioWrapper.clicked += TryGetAudio;
        _hasAudioWrapper.Q<Button>("CloseBut").clicked += DeleteDrivingAudio;

        timelineManager.OnTrackDeleted += OnTrackDeleted;
        timelineManager.OnCursorMovedEvt += OnCursorMoved;

        timelineManager.timeLineEditor.cursorControls.OnPlay += PlayAudio;
        timelineManager.timeLineEditor.cursorControls.OnPause += PauseAudio;
        timelineManager.timeLineEditor.cursorControls.OnStop += StopAudio;
    }

    private void TryGetAudio()
    {
        AssetManager.Instance.GetDrivingAudioFile(async (audio, filename) =>
        {
            audioSource.clip = audio;
            _hasAudioWrapper.style.display = DisplayStyle.Flex;
            _hasAudioWrapper.Q<Label>("AudioTitle").text = filename;

            _uploadAudioWrapper.style.display = DisplayStyle.None;

            timelineManager.AddAudioRangeKey(audio, filename);

            await UniTask.WaitForSeconds(0.1f);
            timelineManager.timeLineEditor.cursorControls.SetMaxFramesEditable(false);
        });
    }

    private void OnTrackDeleted(string title)
    {
        if(title == _hasAudioWrapper.Q<Label>("AudioTitle").text)
        {
            DeleteDrivingAudio();
        }
    }

    private void OnCursorMoved(int frameIdx, TimelineData data)
    {
        if(audioSource.clip == null)
        {
            return;
        }

        float targetTime = frameIdx / 30.0f;
        audioSource.time = targetTime;
    }

    private void DeleteDrivingAudio()
    {
        timelineManager.timeLineEditor.cursorControls.SetMaxFramesEditable(true);

        AssetManager.Instance.RemoveDrivingAudio();
        audioSource.Stop();
        audioSource.clip = null;

        _hasAudioWrapper.style.display = DisplayStyle.None;
        _uploadAudioWrapper.style.display = DisplayStyle.Flex;
    }

    private void PlayAudio()
    {
        audioSource.Play();
    }

    private void PauseAudio()
    {
        audioSource.Pause();
    }

    private void StopAudio()
    {
        audioSource.Stop();
    }
}
