using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject shop;

    [SerializeField] private SkeletonAnimation skeleton;
    [SerializeField] private GameObject outline;

    [SerializeField] [SpineAnimation] private string idle, open;

    [SerializeField] private float animationDelay = .35f;

    private List<GameObject> currentInteractors = new List<GameObject>();

    private ShopLevel[] levels;

    private bool shopIsOpen = false;

    private void Start()
    {
        if (shop == null) shop = UIManager.Instance.ShopContentMenu;

        levels = new ShopLevel[shop.transform.childCount];

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] = shop.transform.GetChild(i).GetComponent<ShopLevel>();
            levels[i].SetShop(this);
        }

        SetData();
    }
    public void EnteredInRange(GameObject interactor)
    {
        outline.SetActive(true);
        currentInteractors.Add(interactor);
    }

    public void ExitedRange(GameObject interactor)
    {
        currentInteractors.Remove(interactor);
        if (currentInteractors.Count > 0) return;

        if (shopIsOpen) CloseShop();
        outline.SetActive(false);
    }

    public void Interact(GameObject interactor)
    {
        if (shopIsOpen) CloseShop();
        else OpenShop();

        outline.SetActive(false);
    }

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);

    public void UIInteract()
    {
        if (shopIsOpen) CloseShop();
        else OpenShop();

        outline.SetActive(false);
    }

    public void UpdateCostsMoney()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetPlayersMoney();
        }
    }

    public void OpenShop()
    {
        if (skeleton.AnimationName != idle) return;

        skeleton.AnimationState.SetAnimation(0, open, true);
        GameManager.Instance.GameState = GameManager.E_GameState.Restricted;
        UIManager.Instance.FadeAllHUD(fadeIn: false);

        LeanTween.delayedCall(animationDelay, DelayedOpen);
    }
    private void DelayedOpen()
    {
        UIManager.Instance.OpenShop(false);
        UIManager.Instance.D_closeMenu += CheckIfClosedMenuIsShop;
        shopIsOpen = true;
        GameManager.Player1Ref.SetAllVelocity(Vector2.zero);


        foreach (var item in levels)
        {
            item.SetPlayersMoney();
        }
    }

    public void CloseShop()
    {
        GameManager.Instance.GameState = GameManager.E_GameState.InGame;
        UIManager.Instance.CloseShop();
        UIManager.Instance.D_closeMenu -= CheckIfClosedMenuIsShop;
        shopIsOpen = false;

        LeanTween.delayedCall(animationDelay, DelayedIdle);
    }
    public void CloseShopFromUI()
    {
        UIManager.Instance.D_closeMenu -= CheckIfClosedMenuIsShop;
        shopIsOpen = false;

        LeanTween.delayedCall(animationDelay, DelayedIdle);
    }

    private void DelayedIdle()
    {
        skeleton.AnimationState.SetAnimation(0, idle, true);

        if (currentInteractors.Count > 0) outline.SetActive(true);
    }

    public void SetIsShopOpen(bool newState) => shopIsOpen = newState;

    private void CheckIfClosedMenuIsShop()
    {
        // If the shop menu is still open, do nothing
        foreach (var item in UIManager.Instance.OpenMenusQueues) if (item.Equals(UIManager.Instance.ShopMenu)) return;

        // else, remember that it is closed
        shopIsOpen = false;
        UIManager.Instance.D_closeMenu -= CheckIfClosedMenuIsShop;
    }

    public bool CanBeInteractedWith()
    {
        return true;
    }

    private void SetData()
    {
        if (DataKeeper.Instance.unlockedLevels == null || DataKeeper.Instance.unlockedLevels.Count <= 0) return;

        foreach (var item in levels)
        {
            if (DataKeeper.Instance.unlockedLevels.Contains(item.ID)) item.Unlock(reloadUnlock: true);
        }
    }
}
