using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFP_BackButton : MonoBehaviour
{
    [SerializeField] private PlayerPanelsManager panelsManager;
    [SerializeField] private UIVideoPlayer videoPlayer;
    [SerializeField] private CanvasGroup canvasGroup;

    public void OnBack()
    {
        videoPlayer.GetVideoPlayer.Stop();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        videoPlayer.FadeVideo(false, 0);

        foreach (var item in panelsManager.GetPlayerPanels)
        {
            item.transform.localScale = Vector2.zero;
        }
    }
}
