using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioPlayerComponent : MonoBehaviour
{
    public AudioSource audioSource;

    private bool _hasAudio = false;
    private VisualElement _audioWrapper;
    private VisualElement _hasAudioWrapper;
    private Button _uploadAudioWrapper;
    private Label _timeCode;
    private VisualElement _timeline;
    private VisualElement _knob;

    void Start()
    {
        _audioWrapper = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("AudioPlayer");
        _hasAudioWrapper = _audioWrapper.Q<VisualElement>("HasAudioWrapper");
        _uploadAudioWrapper = _audioWrapper.Q<Button>("UploadAudioWrapper");

        _uploadAudioWrapper.clicked += TryGetAudio;
        _hasAudioWrapper.Q<Button>("CloseBut").clicked += DeleteDrivingAudio;

        var controls = _audioWrapper.Q<VisualElement>("Controls");

        controls.Q<Button>("PlayButton").clicked += PlayAudio;
        controls.Q<Button>("PauseButton").clicked += PauseAudio;
        controls.Q<Button>("StopButton").clicked += StopAudio;

        _timeline = _audioWrapper.Q<VisualElement>("Timeline");
        _timeCode = _timeline.Q<Label>("TimeCode");
        _knob = _timeline.Q<VisualElement>("Knob");

        _timeline.RegisterCallback<ClickEvent>(SetTimeOnCLick);
    }

    private void TryGetAudio()
    {
        AssetManager.Instance.GetDrivingAudioFile((audio) =>
        {
            audioSource.clip = audio;
            _hasAudioWrapper.style.display = DisplayStyle.Flex;
            _uploadAudioWrapper.style.display = DisplayStyle.None;

            SetAudio(audio);
        });
    }

    private void DeleteDrivingAudio()
    {
        AssetManager.Instance.RemoveDrivingAudio();
        audioSource.Stop();
        audioSource.clip = null;

        _hasAudioWrapper.style.display = DisplayStyle.None;
        _uploadAudioWrapper.style.display = DisplayStyle.Flex;
    }

    private void SetTimeOnCLick(ClickEvent clickEvent)
    {
        var clickPosition = clickEvent.localPosition.x;
        var audioTime = (clickPosition / _timeline.resolvedStyle.width) * audioSource.clip.length;
        Debug.Log("Time: " + audioTime);
        audioSource.time = audioTime;
    }

    private void SetAudio(AudioClip audio)
    {
        _timeCode.text = "00:00/" + TimeSpan.FromSeconds(audio.length).ToString(@"mm\:ss");
    }

    private void PlayAudio()
    {
        audioSource.Play();
        StartCoroutine(UpdateAudioTime());
    }

    private void PauseAudio()
    {
        audioSource.Pause();
    }

    private void StopAudio()
    {
        audioSource.Stop();
    }

    private IEnumerator UpdateAudioTime()
    {
        while(audioSource.isPlaying)
        {
            //replace first 5 characters with the new time
            _timeCode.text = TimeSpan.FromSeconds(audioSource.time).ToString(@"mm\:ss") + _timeCode.text.Substring(5);
            _knob.style.left = new StyleLength((audioSource.time / audioSource.clip.length) * _timeline.resolvedStyle.width);
            yield return new WaitForSeconds(1);
        }
    }
}
