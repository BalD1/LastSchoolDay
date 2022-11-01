using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopLevel : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;

    [SerializeField] private ShopLevel[] neighbours;

    [SerializeField] private int id;
    [SerializeField] private int cost;

    [SerializeField] private Modifier[] modifiers;

    [SerializeField] private bool isActive = false;
    private bool isUnlocked = false;

    [SerializeField] private Color unlockedColor;

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
        button.interactable = (isActive && !isUnlocked);

        buttonText.text = $"Level {id} \n";
        foreach (var item in modifiers)
        {
            buttonText.text += $"+{item.amount}% {item.stat} \n";
        }
        buttonText.text += $"Cost : {cost}";
    }

    public void SetActive(bool active)
    {
        isActive = active;
        button.interactable = isActive;
    }

    public bool TryUnlock(PlayerCharacter player)
    {
        if (!player.HasEnoughMoney(cost)) return false;

        player.RemoveMoney(cost, false);
        Unlock(player);

        return true;
    }

    public void Unlock(PlayerCharacter player)
    {
        if (modifiers != null)
            foreach (var item in modifiers) player.AddModifier(item.idName, item.amount, item.stat);

        if (neighbours != null)
            foreach (var item in neighbours) item.SetActive(true);

        isUnlocked = true;
        this.button.interactable = false;
        this.button.image.color = unlockedColor;

        player.LevelUp();
    }

    public void OnClick()
    {
        TryUnlock(GameManager.Player1Ref);
    }
}
