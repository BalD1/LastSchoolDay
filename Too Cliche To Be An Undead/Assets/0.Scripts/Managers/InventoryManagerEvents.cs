using System;

public static class InventoryManagerEvents
{
	public static event Action<int> OnAddedMoney;
	public static void AddedMoney(this InventoryManager manager, int amount)
		=> OnAddedMoney?.Invoke(amount);

	public static event Action<int> OnRemovedMoney;
	public static void RemovedMoney(this InventoryManager manager, int amount)
		=> OnRemovedMoney?.Invoke(amount);

	public static event Action<int> OnMoneyChange;

    public static void MoneyAmountChanged(this InventoryManager manager, int newAmount)
		=> OnMoneyChange?.Invoke(newAmount);
}