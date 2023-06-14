using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private RectTransform selfRect;

    [SerializeField] private Image portrait;
    [SerializeField] private Image portraitBG;
    private RectTransform portraitRect;

    [Header("HP")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image hpBarContainer;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Dash")]
    [SerializeField] private RectTransform dashContainer;
    [SerializeField] private Image dashFill;
    [SerializeField] private Image dashThumbnail;
    [SerializeField] private TextMeshProUGUI dashTimerTXT;

    [Header("Skill")]
    [SerializeField] private RectTransform skillContainer;
    [SerializeField] private Image skillFill;
    [SerializeField] private Image skillThumbnail;
    [SerializeField] private TextMeshProUGUI skillTimerTXT;

    [SerializeField] private Vector3 iconsMaxScale = new Vector3(1.3f, 1.3f, 1.3f);
    [SerializeField] private float maxScaleTime = .7f;
    [SerializeField] private LeanTweenType inType = LeanTweenType.easeInSine;
    [SerializeField] private LeanTweenType outType = LeanTweenType.easeOutSine;

    private SpriteRenderer minimapRenderer;

    private UIManager.CharacterPortrait characterPortrait;
    private int currentPortraitIdx;

    private int ownerIDX;

    private PlayerCharacter owner;

    public void Setup(PlayerCharacter _owner, SpriteRenderer _minimapRenderer, GameManager.E_CharactersNames character, SCRPT_Dash playerDash, SCRPT_Skill playerSkill)
    {
        owner = _owner;
        ownerIDX = _owner.PlayerIndex;

        portraitBG.gameObject.SetActive(false);

        minimapRenderer = _minimapRenderer;

        SetupCharacterPortrait(character);

        SetDashThumbnail();

        owner.OnHealthChange += OnOwnerHealthChange;
        owner.OnSwitchCharacter += OnCharacterSwitch;

        portraitRect = portrait.GetComponent<RectTransform>();
    }

    public void SetSkillThumbnail(Sprite image)
    {
        skillThumbnail.sprite = image;
    }

    public void UpdateSkillThumbnailFill(float fill, bool allowTween = true)
    {
        if (skillFill != null)
        skillFill.fillAmount = fill;

        if (skillTimerTXT == null) return;

        skillTimerTXT.text = (fill * owner.MaxSkillCD_M).ToString("F0");

        if (fill <= 0)
        {
            skillTimerTXT.text = "";
            if (allowTween)
                ScaleTweenObject(skillContainer.gameObject, LeanTweenType.linear, LeanTweenType.easeOutSine);
        }
    }

    private void SetDashThumbnail() => SetDashThumbnail(owner.PlayerDash.Thumbnail);
    public void SetDashThumbnail(Sprite image)
    {
        dashThumbnail.sprite = image;
    }

    public void UpdateDashThumbnailFill(float fill)
    {
        dashFill.fillAmount = fill;

        dashTimerTXT.text = (owner.MaxDashCD_M * fill).ToString("F0");

        if (fill <= 0)
        {
            ScaleTweenObject(dashContainer.gameObject, inType, outType);
            dashTimerTXT.text = "";
        }
    }

    private void OnCharacterSwitch()
    {
        SetDashThumbnail();
        ForceHPUpdate();
        SetupCharacterPortrait();
    }

    private void SetupCharacterPortrait() => SetupCharacterPortrait(owner.GetCharacterName());
    private void SetupCharacterPortrait(GameManager.E_CharactersNames character)
    {
        foreach (var item in UIManager.Instance.CharacterPortraits)
        {
            if (item.characterName.Equals(character)) characterPortrait = item;
        }
        this.portrait.sprite = characterPortrait.characterPortraitsByHP[0].portrait;
    }

    private void OnOwnerHealthChange(bool tookDamages)
    {
        if (tookDamages) OnOwnerTakeDamages();
        else OnOwnerHeal();
    }

    private void OnOwnerHeal() => OnOwnerHPChange(Color.green);
    private void OnOwnerTakeDamages() => OnOwnerHPChange(Color.red);
    private void OnOwnerHPChange(Color leanCol)
    {
        if (portrait == null) return;
        if (portrait.rectTransform == null) return;

        hpBar.fillAmount = (owner.CurrentHP / owner.MaxHP_M);

        StringBuilder sb = new StringBuilder();
        sb.Append(owner.CurrentHP);
        sb.Append(" / ");
        sb.Append(owner.MaxHP_M);
        hpText.text = sb.ToString();

        LeanTween.color(portrait.rectTransform, leanCol, .2f).setEase(inType)
                 .setOnComplete(() =>
                 {
                     LeanTween.color(portrait.rectTransform, Color.white, .2f);
                 });

        if (SetPortrait())
        {
            LeanTween.scale(portraitRect, iconsMaxScale, maxScaleTime / 2).setEase(inType)
                     .setOnComplete(() =>
                     {
                         LeanTween.scale(portraitRect, Vector3.one, maxScaleTime / 2).setEase(outType);
                     });
        }
    }

    public void ForceHPUpdate() => OnOwnerHPChange(Color.white);

    private bool SetPortrait(float currentMaxHP = -1)
    {
        if (currentMaxHP == -1)
            currentMaxHP = owner.MaxHP_M;

        if (currentPortraitIdx + 1 <= characterPortrait.characterPortraitsByHP.Length)
        {
            if (owner.CurrentHP / currentMaxHP * 100 <= 100 * CurrentCharacterPortrait().percentage)
            {
                SetCharacterPortrait(currentPortraitIdx + 1);
                SetPortrait(currentMaxHP);
                return true;
            }
        }

        if (currentPortraitIdx > 0)
        {
            if (owner.CurrentHP / currentMaxHP * 100 > 100 * LastCharacterPortrait().percentage)
            {
                SetCharacterPortrait(currentPortraitIdx - 1);
                SetPortrait(currentMaxHP);
                return true;
            }
        }

        return false;
    }

    private UIManager.CharacterPortraitByHP CurrentCharacterPortrait() => characterPortrait.characterPortraitsByHP[currentPortraitIdx];
    private UIManager.CharacterPortraitByHP NextCharacterPortrait() => characterPortrait.characterPortraitsByHP[currentPortraitIdx + 1];
    private UIManager.CharacterPortraitByHP LastCharacterPortrait() => characterPortrait.characterPortraitsByHP[currentPortraitIdx - 1];
    private void SetCharacterPortrait(int idx)
    {
        if (idx < characterPortrait.characterPortraitsByHP.Length)
            this.portrait.sprite = characterPortrait.characterPortraitsByHP[idx].portrait;

        currentPortraitIdx = idx;

        if (currentPortraitIdx == characterPortrait.characterPortraitsByHP.Length - 1)
        {
            portraitBG.gameObject.SetActive(true);
        }
        else if (portraitBG.isActiveAndEnabled)
        {
            portrait.fillAmount = 1;
            portraitBG.gameObject.SetActive(false);
        }


        minimapRenderer.sprite = this.portrait.sprite;
    }

    public void FillPortrait(float fill) => portrait.fillAmount = fill;

    public void ScaleTweenObject(GameObject target, LeanTweenType _inType, LeanTweenType _outType)
    {
        if (target == null) return;

        LeanTween.cancel(target);

        LeanTween.scale(target, iconsMaxScale, maxScaleTime).setEase(_inType)
        .setOnComplete(() =>
        {
            LeanTween.scale(target, Vector3.one, maxScaleTime).setEase(_outType);
        });
    }

    private void OnDestroy()
    {
        owner.OnHealthChange -= OnOwnerHealthChange;
        owner.OnSwitchCharacter -= OnCharacterSwitch;
        LeanTween.cancel(this.gameObject);
    }
}
