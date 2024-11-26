using System.Collections.Generic;
using UnityEngine;

public class DrivingFaceControls : MonoBehaviour
{
    public SkinnedMeshRenderer targetMeshRenderer;

    public Transform lEyeBone;
    public Transform rEyeBone;
    public Transform headBone;

    public AudioSource talkingAudioSource;

    [Space]
    public SlidersManager slidersManager;

    private List<float> initialSliderValues;
    private List<KeyableSlider> _sliders;

    private async void Start()
    {
        initialSliderValues = new List<float>();

        _sliders = await slidersManager.GetSliders();

        foreach (var slider in _sliders)
        {
            initialSliderValues.Add(slider.GetValue());
        }
    }
    public void Reset()
    {
        for (int i = 0; i < _sliders.Count; i++)
        {
            _sliders[i].SetValue(initialSliderValues[i]);
        }
    }

    public void SetLeftBrow(float val)
    {
        var raiseBrowLeftName = "RaiseBrowLeft";
        var raiseBrowLeftIndex = targetMeshRenderer.sharedMesh.GetBlendShapeIndex(raiseBrowLeftName);
        int value = (int)(100 * 2 * Mathf.Clamp(val - 0.5f, 0, 0.5f));
        targetMeshRenderer.SetBlendShapeWeight(raiseBrowLeftIndex, value);

        var lowerBrowLeftName = "SadBrowLeft";
        var lowerBrowLeftIndex = targetMeshRenderer.sharedMesh.GetBlendShapeIndex(lowerBrowLeftName);
        value = (int)(100 * 2 * Mathf.Clamp(0.5f - val, 0, 0.5f));
        targetMeshRenderer.SetBlendShapeWeight(lowerBrowLeftIndex, value);
    }

    public void SetRightBrow(float val)
    {
        var raiseBrowRightName = "RaiseBrowRight";
        var raiseBrowRightIndex = targetMeshRenderer.sharedMesh.GetBlendShapeIndex(raiseBrowRightName);
        int value = (int)(100 * 2 * Mathf.Clamp(val - 0.5f, 0, 0.5f));
        targetMeshRenderer.SetBlendShapeWeight(raiseBrowRightIndex, value);

        var lowerBrowRightName = "SadBrowRight";
        var lowerBrowRightIndex = targetMeshRenderer.sharedMesh.GetBlendShapeIndex(lowerBrowRightName);
        value = (int)(100 * 2 * Mathf.Clamp(0.5f - val, 0, 0.5f));
        targetMeshRenderer.SetBlendShapeWeight(lowerBrowRightIndex, value);
    }

    public void SetSmileLeft(float val)
    {
        var smileLeftName = "SmileLeft";
        var smileLeftIndex = targetMeshRenderer.sharedMesh.GetBlendShapeIndex(smileLeftName);
        int value = (int)(100 * 2 * Mathf.Clamp(val - 0.5f, 0, 0.5f));
        targetMeshRenderer.SetBlendShapeWeight(smileLeftIndex, value);

        var sadLeftName = "SadLeft";
        var sadLeftIndex = targetMeshRenderer.sharedMesh.GetBlendShapeIndex(sadLeftName);
        value = (int)(100 * 2 * Mathf.Clamp(0.5f - val, 0, 0.5f));
        targetMeshRenderer.SetBlendShapeWeight(sadLeftIndex, value);
    }

    public void SetSmileRight(float val)
    {
        var smileRightName = "SmileRight";
        var smileRightIndex = targetMeshRenderer.sharedMesh.GetBlendShapeIndex(smileRightName);
        int value = (int)(100 * 2 * Mathf.Clamp(val - 0.5f, 0, 0.5f));
        targetMeshRenderer.SetBlendShapeWeight(smileRightIndex, value);

        var sadRightName = "SadRight";
        var sadRightIndex = targetMeshRenderer.sharedMesh.GetBlendShapeIndex(sadRightName);
        value = (int)(100 * 2 * Mathf.Clamp(0.5f - val, 0, 0.5f));
        targetMeshRenderer.SetBlendShapeWeight(sadRightIndex, value);
    }

    public void SetMouthOpen(float val)
    {
        var mouthOpenName = "OpenMouth";
        var mouthOpenIndex = targetMeshRenderer.sharedMesh.GetBlendShapeIndex(mouthOpenName);
        int value = (int)(100 * Mathf.Clamp(val, 0, 1f));
        targetMeshRenderer.SetBlendShapeWeight(mouthOpenIndex, value);
    }

    public void SetMouthPuff(float val)
    {
        var mouthPuffName = "MouthPuff";
        var mouthPuffIndex = targetMeshRenderer.sharedMesh.GetBlendShapeIndex(mouthPuffName);
        int value = (int)(100 * Mathf.Clamp(val, 0, 1f));
        targetMeshRenderer.SetBlendShapeWeight(mouthPuffIndex, value);
    }

    public void SetBlink(float val)
    {
        int value = (int)(100 * Mathf.Clamp(val, 0, 1f));

        var eyesLeftBlinkName = "WinkLeft";
        var eyesLeftBlinkIndex = targetMeshRenderer.sharedMesh.GetBlendShapeIndex(eyesLeftBlinkName);
        targetMeshRenderer.SetBlendShapeWeight(eyesLeftBlinkIndex, value);

        var eyesRightBlinkName = "WinkRight";
        var eyesRightBlinkIndex = targetMeshRenderer.sharedMesh.GetBlendShapeIndex(eyesRightBlinkName);
        targetMeshRenderer.SetBlendShapeWeight(eyesRightBlinkIndex, value);
    }

    public void SetEyesDirection(float val)
    {
        float lEyeLLimit = 35f;
        float lEyeRLimit = -40f;

        float rEyeLLimit = 40f;
        float rEyeRLimit = -35f;

        float lAngle, rAngle;
        //Look to the right
        if (val < 0.5)
        {
            lAngle = lEyeLLimit * 2 * (0.5f - val);
            rAngle = rEyeLLimit * 2 * (0.5f - val);
        }
        else
        {
            lAngle = lEyeRLimit * 2 * (val - 0.5f);
            rAngle = rEyeRLimit * 2 * (val - 0.5f);
        }

        var localAngleLEye = lEyeBone.localEulerAngles;
        localAngleLEye.y = lAngle;
        lEyeBone.localEulerAngles = localAngleLEye;

        var localAngleREye = rEyeBone.localEulerAngles;
        localAngleREye.y = rAngle;
        rEyeBone.localEulerAngles = localAngleREye;
    }

    public void SetHeadYaw(float val)
    {
        var headAngle = headBone.localEulerAngles;

        float headRLimit = 80f;
        float headLLimit = -80f;

        headAngle.y = headLLimit + (headRLimit - headLLimit) * val;
        headBone.localEulerAngles = headAngle;
    }

    public void SetHeadPitch(float val)
    {
        var headAngle = headBone.localEulerAngles;

        float headDownLimit = 45f;
        float headUpLimit = -45f;

        headAngle.x = headUpLimit + (headDownLimit - headUpLimit) * val;
        headBone.localEulerAngles = headAngle;
    }

    public void SetHeadRoll(float val)
    {
        var headAngle = headBone.localEulerAngles;

        float headRLimit = -45f;
        float headLLimit = 45f;

        headAngle.z = headLLimit + (headRLimit - headLLimit) * val;
        headBone.localEulerAngles = headAngle;
    }

    public void SwitchAudioSource()
    {
        talkingAudioSource.enabled = !talkingAudioSource.enabled;
    }
}
