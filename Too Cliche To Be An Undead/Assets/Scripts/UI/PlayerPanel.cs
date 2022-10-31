using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerIdxText;
    public TextMeshProUGUI PlayerIdxText { get => playerIdxText; }

    [SerializeField] private Image characterImage;
    public Image CharacterImage { get => characterImage; }

    [SerializeField] private Button leftArrow;
    [SerializeField] private Button rightArrow;

    private int playerID;

    private int characterIdx;

    public PlayerPanelsManager panelsManager;

    public void Enable()
    {
        SetImageOpacity(ref characterImage, 1);
        SetImageOpacity(ref leftArrow, 1);
        SetImageOpacity(ref rightArrow, 1);

        leftArrow.gameObject.SetActive(true);
        rightArrow.gameObject.SetActive(true);
    }

    public void Setup(int id)
    {
        playerIdxText.text = "Player " + (id + 1);

        playerID = id;

        Enable();
    }

    public void ChangePreset(bool left)
    {
        if (left)
        {
            characterIdx--;
            if (characterIdx < 0) characterIdx = 3;
        }
        else
        {
            characterIdx++;
            if (characterIdx > 3) characterIdx = 0;
        }

        characterImage.sprite = panelsManager.GetCharacterSprite(characterIdx);
        DataKeeper.Instance.playersDataKeep[playerID].character = (GameManager.E_CharactersNames)characterIdx;
    }

    public void ResetPanel()
    {
        playerIdxText.text = "Press select to join";

        SetImageOpacity(ref characterImage, 0);
        SetImageOpacity(ref leftArrow, 0);
        SetImageOpacity(ref rightArrow, 0);

        leftArrow.gameObject.SetActive(false);
        rightArrow.gameObject.SetActive(false);
    }

    private void SetImageOpacity(ref Image i, int opacity)
    {
        Color c = i.color;
        c.a = opacity;
        i.color = c;
    }
    private void SetImageOpacity(ref Button b, int opacity)
    {
        Color c = b.image.color;
        c.a = opacity;
        b.image.color = c;
    }
}