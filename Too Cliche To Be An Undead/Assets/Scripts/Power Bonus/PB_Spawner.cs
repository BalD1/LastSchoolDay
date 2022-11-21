using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PB_Spawner : MonoBehaviour
{
    [SerializeField] private GameAssets.E_PBRarity rarity;

    private void Start()
    {
        if (rarity == GameAssets.E_PBRarity.Unique)
        {
            if (GameAssets.Instance.UniquePBs.Count > 0)
            {
                int randIdx = Random.Range(0, GameAssets.Instance.UniquePBs.Count);
                Instantiate(GameAssets.Instance.UniquePBs[randIdx], this.transform.position, Quaternion.identity);
                GameAssets.Instance.UniquePBs.RemoveAt(randIdx);
            }
            Destroy(this.gameObject);
            return;
        }

        foreach (var item in GameAssets.Instance.PBWithRarities)
        {
            if (item.pbRarity == this.rarity)
            {
                SpawnRandomFromArray(item.pbsPF);
                Destroy(this.gameObject);
                return;
            }
        }
    }

    private void SpawnRandomFromArray(GameObject[] array)
    {
        Instantiate(array[Random.Range(0, array.Length)], this.transform.position, Quaternion.identity);
    }
}
