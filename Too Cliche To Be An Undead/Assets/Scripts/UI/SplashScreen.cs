using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private Image splashImage;

    [SerializeField] private TextMeshProUGUI pressAnyKey;

    [SerializeField] private CanvasGroup mainScreen;

    private bool allowMainMenu = false;

    private void Awake()
    {
        mainScreen.alpha = 0;
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
        FadeTarget(splashImage, 1, 1, OnFadeInEnded);
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            ManageInput();
        }
    }

    private void FadeTarget(Image target, int goal, float time, Action onCompleteAction)
    {
        LeanTween.value(target.color.a, goal, time)
            .setOnUpdate(
            (float val) =>
            {
                Color c = target.color;
                c.a = val;
                target.color = c;
            }).setIgnoreTimeScale(true)
            .setOnComplete(onCompleteAction);
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
        //else CancelFadeIn();
    }

    private void OnFadeInEnded()
    {
        allowMainMenu = true;

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
        splashImage.gameObject.SetActive(false);
    }

    private void FadeOutScreen()
    {
        LeanTween.cancel(splashImage.gameObject);
        LeanTween.cancel(pressAnyKey.gameObject);

        FadeTarget(splashImage, 0, .5f, OnFadeOutEnded);
        mainScreen.LeanAlpha(1, .5f).setIgnoreTimeScale(true);
    }

    private void CancelFadeIn()
    {
        LeanTween.cancel(splashImage.gameObject);

        Color c = splashImage.color;
        c.a = 1;
        splashImage.color = c;
        mainScreen.LeanAlpha(1, .5f).setIgnoreTimeScale(true);

        OnFadeOutEnded();
    }
}
