using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static UIVideoPlayer;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private UIVideoPlayer videoPlayer;

    [SerializeField] private TextMeshProUGUI pressAnyKey;

    [SerializeField] private CanvasGroup mainScreen;

    [SerializeField] private PlayerPanelsManager panelsManager;

    [SerializeField] private float allowMainMenu_DURATION = 1.5f;
    private float allowMainMenu_TIMER = -1;

    private bool allowMainMenu = false;

    private void Awake()
    {
        panelsManager.gameObject.SetActive(false);
        mainScreen.alpha = 0;
        videoPlayer.SetNewVideo(E_VideoTag.SplashScreen);
    }

    private void Start()
    {
        if (DataKeeper.Instance.firstPassInMainMenu == false)
        {
            mainScreen.alpha = 1;
            OnFadeOutEnded();
            return;
        }

        DataKeeper.Instance.firstPassInMainMenu = false;
        mainScreen.alpha = 0;
        videoPlayer.FadeVideo(true, 1, OnFadeInEnded);
    }

    private void Update()
    {
        if (allowMainMenu_TIMER > 0)
        {
            allowMainMenu_TIMER -= Time.deltaTime;

            if (allowMainMenu_TIMER <= 0)
            {
                allowMainMenu = true;
            }
        }
        if (Input.anyKey) ManageInput();
    }

    private void FadeTarget(TextMeshProUGUI target, int goal, float time, Action onCompleteAction)
    {
        LeanTween.value(target.color.a, 1, time)
            .setOnUpdate(
            (float val) =>
            {
                Color c = target.color;
                c.a = val;
                target.color = c;
            }).setIgnoreTimeScale(true)
            .setOnComplete(onCompleteAction);
    }

    private void ManageInput()
    {
        if (allowMainMenu) FadeOutScreen();
    }

    private void OnFadeInEnded()
    {
        allowMainMenu_TIMER = allowMainMenu_DURATION;
        LeanTween.value(pressAnyKey.color.a, 1, 1)
            .setOnUpdate(
            (float val) =>
            {
                Color c = pressAnyKey.color;
                c.a = val;
                pressAnyKey.color = c;
            }).setIgnoreTimeScale(true).setLoopPingPong(-1);
    }

    private void OnFadeOutEnded()
    {
        UIManager.Instance.SelectButton("MainMenu");
        panelsManager.gameObject.SetActive(true);
    }

    private void FadeOutScreen()
    {
        allowMainMenu = false;
        LeanTween.cancel(videoPlayer.gameObject);
        LeanTween.cancel(pressAnyKey.gameObject);

        videoPlayer.FadeVideo(false, 1, OnFadeOutEnded);
        pressAnyKey.gameObject.SetActive(false);
        mainScreen.LeanAlpha(1, .5f).setIgnoreTimeScale(true);
    }
}
