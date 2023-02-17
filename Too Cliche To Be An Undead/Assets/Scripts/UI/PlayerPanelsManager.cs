using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Video;

public class PlayerPanelsManager : MonoBehaviour
{

    [SerializeField] private PlayerPanel[] playerPanels;
    public PlayerPanel[] GetPlayerPanels { get => playerPanels; }

    [SerializeField] private UIVideoPlayer videoPlayer;

    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        videoPlayer.SetClipWithoutPlaying(UIVideoPlayer.E_VideoTag.BookOpening);
    }

    public void Begin()
    {
        videoPlayer.StartVideo(WaitForAnimation);
    }

    public void WaitForAnimation(VideoPlayer vp)
    {
        foreach (var item in playerPanels) item.panelsManager = this;

        canvasGroup.alpha = 1;

        PlayersManager.Instance.EnableActions();
        foreach (var item in DataKeeper.Instance.GetCharactersSprites)
        {
            if (item.characterName.Equals(GameManager.E_CharactersNames.Shirley))
            {
                playerPanels[0].Setup(0);
                return;
            }
        }
    }

    public void SetupPanel(int idx)
    {
        playerPanels[idx].Setup(idx);
    }

    public void RemoveAllJoined()
    {
        while (DataKeeper.Instance.playersDataKeep.Count > 1) DataKeeper.Instance.RemoveData(1);
        ResetPanels();
    }

    public void ResetPanels()
    {
        for (int i = 1; i < playerPanels.Length; i++)
        {
            playerPanels[i].ResetPanel(false);
        }
        PlayersManager.Instance.DisableActions();
    }

    public void RemovePanel(int idx)
    {
        if (idx < 1 || idx > 3) return;

        playerPanels[idx].ResetPanel();

        if (idx < 3)
        {
            if (playerPanels[idx + 1].IsSetup)
            {
                playerPanels[idx].Setup(idx);
                playerPanels[idx].CharacterImage.sprite = playerPanels[idx + 1].CharacterImage.sprite;
                playerPanels[idx + 1].ResetPanel();
            }
        }

    }

    public Sprite GetCharacterSprite(int idx)
    {
        foreach (var item in DataKeeper.Instance.GetCharactersSprites)
        {
            if ((int)item.characterName == idx) return item.characterSprite;
        }

        return null;
    }
}
