using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] private RectTransform panel;

    [SerializeField] private Button panelButton;
    public Button PanelButton { get => panelButton; }

    [field: SerializeField] public TextMeshProUGUI ButtonText { get; private set; }

    [SerializeField] private Image panelImage;
    [SerializeField] private Color wrongColor;
    [field: SerializeField] public Color DisabledColor { get; private set; }

    [SerializeField] private CanvasGroup arrows;

    [SerializeField] private CanvasGroup cornersGroup;
    [SerializeField] private Image[] cornersImages;

    public Image PanelImage { get => panelImage; }

    [SerializeField] private Image playerToken;

    [SerializeField] private TextMeshProUGUI pressToJoin;

    [field: SerializeField] public PlayerPanelsManager.S_ImageByCharacter currentCharacter { get; private set; }
    private int currentCharacterIdx;
    private int currentPlayerIdx;

    public int CurrentCharacterIdx { get => currentCharacterIdx; }

    [field: SerializeField] public bool isValid { get; private set; }

    private bool isEnabled = false;
    public bool IsEnabled { get => isEnabled; }

    [System.Serializable] 
    public class S_TakenTokensByPlayerIndex
    {
        public int playerIndex;
        public Image token;

        public S_TakenTokensByPlayerIndex(int _playerIndex, Image _token)
        {
            playerIndex = _playerIndex;
            token = _token;
        }
    }

    [SerializeField] private LeanTweenType inType = LeanTweenType.easeInSine;
    [SerializeField] private LeanTweenType outType = LeanTweenType.easeOutSine;

    [SerializeField] private Vector3 scaleGoal = new Vector3(1.2f,1.2f,1.2f);

    [SerializeField] private float scaleTime = .4f;

    public PlayerPanelsManager panelsManager;
    [field: SerializeField] public int panelID { get; private set; }

    public void Enable()
    {
        ScaleUpAndDown();
        this.panelImage.color = Color.white;

        if (pressToJoin != null)
            pressToJoin.alpha = 0;

        isEnabled = true;

        foreach (var item in cornersImages)
        {
            item.color = PlayersManager.Instance.PlayerColorByIndex[currentPlayerIdx];
        }
        cornersGroup.alpha = 1;
        arrows.alpha = 1;
    }

    public void Disable()
    {
        isEnabled = false;
        this.panelImage.color = DisabledColor;

        if (pressToJoin != null)
            pressToJoin.alpha = 1;

        cornersGroup.alpha = 0;
        arrows.alpha = 0;
    }

    public void JoinPanel(int id)
    {
        playerToken.sprite = panelsManager.tokensByIndex[id];
        playerToken.SetAlpha(1);

        currentPlayerIdx = id;

        Enable();
    }

    public void QuitPanel(int id)
    {
        playerToken.SetAlpha(0);

        Disable();
    }

    public void ResetPanel(bool tweenScale = true)
    {
        Disable();

        playerToken.SetAlpha(0);

        if (tweenScale)
            ScaleUpAndDown();
    }
    public void SoftReset()
    {
        Disable();
        isValid = true;
        playerToken.SetAlpha(0);
    }

    private void ScaleUpAndDown()
    {
        LeanTween.scale(panel, scaleGoal, scaleTime).setEase(inType).setIgnoreTimeScale(true).setOnComplete(
        () =>
        {
            LeanTween.scale(panel, Vector3.one, scaleTime).setEase(outType).setIgnoreTimeScale(true);
        });
    }

    public void SetCharacterToLast()
    {
        currentCharacterIdx--;
        if (currentCharacterIdx < 0)
            currentCharacterIdx = panelsManager.ImagesByCharacter.Length - 1;

        ChangeCharacter(panelsManager.ImagesByCharacter[currentCharacterIdx], currentCharacterIdx);
    }
    public void SetCharacterToNext()
    {
        currentCharacterIdx++;
        if (currentCharacterIdx >= panelsManager.ImagesByCharacter.Length)
            currentCharacterIdx = 0;

        ChangeCharacter(panelsManager.ImagesByCharacter[currentCharacterIdx], currentCharacterIdx);
    }
    public void ChangeCharacter(PlayerPanelsManager.S_ImageByCharacter newChar, int characterIdx)
    {
        currentCharacter = newChar;

        currentCharacterIdx = characterIdx;

        panelImage.sprite = newChar.image;
    }

    public void AssociateCharacterToPlayer()
    {
        DataKeeper.Instance.playersDataKeep[currentPlayerIdx].character = currentCharacter.character;
    }
}
