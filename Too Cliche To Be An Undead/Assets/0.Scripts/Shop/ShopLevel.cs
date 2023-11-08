using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class ShopLevel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Button button;

    [SerializeField] private Image selfImage;
    [SerializeField] private Image halo;

    [SerializeField] private ShopLevel[] levelsToActivate;
    [SerializeField] private Image[] glowLiaisons;


    [field: SerializeField] public SO_ShopUpgrade Data { get; private set; }

    private Shop shop;

    [SerializeField] private bool isActive = false;
    private bool isUnlocked = false;

    public Button GetButon { get => button; }
    public int ID { get => Data.ID; }

    private bool isTweening = false;

    public event Action<ShopLevel> OnSelectLevel;
    public event Action<ShopLevel> OnDeselectLevel;
    public event Action<ShopLevel> OnUnlock;

    private void Awake()
    {
        if (DataKeeper.Instance.unlockedLevels.Contains(this.ID))
        {
            Unlock(true);
            selfImage.sprite = isUnlocked ? Data.LevelSprites.BoughedSprite :
                                            Data.LevelSprites.UnlockedSprite;
        }
        else
        {
            selfImage.sprite = button.interactable ? Data.LevelSprites.UnlockedSprite :
                                                     Data.LevelSprites.LockedSprite;
        }

        halo.sprite = selfImage.sprite;
    }

    public void Setup(Shop _shop)
    {
        this.shop = _shop;
    }

    public void SetActive(bool active)
    {
        if (isActive == active) return;

        isActive = active;
        button.interactable = isActive;

        LeanTween.scale(this.GetComponent<RectTransform>(), new Vector3(1.2f, 1.2f, 1.2f), .35f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true).
        setOnComplete(() =>
        {
            LeanTween.scale(this.GetComponent<RectTransform>(), Vector3.one, .35f).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true);
        });
    }

    public bool TryUnlock()
    {
        if (isUnlocked)
        {
            OnTryUnlockFail();
            return false;
        }
        if (!InventoryManager.Instance.TryBuy(Data.Cost))
        {
            OnTryUnlockFail();
            return false;
        }
        BoughtUpgradeFeedback();
        Unlock();

        return true;
    }

    private void OnTryUnlockFail()
    {
        if (isUnlocked)
        {
            if (isTweening) return;
            LeanTween.rotate(button.gameObject, new Vector3(0, 0, 5), .15f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                LeanTween.rotate(button.gameObject, new Vector3(0, 0, -5), .30f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    LeanTween.rotate(button.gameObject, new Vector3(0, 0, 0), .15f).setEase(LeanTweenType.easeOutSine).setIgnoreTimeScale(true)
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

    public void Unlock(bool reloadUnlock = false)
    {
        if (isUnlocked) return;
        isUnlocked = true;

        this.BoughtNewLevel(Data);

        foreach (var item in glowLiaisons)
        {
            if (item.fillAmount != 0) continue;
            LeanTween.value(item.fillAmount, 1, .7f).setOnUpdate((float val) => item.fillAmount = val).setIgnoreTimeScale(true);
        }

        LeanTween.delayedCall(.7f, () =>
        {
            foreach (var item in levelsToActivate)
            {
                item.SetActive(true);
                item.selfImage.sprite = item.Data.LevelSprites.UnlockedSprite;
                item.halo.sprite = item.Data.LevelSprites.UnlockedSprite;
            }
        }).setIgnoreTimeScale(true);

        this.selfImage.sprite = Data.LevelSprites.BoughedSprite;
        this.halo.sprite = Data.LevelSprites.BoughedSprite;
        this.halo.gameObject.SetActive(true);

        if (!reloadUnlock)
        {
            DataKeeper.Instance.unlockedLevels.Add(this.ID);
            shop.PlayAudio(shop.ShopAudioData.buyAudio);
        }
    }

    private void BoughtUpgradeFeedback()
    {
        if (isTweening) return;
        isTweening = true;
        LeanTween.scale(this.GetComponent<RectTransform>(), Vector3.one, .2f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true)
        .setOnComplete(() =>
        {
            isTweening = false;
        });
    }

    private void CantUpgradeFeedback()
    {
        if (isTweening) return;
        isTweening = true;
        shop.PlayAudio(shop.ShopAudioData.notEnoughMoneyAudio);
        LeanTween.rotate(button.gameObject, new Vector3(0, 0, 5), .15f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true);
        LeanTween.moveLocalX(this.gameObject, this.transform.localPosition.x + 10, .15f).setIgnoreTimeScale(true)
        .setOnComplete(() =>
        {
            LeanTween.rotate(button.gameObject, new Vector3(0, 0, 0), .15f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true);
            LeanTween.moveLocalX(this.gameObject, this.transform.localPosition.x - 20, .15f).setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                LeanTween.rotate(button.gameObject, new Vector3(0, 0, 0), .15f).setEase(LeanTweenType.easeInSine).setIgnoreTimeScale(true);
                LeanTween.moveLocalX(this.gameObject, this.transform.localPosition.x + 10, .15f).setIgnoreTimeScale(true)
                .setOnComplete(() => isTweening = false);
            });
        });
    }

    public void OnClick() => TryUnlock();
    public void OnPointerEnter(PointerEventData eventData) => Select();
    public void OnSelect(BaseEventData eventData) => Select();
    public void OnPointerExit(PointerEventData eventData)
    { }
    public void OnDeselect(BaseEventData eventData) => Deselect();

    public void Select()
    {
        OnSelectLevel?.Invoke(this);

        if (!button.interactable) return;

        selfImage.sprite = isUnlocked ? Data.LevelSprites.BoughedSprite :
                                        Data.LevelSprites.UnlockedSprite;

        halo.sprite = selfImage.sprite;
    }

    public void Deselect()
    {
        OnDeselectLevel?.Invoke(this);

        if (!button.interactable) return;

        selfImage.sprite = isUnlocked ? Data.LevelSprites.BoughedSprite :
                                        Data.LevelSprites.UnlockedSprite;

        halo.sprite = selfImage.sprite;
    }
}
