using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreloadScreen : MonoBehaviour
{
    [SerializeField] private GameObject panelsManager;

    [SerializeField] private ButtonArgs_Scene sceneArgs;

    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private Image mainScreenImage;
    [SerializeField] private RawImage videoPlayerImage;

    [SerializeField] private float fadeDuration = .5f;

    [SerializeField] private RectTransform playTutoButton;
    [SerializeField] private RectTransform skipTutoButton;

    PlayerCharacter p1;

    public void BeginScreen()
    {
        GameManager.Instance.allowQuitLobby = false;

        mainScreenImage.SetAlpha(0);
        videoPlayerImage.LeanAlpha(0, fadeDuration);
        panelsManager.GetComponent<PlayerPanelsManager>().CanvasGroup.LeanAlpha(0, fadeDuration).setOnComplete( () =>
        {
            canvasGroup.LeanAlpha(1, fadeDuration).setOnComplete(() =>
            {
                p1 = GameManager.Player1Ref;

                p1.D_validateInput += PlayTutorial;
                p1.D_fourthActionButton += SkipTutorial;
                p1.D_cancelInput += Back;
            });
        });
    }

    public void PlayTutorial()
    {
        AnimateButton(playTutoButton, false);
    }

    public void SkipTutorial()
    {
        AnimateButton(skipTutoButton, true);
    }

    private void AnimateButton(RectTransform rectTransform, bool launchSkipTuto)
    {
        LeanTween.scale(rectTransform, Vector3.one * 1.2f, .25f)
            .setOnComplete(() =>
            {
                canvasGroup.LeanAlpha(0, .5f).setIgnoreTimeScale(true);
                LeanTween.scale(rectTransform, Vector3.zero, .5f).setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    Launch(launchSkipTuto);
                });
            }).setIgnoreTimeScale(true);
    }

    private void Launch(bool skipTuto)
    {
        DataKeeper.Instance.alreadyPlayedTuto = skipTuto;
        DataKeeper.Instance.skipTuto = skipTuto;

        p1.D_validateInput -= PlayTutorial;
        p1.D_fourthActionButton -= SkipTutorial;
        p1.D_cancelInput -= Back;

        if (sceneArgs == null)
        {
            sceneArgs = new ButtonArgs_Scene();
            sceneArgs.args = GameManager.E_ScenesNames.MainScene;
        }

        GameManager.ChangeScene(GameManager.E_ScenesNames.MainScene);

        this.gameObject.SetActive(false);
    }

    public void Back()
    {
        PlayerPanelsManager ppm = panelsManager.GetComponent<PlayerPanelsManager>();

        p1.D_validateInput -= PlayTutorial;
        p1.D_fourthActionButton -= SkipTutorial;
        p1.D_cancelInput -= Back;

        canvasGroup.LeanAlpha(0, fadeDuration).setOnComplete(() =>
        {
            videoPlayerImage.LeanAlpha(1, fadeDuration);
            ppm.CanvasGroup.LeanAlpha(1, fadeDuration).setOnComplete(() =>
            {
                mainScreenImage.SetAlpha(1);
                this.gameObject.SetActive(false);
                ppm.Refocus();
                GameManager.Instance.allowQuitLobby = true;
            });
        });
    }
}
