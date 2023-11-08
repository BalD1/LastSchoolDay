using System;
using UnityEngine;

public static class ShopEvents
{
    public static event Action<Shop> OnShopSetup;
    public static Shop CurrentShop { get; private set; }
    public static void SetCurrentShop(this  Shop shop)
    {
        CurrentShop = shop;
        OnShopSetup?.Invoke(shop);
    }

    public static event Action<Shop> OnShopDestroyed;
    public static void UnsetCurrentShop(this Shop shop)
    {
        CurrentShop = null;
        OnShopDestroyed?.Invoke(shop);
    }

    public static event Action OnOpenShop;
    public static void OpenedShop(this Shop shop)
        => OnOpenShop?.Invoke();

    public static event Action OnShopAnimEnded;
    public static void ShopAnimEnded(this Shop shop)
        => OnShopAnimEnded?.Invoke();

    public static event Action OnCloseShop;
    public static void ClosedShop(this Shop shop)
        => OnCloseShop?.Invoke();

    public static event Action<SO_ShopUpgrade> OnBoughtNewLevel;
    public static void BoughtNewLevel(this ShopLevel level, SO_ShopUpgrade data)
        => OnBoughtNewLevel?.Invoke(data);
}
