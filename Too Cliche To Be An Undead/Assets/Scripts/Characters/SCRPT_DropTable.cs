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

    public int minDropAmount = 1;
    public int maxDropAmount = 1;

#if UNITY_EDITOR
    [HideInInspector] public float totalWeight;
    [HideInInspector] public int totalDrops;
#endif

    [System.Serializable]
    public struct DropWithWeight
    {
        public GameObject objectToDrop;
        public float weight;
        public int amount;
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

    public GameObject DropRandom(Vector2 position)
    {
        return DropRandomAll(position)[0];
    }

    public List<GameObject> DropRandomAll(Vector2 position)
    {
        int dropAmount = Random.Range(minDropAmount, maxDropAmount + 1);
        List<GameObject> table = new List<GameObject>();
        for (int i = 0; i < dropAmount; i++)
        {
            SCRPT_DropTable.DropWithWeight drop = GetRandomDrop();
            if (drop.objectToDrop != null)
            {
                for (int j = 0; j < drop.amount; j++)
                {
                    GameObject newDrop = Instantiate(drop.objectToDrop, position, Quaternion.identity);
                    newDrop.GetComponent<Rigidbody2D>()?.AddForce(Vector2.up, ForceMode2D.Impulse);
                    table.Add(newDrop);
                }
            }
        }

        foreach (var item in mandatoryDrops)
        {
            GameObject newDrop = Instantiate(item.objectToDrop, position, Quaternion.identity);
            table.Add(newDrop);
        }

        return table;
    }
}
