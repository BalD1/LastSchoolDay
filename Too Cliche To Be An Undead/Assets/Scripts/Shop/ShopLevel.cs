using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopLevel : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI costText;

    [SerializeField] private ShopLevel[] levelsToActivate;

    private Shop shop;

    [SerializeField] private int id;
    [SerializeField] private int cost;

    [SerializeField] private Modifier[] modifiers;
    [SerializeField] private int revivesToAdd = 0;

    [SerializeField] private bool isActive = false;
    private bool isUnlocked = false;

    [Header("Unlocked Colors")]
    [SerializeField] private ColorBlock unlockedColors;

    [Header("Locked Colors")]
    [SerializeField] private ColorBlock lockedColors;

    public Button GetButon { get => button; }
    public int ID { get => id; }

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
        StringBuilder sb = new StringBuilder();

        foreach (var item in modifiers)
        {
            sb.AppendFormat("{0} {1} to your {2} \n", item.amount > 0 ? "Adds" : "Removes", Mathf.Abs(item.amount), StatsModifier.TypeToString_UI(item.stat));
        }

        if (revivesToAdd > 0)
            sb.AppendFormat(" +{0} revive{1} \n", revivesToAdd, revivesToAdd > 1 ? "s" : "");

        buttonText.text = sb.ToString();

        SetPlayersMoney();
    }

    public void SetShop(Shop _s)
    {
        shop = _s;
    }

    public void SetPlayersMoney()
    {
        StringBuilder sb = new StringBuilder("Cost ");
        sb.AppendFormat("{0} / {1}", PlayerCharacter.GetMoney(), cost);

        costText.text = sb.ToString();
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
        if (!PlayerCharacter.HasEnoughMoney(cost)) return false;

        PlayerCharacter.RemoveMoney(cost, false);
        Unlock();

        return true;
    }

    public void Unlock(bool reloadUnlock = false)
    {
        if (isUnlocked) return;
        isUnlocked = true;

        shop.UpdateCostsMoney();

        if (modifiers != null)
        {
            foreach (var item in GameManager.Instance.playersByName)
            {
                foreach (var modif in modifiers) item.playerScript.AddModifier(modif.idName, modif.amount, modif.stat);

                item.playerScript.selfReviveCount += revivesToAdd;
            }
        }

        foreach (var item in levelsToActivate) item.SetActive(true);

        this.button.colors = unlockedColors;

        if (!reloadUnlock)
            DataKeeper.Instance.unlockedLevels.Add(this.id);

        PlayerCharacter.LevelUp();
    }

    /*
    private void UnlockNeighboursAndSetNavigation()
    {
        if (unlockLeft) CheckNeighbour(leftNeighbour);
        if (unlockRight) CheckNeighbour(rightNeighbour);
        if (unlockDown) CheckNeighbour(downNeighbour);
        if (unlockUp) CheckNeighbour(upNeighbour);

        SetSelfNavigationIfNeighbourIsActive();

        leftNeighbour?.SetSelfNavigationIfNeighbourIsActive();
        rightNeighbour?.SetSelfNavigationIfNeighbourIsActive();
        downNeighbour?.SetSelfNavigationIfNeighbourIsActive();
        upNeighbour?.SetSelfNavigationIfNeighbourIsActive();
    }
    

    private void UnlockNeighboursAndSetNavigation()
    {
        foreach (var item in levelsToUnlock) item.Unlock(); 
    }

    private void CheckNeighbour(ShopLevel target)
    {
        if (target == null) return;

        target.SetActive(true);
    }

    
    public void SetSelfNavigationIfNeighbourIsActive()
    {
        Navigation nav = this.button.navigation;

        if (leftNeighbour != null && leftNeighbour.isActive)
            nav.selectOnLeft = leftNeighbour.GetComponent<Button>();

        if (rightNeighbour != null && rightNeighbour.isActive)
            nav.selectOnRight = rightNeighbour.GetComponent<Button>();

        if (downNeighbour != null && downNeighbour.isActive)
            nav.selectOnDown = downNeighbour.GetComponent<Button>();

        if (upNeighbour != null && upNeighbour.isActive)
            nav.selectOnUp = upNeighbour.GetComponent<Button>();

        this.button.navigation = nav;
    }
    */

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
        Color baseC = buttonText.color;

        LeanTween.value(costText.gameObject, UpdateValueExampleCallback, baseC, Color.red, .3f)
        .setOnComplete(() =>
        {
            LeanTween.value(costText.gameObject, UpdateValueExampleCallback, Color.red, baseC, .3f)
            .setOnComplete(() => isTweening = false);
        });

        LeanTween.scale(costText.GetComponent<RectTransform>(), new Vector3(1.3f, 1.3f, 1.3f), .3f).setEase(LeanTweenType.easeShake)
        .setOnComplete(() =>
        {
            LeanTween.scale(costText.GetComponent<RectTransform>(), Vector3.one, .3f).setEase(LeanTweenType.easeShake);
        });
    }

    private void UpdateValueExampleCallback(Color val)
    {
        costText.color = val;
    }
}
