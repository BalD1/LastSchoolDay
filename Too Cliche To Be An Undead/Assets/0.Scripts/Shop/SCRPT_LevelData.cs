using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable/LevelData")]
public class SCRPT_LevelData : ScriptableObject
{
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public string LevelName { get; private set; }
    [field: SerializeField] public string LevelDescription { get; private set; }
    [field: SerializeField] public Modifier[] modifiers { get; private set; }

    [field: SerializeField] public S_LevelSprites LevelSprites { get; private set; }

    [field: SerializeField] public int revivesToAdd { get; private set; }

    [System.Serializable] 
    public struct S_LevelSprites
    {
        public Sprite BoughedSprite;
        public Sprite UnlockedSprite;
        public Sprite LockedSprite;
    }

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

    [InspectorButton(nameof(SetDescription), ButtonWidth = 150)]
    [SerializeField] private bool setDescription;
    private void SetDescription()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var item in modifiers)
        {
            sb.AppendFormat("{0} {1} to your {2} \n", item.amount > 0 ? "Adds" : "Removes", Mathf.Abs(item.amount), StatsModifier.TypeToString_UI(item.stat));
        }

        int revives = revivesToAdd;
        if (revives > 0)
            sb.AppendFormat(" +{0} revive{1} \n", revives, revives > 1 ? "s" : "");

        LevelDescription = sb.ToString();
    }
}