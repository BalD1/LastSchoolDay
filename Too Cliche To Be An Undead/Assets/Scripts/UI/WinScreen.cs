using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup winGroup;

    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private CanvasGroup buttonsPanel;
    [SerializeField] private int panelShowCount = 1;

    public void Begin()
    {
        UIManager.Instance.FadeScreen(fadeOut: true, onCompleteAction: () =>
        {
            videoPlayer.Play();

            winGroup.LeanAlpha(1, .25f).setIgnoreTimeScale(true);

            StartCoroutine(Animation());
        });
    }

    private IEnumerator Animation()
    {
        yield return new WaitForSecondsRealtime(1);

        buttonsPanel.LeanAlpha(1, .25f).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            winGroup.blocksRaycasts = true;
            winGroup.interactable = true;
            UIManager.Instance.SelectButton("Win");
        });
    }
}
