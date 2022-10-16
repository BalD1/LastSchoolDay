using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "DropTable", menuName = "Scriptable/DropTable")]
public class SCRPT_DropTable : ScriptableObject
{
    [SerializeField] private DropWithWeight[] dropTable;
    public DropWithWeight[] DropTable { get => dropTable; }

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

    public GameObject[] DropRandomAll(Vector2 position)
    {
        SCRPT_DropTable.DropWithWeight drop = GetRandomDrop();
        if (drop.objectToDrop != null)
        {
            GameObject[] table = new GameObject[drop.amount];
            for (int i = 0; i < drop.amount; i++)
            {
                table[i] = Instantiate(drop.objectToDrop, position, Quaternion.identity);
                table[i].GetComponent<Rigidbody2D>()?.AddForce(Vector2.up, ForceMode2D.Impulse);
            }

            return table;
        }

        return null;
    }
}
