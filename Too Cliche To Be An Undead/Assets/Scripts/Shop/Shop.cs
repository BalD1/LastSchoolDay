using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject shop;

    [SerializeField] private SpriteRenderer spriteRenderer;

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
        }

        SetData();
    }
    public void EnteredInRange(GameObject interactor)
    {
        spriteRenderer.material = GameAssets.Instance.OutlineMaterial;
        currentInteractors.Add(interactor);
    }

    public void ExitedRange(GameObject interactor)
    {
        currentInteractors.Remove(interactor);
        if (currentInteractors.Count > 0) return;

        if (shopIsOpen) CloseShop();
        spriteRenderer.material = GameAssets.Instance.DefaultMaterial;
    }

    public void Interact(GameObject interactor)
    {
        if (shopIsOpen) CloseShop();
        else OpenShop();

        if (!CanBeInteractedWith()) spriteRenderer.material = GameAssets.Instance.DefaultMaterial;
    }

    public void UIInteract()
    {
        if (shopIsOpen) CloseShop();
        else OpenShop();

        if (!CanBeInteractedWith()) spriteRenderer.material = GameAssets.Instance.DefaultMaterial;
    }

    private void OpenShop()
    {
        GameManager.Instance.GameState = GameManager.E_GameState.Restricted;
        UIManager.Instance.OpenShop();
        UIManager.Instance.D_closeMenu += CheckIfClosedMenuIsShop;
        shopIsOpen = true;
        GameManager.Player1Ref.SetAllVelocity(Vector2.zero);
    }

    private void CloseShop()
    {
        GameManager.Instance.GameState = GameManager.E_GameState.InGame;
        UIManager.Instance.CloseShop();
        UIManager.Instance.D_closeMenu -= CheckIfClosedMenuIsShop;
        shopIsOpen = false;
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
