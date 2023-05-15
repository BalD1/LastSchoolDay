using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

                PB_Drop drop = Instantiate(GameAssets.Instance.BasePBPF, this.transform.position, Quaternion.identity).GetComponent<PB_Drop>();
                drop.Setup(GameAssets.Instance.UniquePBs[randIdx]);

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

    private void SpawnRandomFromArray(SCRPT_PB[] array)
    {
        SCRPT_PB pbToSpawn = array[Random.Range(0, array.Length)];

        PB_Drop drop = Instantiate(GameAssets.Instance.BasePBPF, this.transform.position, Quaternion.identity).GetComponent<PB_Drop>();
        drop.Setup(pbToSpawn);
    }
}
