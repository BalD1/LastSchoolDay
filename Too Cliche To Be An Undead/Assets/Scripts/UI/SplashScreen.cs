using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static UIVideoPlayer;
using Coffee.UIEffects;
using UnityEngine.Video;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private UIVideoPlayer videoPlayer;

    [SerializeField] private TextMeshProUGUI pressAnyKey;

    [SerializeField] private CanvasGroup mainScreen;
    [SerializeField] private Image mainScreenBackground;

    [SerializeField] private GameObject raycastBlocker;

    [SerializeField] private PlayerPanelsManager panelsManager;

    [SerializeField] private UITransitionEffect title;

    [SerializeField] private float allowMainMenu_DURATION = 1.5f;
    private float allowMainMenu_TIMER = -1;

    private bool allowMainMenu = false;

    private void Awake()
    {
        panelsManager.gameObject.SetActive(false);
        mainScreen.alpha = 0;
        mainScreenBackground.SetAlpha(0);
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

        videoPlayer.FadeVideo(true, 2, OnFadeInEnded);
    }

    private void Update()
    {
        if (allowMainMenu_TIMER > 0)
        {
            allowMainMenu_TIMER -= Time.deltaTime;

            if (allowMainMenu_TIMER <= 0)
            {
                allowMainMenu = true;
                LeanTween.value(pressAnyKey.color.a, 1, .5f)
                .setOnUpdate(
                (float val) =>
                {
                    Color c = pressAnyKey.color;
                    c.a = val;
                    pressAnyKey.color = c;
                }).setIgnoreTimeScale(true).setLoopPingPong(-1);

                title.Show();
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
    }

    private void OnFadeOutEnded()
    {
        UIManager.Instance.SelectButton("MainMenu");
        raycastBlocker.gameObject.SetActive(false);
        panelsManager.gameObject.SetActive(true);
        title.gameObject.SetActive(false);
    }

    private void FadeOutScreen()
    {
        allowMainMenu = false;
        LeanTween.cancel(videoPlayer.gameObject);
        LeanTween.cancel(pressAnyKey.gameObject);

        mainScreenBackground.SetAlpha(1);

        title.effectPlayer.duration *= .5f;
        title.Hide();

        videoPlayer.FadeVideo(false, 1, OnFadeOutEnded);
        pressAnyKey.gameObject.SetActive(false);
        mainScreen.LeanAlpha(1, .5f).setIgnoreTimeScale(true);
        mainScreenBackground.LeanAlpha(1, .5f).setIgnoreTimeScale(true);
    }
}
