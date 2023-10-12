using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICoins : MonoBehaviourEventsHandler
{
    [SerializeField] private ScaleTween coinsTween;
    [SerializeField] private TextMeshProUGUI coinsTMP;

    protected override void EventsSubscriber()
    {
        InventoryManagerEvents.OnMoneyChange += OnMoneyAmountChanged;
    }

    protected override void EventsUnSubscriber()
    {
        InventoryManagerEvents.OnMoneyChange -= OnMoneyAmountChanged;
    }

    private void OnMoneyAmountChanged(int newAmount)
    {
        coinsTMP.text = "x" + newAmount;
        coinsTween.TryPlay();
    }
}
