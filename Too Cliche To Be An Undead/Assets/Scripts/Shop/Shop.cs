using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject shop;

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
    }

    public void ExitedRange(GameObject interactor)
    {
    }

    public void Interact()
    {
        if (shopIsOpen) UIManager.Instance.CloseShop();
        else UIManager.Instance.OpenShop();

        shopIsOpen = !shopIsOpen;
    }

    public bool CanBeInteractedWith()
    {
        return true;
    }

    private void SetData()
    {
        if (DataKeeper.Instance.IsPlayerDataKeepSet() == false) return;

        int lvl = DataKeeper.Instance.playersDataKeep[0].maxLevel;
        for (int i = 0; i < lvl; i++)
        {
            levels[i].Unlock(GameManager.PlayerRef);
        }
    }
}
