using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopLevel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Button button;

    [SerializeField] private Image selfImage;

    [SerializeField] private ShopPanels panels;

    [SerializeField] private ShopLevel[] levelsToActivate;

    [field: SerializeField] public SCRPT_LevelData Data { get; private set; }

    private Shop shop;

    [SerializeField] private bool isActive = false;
    private bool isUnlocked = false;

    public Button GetButon { get => button; }
    public int ID { get => Data.ID; }

    private bool isTweening = false;

    [System.Serializable]
    public struct Modifier
    {
        public string idName;
        public float amount;
        public StatsModifier.E_StatType stat;

        public Modifier(string _idName, float _amount, StatsModifier.E_StatType _stat)
        {
            idName = _idName;
            amount = _amount;
            stat = _stat;
        }
    }

    private void Awake()
    {
        selfImage.sprite = button.interactable ? Data.LevelSprites.UnlockedSprite :
                                                 Data.LevelSprites.LockedSprite;
    }

    public void SetShop(Shop _s)
    {
        shop = _s;
    }

    public void SetPlayersMoney()
    {
        StringBuilder sb = new StringBuilder("Cost ");
        sb.AppendFormat("{0} / {1}", PlayerCharacter.GetMoney(), Data.Cost);
    }

    public void SetActive(bool active)
    {
        if (isActive == active) return;

        isActive = active;
        button.interactable = isActive;

        LeanTween.scale(this.GetComponent<RectTransform>(), new Vector3(1.2f, 1.2f, 1.2f), .2f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true).
            setOnComplete(() =>
            {
                LeanTween.scale(this.GetComponent<RectTransform>(), Vector3.one, .2f).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true);
            });
    }

    public bool TryUnlock()
    {
        if (isUnlocked) return false;
        if (!PlayerCharacter.HasEnoughMoney(Data.Cost)) return false;

        PlayerCharacter.RemoveMoney(Data.Cost, false);
        Unlock();

        return true;
    }

    public void Unlock(bool reloadUnlock = false)
    {
        if (isUnlocked) return;
        isUnlocked = true;

        shop.UpdateCostsMoney();

        if (Data.modifiers != null)
        {
            foreach (var item in GameManager.Instance.playersByName)
            {
                foreach (var modif in Data.modifiers) item.playerScript.AddModifier(modif.idName, modif.amount, modif.stat);

                item.playerScript.selfReviveCount += Data.revivesToAdd;
            }
        }

        foreach (var item in levelsToActivate)
        {
            item.SetActive(true);
            item.selfImage.sprite = item.Data.LevelSprites.UnlockedSprite;
        }

        this.selfImage.sprite = Data.LevelSprites.BoughedSprite;

        if (!reloadUnlock)
            DataKeeper.Instance.unlockedLevels.Add(this.ID);

        PlayerCharacter.LevelUp();
    }

    public void OnClick()
    {
        if (isTweening) return;

        isTweening = true;

        // check if players had enough money to buy the upgrade
        if (TryUnlock())
        {
            BoughtUpgradeFeedback();
        }
        else
        {
            // if the upgrade is already bought, do a little animation
            if (isUnlocked)
            {
                LeanTween.rotate(button.gameObject, new Vector3(0, 0, 5), .15f).setEase(LeanTweenType.easeInSine)
                .setOnComplete(() =>
                {
                    LeanTween.rotate(button.gameObject, new Vector3(0, 0, -5), .30f).setEase(LeanTweenType.easeInSine)
                    .setOnComplete(() =>
                    {
                        LeanTween.rotate(button.gameObject, new Vector3(0, 0, 0), .15f).setEase(LeanTweenType.easeOutSine)
                        .setOnComplete(() => isTweening = false);
                    }
                    );
                });
            }
            else
            {
                CantUpgradeFeedback();
            }
        }
    }

    private void BoughtUpgradeFeedback()
    {
        LeanTween.scale(this.GetComponent<RectTransform>(), Vector3.one, .2f).setEase(LeanTweenType.easeInSine)
        .setOnComplete(() =>
        {
            isTweening = false;
        });
    }

    private void CantUpgradeFeedback()
    {
        LeanTween.moveLocalX(this.gameObject, this.transform.localPosition.x + 1000, .3f)
        .setOnComplete(() =>
        {
            Debug.Log("yo");
            LeanTween.moveLocalX(this.gameObject, this.transform.localPosition.x - 10, .3f)
            .setOnComplete(() => isTweening = false);
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetupPanels();

        if (!button.interactable) return;

        selfImage.sprite = isUnlocked ? Data.LevelSprites.BoughedSprite :
                                           Data.LevelSprites.UnlockedSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SCRPT_LevelData selectedLevel = panels.currentSelectedLevel;

        if (selectedLevel == null)
        {
            panels.SkillName.text = "";
            panels.SkillDescription.text = "";
            panels.SkillIcon.enabled = false;
            panels.SkillIcon.sprite = null;
        }
        else
        {
            SetupPanels(selectedLevel);
        }

        if (!button.interactable) return;

        selfImage.sprite = isUnlocked ? Data.LevelSprites.BoughedSprite :
                                           Data.LevelSprites.UnlockedSprite;
    }

    public void OnSelect(BaseEventData eventData)
    {
        panels.currentSelectedLevel = this.Data;
        SetupPanels();

        if (!button.interactable) return;

        selfImage.sprite = isUnlocked ? Data.LevelSprites.BoughedSprite :
                                           Data.LevelSprites.UnlockedSprite;
    }

    private void SetupPanels(SCRPT_LevelData _data = null)
    {
        if (_data == null) _data = this.Data;

        panels.SkillName.text = _data.LevelName;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(_data.LevelDescription);

        sb.Append(PlayerCharacter.GetMoney().ToString());
        sb.Append(" / ");
        sb.Append(_data.Cost);

        panels.SkillDescription.text = sb.ToString();
        panels.SkillIcon.enabled = true;
        panels.SkillIcon.sprite = _data.LevelSprites.BoughedSprite;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (eventData.selectedObject.GetComponent<ShopLevel>() == null)
            panels.currentSelectedLevel = null;

        if (!button.interactable) return;

        selfImage.sprite = isUnlocked ? Data.LevelSprites.BoughedSprite :
                                           Data.LevelSprites.UnlockedSprite;
    }
}
