using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Upgrade Data", menuName = "Scriptable/Shop/UpgradeData")]
public class SO_ShopUpgrade : ScriptableObject
{
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public string UpgradeName { get; private set; }
    [field: SerializeField] public string UpgradeDescription { get; private set; }
    [field: SerializeField] public SO_StatModifierData[] Modifiers { get; private set; }

    [field: SerializeField] public S_UpgradeSprites LevelSprites { get; private set; }

    [field: SerializeField] public int RevivesToAdd { get; private set; }

    [System.Serializable] 
    public struct S_UpgradeSprites
    {
        public Sprite BoughedSprite;
        public Sprite UnlockedSprite;
        public Sprite LockedSprite;
    }

    [InspectorButton(nameof(SetDescription), ButtonWidth = 150)]
    [SerializeField] private bool setDescription;
    private void SetDescription()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var item in Modifiers)
        {
            sb.AppendFormat("{0} {1} to your {2} \n", item.Amount > 0 ? "Adds" : "Removes", Mathf.Abs(item.Amount), item.StatType.ToString());
        }

        int revives = RevivesToAdd;
        if (revives > 0)
            sb.AppendFormat(" +{0} revive{1} \n", revives, revives > 1 ? "s" : "");

        UpgradeDescription = sb.ToString();
    }
}