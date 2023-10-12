using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : PersistentSingleton<InventoryManager>
{
    [field: SerializeField] public int MoneyAmount { get; private set; }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu))
            ForceSetMoney(0);
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
    }

    public void AddMoney(int amount)
    {
        MoneyAmount += amount;
        this.AddedMoney(amount);
        this.MoneyAmountChanged(MoneyAmount);
    }

    public bool TryBuy(int amount)
    {
        if (MoneyAmount < amount) return false;

        MoneyAmount -= amount;
        this.RemovedMoney(amount);
        this.MoneyAmountChanged(MoneyAmount);
        return true;
    }

    public void ForceSetMoney(int amount)
    {
        MoneyAmount = amount;
        this.MoneyAmountChanged(amount);
    }

    public bool HasEnoughMoney(int price) => MoneyAmount >= price ? true : false;
}