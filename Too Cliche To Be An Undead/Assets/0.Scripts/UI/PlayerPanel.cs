using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Progress;

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
    [field: ReadOnly] [field: SerializeField] public PlayerInputs LinkedInputs { get; private set; }
    [field: SerializeField] [field: ReadOnly] public int CurrentInputsIdx { get; private set; }

    private int currentCharacterIdx;
    public int CurrentCharacterIdx { get => currentCharacterIdx; }

    [field: SerializeField] public bool isValid { get; private set; }

    [field: SerializeField] public bool IsEnabled { get; private set; }

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

    private LTDescr ptjTween;

    private void Start()
    {
        SetPTJTween();
    }

    public void Enable()
    {
        if (ptjTween != null) LeanTween.cancel(ptjTween.uniqueId);

        ScaleUpAndDown();
        this.panelImage.color = Color.white;

        if (pressToJoin != null)
            pressToJoin.alpha = 0;

        IsEnabled = true;

        foreach (var item in cornersImages)
        {
            //item.color = PlayersManager.Instance.PlayersColor[CurrentPlayerIdx];
        }
        cornersGroup.alpha = 1;
        arrows.alpha = 1;
    }

    public void Disable()
    {
        SetPTJTween();

        IsEnabled = false;
        if (this.panelImage != null)
            this.panelImage.color = DisabledColor;

        if (pressToJoin != null)
            pressToJoin.alpha = 1;

        cornersGroup.alpha = 0;
        arrows.alpha = 0;
    }

    private void SetPTJTween()
    {
        if (pressToJoin != null)
        {
            if (ptjTween != null) LeanTween.cancel(ptjTween.uniqueId);
            ptjTween = LeanTween.delayedCall(3, () => {

                if (pressToJoin == null) return;
                LeanTween.scale(pressToJoin.rectTransform, Vector3.one * 1.1f, 1.5f)
                .setLoopPingPong()
                .setEase(LeanTweenType.easeInOutBack).setRepeat(2)
                .setIgnoreTimeScale(true);
            }).setRepeat(-1)
            .setIgnoreTimeScale(true);
        }
    }

    public void SetValidity(bool validity)
    {
        isValid = validity;

        this.panelImage.color = isValid ? Color.white : wrongColor;
    }

    public void JoinPanel(PlayerInputs playerInputs)
    {
        if (panelsManager.tokensQueue.Count > 0)
            playerToken.sprite = panelsManager.tokensQueue.Dequeue();
        else 
            playerToken.sprite = panelsManager.PlayerTokens[0];

        playerToken.SetAlpha(1);

        CurrentInputsIdx = playerInputs.InputsID;

        LinkedInputs = playerInputs;

        Enable();
    }

    public void QuitPanel(int id, bool destroy = true)
    {
        if (destroy && LinkedInputs != null)
            Destroy(LinkedInputs.gameObject);
        LinkedInputs = null;
        panelsManager.tokensQueue.Enqueue(playerToken.sprite);

        ResetPanel();
    }

    public void ResetPanel(bool tweenScale = true)
    {
        Disable();
        playerToken.SetAlpha(0);
        CurrentInputsIdx = -1;
        if (tweenScale)
            ScaleUpAndDown();
        this.ChangeCharacter(panelsManager.ImagesByCharacter[0], 0);
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

    public void ChangeCharacter(PlayerPanelsManager.S_ImageByCharacter newChar, int characterIdx)
    {
        currentCharacter = newChar;

        currentCharacterIdx = characterIdx;

        panelImage.sprite = newChar.image;
    }
    public void GetCharacter(out PlayerPanelsManager.S_ImageByCharacter newChar, out int characterIdx)
    {
        newChar = currentCharacter;
        characterIdx = currentCharacterIdx;
    }

    public void AssociateCharacterToPlayer()
    {
        DataKeeper.Instance.playersDataKeep[CurrentInputsIdx].character = currentCharacter.character;
    }
}
