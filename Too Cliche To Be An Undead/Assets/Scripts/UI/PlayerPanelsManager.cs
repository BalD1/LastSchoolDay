using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPanelsManager : MonoBehaviour
{
    [SerializeField] private PlayerPanel[] playerPanels;

    public void Begin()
    {
        foreach (var item in playerPanels) item.panelsManager = this;

        PlayersManager.Instance.EnableActions();
        foreach (var item in DataKeeper.Instance.GetCharactersSprites)
        {
            if (item.characterName.Equals(GameManager.E_CharactersNames.Shirley))
            {
                playerPanels[0].CharacterImage.sprite = item.characterSprite;
                playerPanels[0].Setup(0);
                return;
            }
        }
    }

    public void SetupPanel(int idx)
    {
        playerPanels[idx].Setup(idx);
    }

    public void ResetPanels()
    {
        foreach (var item in playerPanels) item.ResetPanel();
        PlayersManager.Instance.DisableActions();
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
