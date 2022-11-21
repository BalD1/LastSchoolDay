using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "DropTable", menuName = "Scriptable/DropTable")]
public class SCRPT_DropTable : ScriptableObject
{
    [SerializeField] private DropWithWeight[] dropTable;
    public DropWithWeight[] DropTable { get => dropTable; }

    [SerializeField] private CoinsDropWithWeight[] bonusCoins;
    public CoinsDropWithWeight[] BonusCoins { get => bonusCoins; }

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
    public struct CoinsDropWithWeight
    {
#if UNITY_EDITOR
        public string editorName;
#endif
        public CoinTypeWithAmount[] coinTypeWithAmounts;
        public float weight;
    }
    [System.Serializable]
    public struct CoinTypeWithAmount
    {
#if UNITY_EDITOR
        public string editorName;
#endif
        public GameAssets.E_CoinType coinType;
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
    public DropWithWeight GetRandomDrop(DropWithWeight[] table)
    {
        float totalWeight = 0;
        foreach (DropWithWeight drop in table)
        {
            totalWeight += drop.weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float currentWeight = 0;
        foreach (DropWithWeight d in table)
        {
            currentWeight += d.weight;
            if (currentWeight >= randomValue)
            {
                return d;
            }
        }

        return new DropWithWeight();
    }
    public CoinsDropWithWeight GetRandomDrop(CoinsDropWithWeight[] table)
    {
        float totalWeight = 0;
        foreach (CoinsDropWithWeight drop in table)
        {
            totalWeight += drop.weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float currentWeight = 0;
        foreach (CoinsDropWithWeight d in table)
        {
            currentWeight += d.weight;
            if (currentWeight >= randomValue)
            {
                return d;
            }
        }

        return new CoinsDropWithWeight();
    }

    public List<GameObject> DropRandom(Vector2 position)
    {
        List<GameObject> table = new List<GameObject>();

        SCRPT_DropTable.DropWithWeight drop = GetRandomDrop();
            AddDropsToTableAndSpawn(ref table, drop, position);

        SCRPT_DropTable.CoinsDropWithWeight coins = GetRandomDrop(bonusCoins);
        AddDropsToTableAndSpawn(ref table, coins, position);

        foreach (var item in mandatoryDrops)
        {
            GameObject newDrop = Instantiate(item.objectToDrop, position, Quaternion.identity);
            table.Add(newDrop);
        }

        return table;
    }

    private void AddDropsToTableAndSpawn(ref List<GameObject> table, DropWithWeight drops, Vector2 pos)
    {
        if (drops.objectsToDrop?.Length > 0)
        {
            foreach (var item in drops.objectsToDrop)
            {
                int amount = Random.Range(item.minAmount, item.maxAmount + 1);
                for (int i = 0; i < amount; i++)
                {
                    GameObject newDrop = Instantiate(item.objectToDrop, pos, Quaternion.identity);
                    table.Add(newDrop);
                }
            }
        }
    }
    private void AddDropsToTableAndSpawn(ref List<GameObject> table, CoinsDropWithWeight drops, Vector2 pos)
    {
        if (drops.coinTypeWithAmounts?.Length > 0)
        {
            GameObject coin = null;
            foreach (var item in drops.coinTypeWithAmounts)
            {
                int amount = Random.Range(item.minAmount, item.maxAmount + 1);

                switch (item.coinType)
                {
                    case GameAssets.E_CoinType.Small:
                        coin = GameAssets.Instance.SmallCoinDropPF;
                        break;

                    case GameAssets.E_CoinType.Large:
                        coin = GameAssets.Instance.LargeCoinDropPF;
                        break;

                    default:
                        Debug.LogError(item.coinType + " not found in switch statement.");
                        break;
                }

                if (coin == null) continue;
                for (int i = 0; i < amount; i++)
                {
                    GameObject newDrop = Instantiate(coin, pos, Quaternion.identity);
                    table.Add(newDrop);
                }
            }
        }
    }
}
