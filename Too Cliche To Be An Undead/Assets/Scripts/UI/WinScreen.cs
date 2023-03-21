using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup winGroup;

    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private CanvasGroup buttonsPanel;
    [SerializeField] private int panelShowCount = 1;

    [SerializeField] private CanvasGroup scorePanel;
    [SerializeField] private PlayersScorePanelsController scorePanelsController;

    [SerializeField] private Image youSurvivedImage;

    public void Begin()
    {
        UIManager.Instance.FadeScreen(fadeOut: true, onCompleteAction: () =>
        {
            videoPlayer.Play();

            winGroup.LeanAlpha(1, .25f).setIgnoreTimeScale(true);

            winGroup.interactable = true;
            winGroup.blocksRaycasts = true;

            StartCoroutine(Animation());
        });
    }

    private IEnumerator Animation()
    {
        float leanTime = .25f;

        yield return new WaitForSecondsRealtime(2);

        youSurvivedImage.LeanAlpha(1, leanTime).setIgnoreTimeScale(true);

        yield return new WaitForSecondsRealtime(2);

        youSurvivedImage.LeanAlpha(0, leanTime).setIgnoreTimeScale(true);
        scorePanel.LeanAlpha(1, leanTime).setIgnoreTimeScale(true);

        scorePanelsController.Begin();
    }

    public void ShowButtonsPanel()
    {
        scorePanel.LeanAlpha(0, .25f).setIgnoreTimeScale(true);
        buttonsPanel.LeanAlpha(1, .25f).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            winGroup.blocksRaycasts = true;
            winGroup.interactable = true;
            UIManager.Instance.SelectButton("Win");
        });
    }
}
