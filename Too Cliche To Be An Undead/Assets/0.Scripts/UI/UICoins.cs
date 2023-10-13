using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICoins : MonoBehaviourEventsHandler
{
    [SerializeField] private ScaleTween coinsTween;
    [SerializeField] private TextMeshProUGUI coinsTMP;
    [SerializeField] private bool tweenOnCoinsUpdate;

    protected override void EventsSubscriber()
    {
        InventoryManagerEvents.OnMoneyChange += OnMoneyAmountChanged;
    }

    protected override void EventsUnSubscriber()
    {
        InventoryManagerEvents.OnMoneyChange -= OnMoneyAmountChanged;
    }

    public void UpdateUI()
        => UpdateUI(InventoryManager.Instance.MoneyAmount);
    public void UpdateUI(int newAmount)
    {
        coinsTMP.text = "x" + newAmount;
    }

    private void OnMoneyAmountChanged(int newAmount)
    {
        UpdateUI(newAmount);
        if (tweenOnCoinsUpdate)
            coinsTween.TryPlay();
    }
}
