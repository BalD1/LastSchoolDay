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
                PlayerInputsEvents.OnValidateButton += PlayTutorial;
                PlayerInputsEvents.OnFourthAction += SkipTutorial;
                PlayerInputsEvents.OnCancelButton += Back;
            });
        });
    }

    public void PlayTutorial(int idx)
    {
        AnimateButton(playTutoButton, false);
    }

    public void SkipTutorial(int idx)
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

        PlayerInputsEvents.OnValidateButton -= PlayTutorial;
        PlayerInputsEvents.OnFourthAction -= SkipTutorial;
        PlayerInputsEvents.OnCancelButton -= Back;

        if (sceneArgs == null)
        {
            sceneArgs = new ButtonArgs_Scene();
            sceneArgs.args = GameManager.E_ScenesNames.MainScene;
        }

        GameManager.ChangeScene(GameManager.E_ScenesNames.MainScene);

        this.gameObject.SetActive(false);
    }

    public void Back(int idx)
    {
        PlayerPanelsManager ppm = panelsManager.GetComponent<PlayerPanelsManager>();

        PlayerInputsEvents.OnValidateButton -= PlayTutorial;
        PlayerInputsEvents.OnFourthAction -= SkipTutorial;
        PlayerInputsEvents.OnCancelButton -= Back;

        canvasGroup.LeanAlpha(0, fadeDuration).setOnComplete(() =>
        {
            videoPlayerImage.LeanAlpha(1, fadeDuration);
            ppm.CanvasGroup.LeanAlpha(1, fadeDuration).setOnComplete(() =>
            {
                mainScreenImage.SetAlpha(1);
                ppm.Refocus();
                GameManager.Instance.allowQuitLobby = true;
            });
        });
    }
}
