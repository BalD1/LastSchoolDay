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
    [SerializeField] private TextMeshProUGUI skipText;

    [SerializeField] private CanvasGroup mainScreen;
    [SerializeField] private Image mainScreenBackground;

    [SerializeField] private GameObject raycastBlocker;

    [SerializeField] private PlayerPanelsManager panelsManager;

    [SerializeField] private UITransitionEffect title;

    [SerializeField] private SCRPT_MusicData titleScreenMusic;

    [SerializeField] private float allowMainMenu_DURATION = 1.5f;
    private float allowMainMenu_TIMER = -1;

    [SerializeField] private float allowSkipText_DURATION = .5f;
    private float allowSkipText_TIMER = -1;

    private bool allowMainMenu = false;

    private bool skipTextIsVisible = false;
    private bool skippedScreen = false;

    private void Awake()
    {
        if (DataKeeper.Instance.firstPassInMainMenu == false)
        {
            OnFadeOutEnded();
            mainScreen.alpha = 1;

            return;
        }

        panelsManager.gameObject.SetActive(false);
        mainScreen.alpha = 0;
        mainScreenBackground.SetAlpha(0);
        videoPlayer.SetNewVideo(E_VideoTag.SplashScreen);
        pressAnyKey.raycastTarget = false;

        allowSkipText_TIMER = allowSkipText_DURATION;
    }

    private void Start()
    {
        if (DataKeeper.Instance.firstPassInMainMenu == false) return;

        SoundManager.Instance.PlayMusic(titleScreenMusic);

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
                if (skippedScreen) return;

                allowMainMenu = true;
                skipText.alpha = 0;
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

        if (allowSkipText_TIMER > 0) allowSkipText_TIMER -= Time.deltaTime;
        else if (Input.anyKey) ManageInput();
    }

    private void ManageInput()
    {
        if (skippedScreen) return;

        if (allowMainMenu) FadeOutScreen();
        else
        {
            if (!skipTextIsVisible)
            {
                LeanTween.value(skipText.color.a, 1, .15f)
                .setOnUpdate(
                (float val) =>
                {
                    Color c = skipText.color;
                    c.a = val;
                    skipText.color = c;
                }).setIgnoreTimeScale(true).setOnComplete(() => skipTextIsVisible = true);
            }
            else
            {
                skippedScreen = true;
                FadeOutScreen();
            }
        }
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
        this.gameObject.SetActive(false);
        SoundManager.Instance.PlayMusic(SoundManager.E_MusicClipsTags.InLobby);
    }

    private void FadeOutScreen()
    {
        LeanTween.cancel(skipText.gameObject);
        LeanTween.cancel(videoPlayer.gameObject);
        LeanTween.cancel(pressAnyKey.gameObject);

        skipTextIsVisible = false;
        allowMainMenu = false;

        mainScreenBackground.SetAlpha(1);

        title.effectPlayer.duration *= .5f;
        title.Hide();

        videoPlayer.FadeVideo(false, 1, OnFadeOutEnded);
        pressAnyKey.gameObject.SetActive(false);
        skipText.gameObject.SetActive(false);
        mainScreen.LeanAlpha(1, .5f).setIgnoreTimeScale(true);
        mainScreenBackground.LeanAlpha(1, .5f).setIgnoreTimeScale(true);
    }
}
