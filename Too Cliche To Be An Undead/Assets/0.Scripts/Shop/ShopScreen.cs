using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : UIScreenBase
{
    [field: SerializeField] public ShopLevel[] ShopLevels { get; private set; }
    [field: SerializeField] public Button closeButton;
    public Shop RelatedShop { get; private set; }

    [field: SerializeField] public TextMeshProUGUI SkillName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SkillDescription { get; private set; }
    [field: SerializeField] public TextMeshProUGUI SkillCost { get; private set; }
    [field: SerializeField] public Image SkillIcon { get; private set; }

    [field: SerializeField, ReadOnly] public SCRPT_LevelData CurrentSelectedLevelData { get; private set; }
    [field: SerializeField, ReadOnly] public ShopLevel CurrentSelectedLevel { get; private set; }

    [SerializeField] private TextMeshProUGUI playerMoneyText;

    protected override void Awake()
    {
        base.Awake();
        this.Close();
        closeButton.onClick.AddListener(CloseShop);

        if (DataKeeper.Instance.unlockedLevels == null || DataKeeper.Instance.unlockedLevels.Count <= 0) return;

        foreach (var item in ShopLevels)
        {
            if (DataKeeper.Instance.unlockedLevels.Contains(item.ID)) item.Unlock(reloadUnlock: true);
        }
    }

    protected override void EventsSubscriber()
    {
        ShopEvents.OnShopAnimEnded += OnShopAnimEnded;
    }

    protected override void EventsUnSubscriber()
    {
        ShopEvents.OnShopAnimEnded -= OnShopAnimEnded;
    }

    private void OnShopAnimEnded()
    {
        PlayerInputsEvents.OnCancelButton += CloseShop;
        PlayerInputsEvents.OnValidateButton += OnValidateInput;
        base.Open();
        this.OpenUI();
    }

    public override void Open(bool ignoreTweens = false)
    {
        base.Open(ignoreTweens);
        ShopLevels[0].Select();
    }

    private void CloseShop(int playerIdx)
    {
        PlayerInputsEvents.OnCancelButton -= CloseShop;
        PlayerInputsEvents.OnValidateButton -= OnValidateInput;
        base.Close();
        this.CloseUI();
    }
    private void CloseShop() => CloseShop(0);

    private void OnValidateInput(int playerIdx)
    {
        CurrentSelectedLevel?.TryUnlock();
    }

    public void Setup(Shop relatedShop)
    {
        RelatedShop = relatedShop;
        foreach (var item in ShopLevels)
        {
            item.Setup(relatedShop);
            item.OnSelectLevel += OnSelectLevel;
            item.OnDeselectLevel += OnDeselectLevel;
            item.OnUnlock += OnLevelUnlock;
        }
        UpdatePlayerMoney();
    }

    private void OnSelectLevel(ShopLevel level)
    {
        this.CurrentSelectedLevel?.Deselect();
        this.CurrentSelectedLevel = level;
        this.CurrentSelectedLevelData = level.Data;
        SetupPanels(level.Data);
    }

    private void OnDeselectLevel(ShopLevel level)
    {
        if (level != this.CurrentSelectedLevel) return;
        this.CurrentSelectedLevel = null;
        this.CurrentSelectedLevelData = null;
        SetupPanels(null);
    }

    private void SetupPanels(SCRPT_LevelData _data = null)
    {
        if (_data == null)
        {
            this.SkillName.text = "";
            this.SkillDescription.text = "";
            this.SkillCost.text = "";
            this.SkillIcon.enabled = false;
            this.SkillIcon.sprite = null;
            return;
        }

        this.SkillName.text = _data.LevelName;
        this.SkillDescription.text = _data.LevelDescription;
        this.SkillCost.text = "x " + _data.Cost;
        this.SkillIcon.enabled = true;
        this.SkillIcon.sprite = _data.LevelSprites.BoughedSprite;
    }

    private void UpdatePlayerMoney()
    {
        playerMoneyText.text = "x" + PlayerCharacter.GetMoney();
    }
    private void OnLevelUnlock(ShopLevel lvl) => UpdatePlayerMoney();
}
