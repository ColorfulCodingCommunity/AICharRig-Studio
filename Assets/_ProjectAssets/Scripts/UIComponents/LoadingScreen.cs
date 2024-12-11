using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoSingleton<LoadingScreen>
{
    [SerializeField] 
    private GameObject wrapper;

    [Space]
    [SerializeField]
    private Image circularImage;
    [SerializeField]
    private TMPro.TextMeshProUGUI loadingText;

    public void Show()
    {
        wrapper.SetActive(true);
    }

    public void Hide()
    {
        wrapper.SetActive(false);
    }

    public void SetState(float progress, string text)
    {
        loadingText.text = text;

        circularImage.fillAmount = progress;
    }
}
