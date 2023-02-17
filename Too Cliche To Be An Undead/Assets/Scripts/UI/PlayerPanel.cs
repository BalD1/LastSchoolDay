using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerIdxText;
    public TextMeshProUGUI PlayerIdxText { get => playerIdxText; }

    [SerializeField] private RectTransform panel;

    [SerializeField] private Image characterImage;
    public Image CharacterImage { get => characterImage; }

    [SerializeField] private LeanTweenType inType = LeanTweenType.easeInSine;
    [SerializeField] private LeanTweenType outType = LeanTweenType.easeOutSine;

    [SerializeField] private Vector3 scaleGoal = new Vector3(1.2f,1.2f,1.2f);

    [SerializeField] private float scaleTime = .4f;

    private int playerID;

    private int characterIdx;

    public PlayerPanelsManager panelsManager;

    private bool isSetup;
    public bool IsSetup { get => isSetup; }

    public void Enable()
    {
        SetImageOpacity(ref characterImage, 1);

        ScaleUpAndDown();
    }

    public void Setup(int id)
    {
        playerIdxText.text = "Player " + (id + 1);

        playerID = id;

        Enable();

        isSetup = true;
    }

    public void ChangePreset(bool left)
    {
        if (characterImage.isActiveAndEnabled == false) return;

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

    public void ResetPanel(bool tweenScale = true)
    {
        playerIdxText.text = "Press \"select\" to join";

        SetImageOpacity(ref characterImage, 0);

        this.playerID = 0;
        this.characterIdx = 0;

        this.characterImage.sprite = panelsManager.GetCharacterSprite(0);
        isSetup = false;

        if (tweenScale)
            ScaleUpAndDown();
    }

    private void ScaleUpAndDown()
    {
        LeanTween.scale(panel, scaleGoal, scaleTime).setEase(inType).setIgnoreTimeScale(true).setOnComplete(
        () =>
        {
            LeanTween.scale(panel, Vector3.one, scaleTime).setEase(outType).setIgnoreTimeScale(true);
        });
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
