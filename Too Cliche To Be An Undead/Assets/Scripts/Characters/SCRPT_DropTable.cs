using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "DropTable", menuName = "Scriptable/DropTable")]
public class SCRPT_DropTable : ScriptableObject
{
    [SerializeField] private DropWithWeight[] dropTable;
    public DropWithWeight[] DropTable { get => dropTable; }

    [SerializeField] private MandatoryDrop[] mandatoryDrops;
    public MandatoryDrop[] MandatoryDrops { get => mandatoryDrops; }

#if UNITY_EDITOR
    [HideInInspector] public float totalWeight;
    [HideInInspector] public int totalDrops;
#endif

    [System.Serializable]
    public struct DropWithWeight
    {
#if UNITY_EDITOR
        public string editorName;
#endif
        public ObjectWithAmount[] objectsToDrop;
        public float weight;
    }
    [System.Serializable]
    public struct ObjectWithAmount
    {
#if UNITY_EDITOR
        public string editorName;
#endif
        public GameObject objectToDrop;
        [Range(0, 50)] public int minAmount;
        [Range(0, 50)] public int maxAmount;
    }

    [System.Serializable]
    public struct MandatoryDrop
    {
        public GameObject objectToDrop;
        public int amount;
    }

    public DropWithWeight GetRandomDrop()
    {
        float totalWeight = 0;
        foreach (DropWithWeight drop in dropTable)
        {
            totalWeight += drop.weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float currentWeight = 0;
        foreach (DropWithWeight d in dropTable)
        {
            currentWeight += d.weight;
            if (currentWeight >= randomValue)
            {
                return d;
            }
        }

        return new DropWithWeight();
    }

    public List<GameObject> DropRandom(Vector2 position)
    {
        List<GameObject> table = new List<GameObject>();
        SCRPT_DropTable.DropWithWeight drop = GetRandomDrop();
        if (drop.objectsToDrop.Length > 0)
        {
            foreach (var item in drop.objectsToDrop)
            {
                int amount = Random.Range(item.minAmount, item.maxAmount + 1);
                for (int i = 0; i < amount; i++)
                {
                    GameObject newDrop = Instantiate(item.objectToDrop, position, Quaternion.identity);
                    newDrop.GetComponent<Rigidbody2D>()?.AddForce(Vector2.up, ForceMode2D.Impulse);
                    table.Add(newDrop);
                }
            }
        }

        //Debug.Log($"Dropping {drop.editorName}");

        foreach (var item in mandatoryDrops)
        {
            GameObject newDrop = Instantiate(item.objectToDrop, position, Quaternion.identity);
            table.Add(newDrop);
        }

        return table;
    }
}
