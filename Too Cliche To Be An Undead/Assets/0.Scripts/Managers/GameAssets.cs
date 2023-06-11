using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[ExecuteInEditMode]
public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;
    public static GameAssets Instance
    {
        get
        {
            if (instance == null) Debug.LogError("Could not find GameAssets instance.");

            return instance;
        }
    }

    [SerializeField] private GameObject playerPF;
    [SerializeField] private GameObject damagesPopupPF;
    [SerializeField] private GameObject textPopupPF;
    [SerializeField] private GameObject trainingDummyPF;
    [SerializeField] private GameObject coinPF;
    [SerializeField] private GameObject pbThumbnailPF;
    [SerializeField] private GameObject keycardPF;
    [SerializeField] private GameObject audioPlayerPF;

    [SerializeField] private GameObject smallCoinDropPF;
    [SerializeField] private GameObject largeCoinDropPF;

    [field: SerializeField] public GameObject[] GroundedZombiesPF { get; private set; }

    [field: SerializeField] public AudioMixerGroup MixerGroup_SFX { get; private set; }

    [field: SerializeField] public GameObject BaseDestructionParticlesPF { get; private set; }
    [field: SerializeField] public GameObject BasePropDamagesParticlesPF { get; private set; }

    [field: SerializeField] public GameObject BloodParticlesPF { get; private set; }
    [field: SerializeField] public GameObject DashHitParticlesPF { get; private set; }
    [field: SerializeField] public GameObject BossHitFX { get; private set; }

    [field: SerializeField] public GameObject BaseProjectilePF { get; private set; }

    [field: SerializeField] public GameObject AudioclipPlayerPF { get; private set; }

    [SerializeField] private GameObject basePBPF;

    [SerializeField] private S_ZombiesWithWeight[] zombiesWithWeights;
    [System.Serializable] public struct S_ZombiesWithWeight
    {
        public float weight;
        public GameObject zombiePF;
    }
    [field: SerializeField] public float zombiesTotalWeight { get; private set; }

    [field: SerializeField] public SCRPT_TextPopupComponents textComponents { get; private set; }
    public static SCRPT_TextPopupComponents.HitComponents BaseComponents { get => GameAssets.Instance.textComponents.baseComponents; }
    public static SCRPT_TextPopupComponents.HitComponents StunComponents { get => GameAssets.Instance.textComponents.StunComponents; }
    public static SCRPT_TextPopupComponents.HitComponents ItemComponents { get => GameAssets.Instance.textComponents.ItemPickupComponents; }

    [System.Serializable]
    public struct PBWithRarity
    {
        public E_PBRarity pbRarity;
        public SCRPT_PB[] pbsPF;
    }

    [SerializeField] private PBWithRarity[] pBWithRarities;

    [SerializeField] private List<SCRPT_PB> uniquePBs;

    public enum E_PBRarity
    {
        Common,
        Uncommon,
        Rare,
        Unique,
    }

    public enum E_CoinType
    {
        Small,
        Large,
    }

    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Material defaultMaterial;

    public GameObject PlayerPF { get => playerPF; }
    public GameObject DamagesPopupPF { get => damagesPopupPF; }
    public GameObject TextPopupPF { get => textPopupPF; }
    public GameObject TrainingDummyPF { get => trainingDummyPF; }

    public GameObject GetRandomZombie
    {
        get
        {
            float randomValue = Random.Range(0, zombiesTotalWeight);
            float currentWeight = 0;
            foreach (S_ZombiesWithWeight current in zombiesWithWeights)
            {
                currentWeight += current.weight;
                if (currentWeight >= randomValue)
                {
                    return current.zombiePF;
                }
            }

            return zombiesWithWeights[0].zombiePF;
        }
    }

    public GameObject CoinPF { get => coinPF; }
    public GameObject PBThumbnailPF { get => pbThumbnailPF; }
    public GameObject KeycardPF { get => keycardPF; }

    public GameObject SmallCoinDropPF { get => smallCoinDropPF; }
    public GameObject LargeCoinDropPF { get => largeCoinDropPF; }

    public GameObject BasePBPF { get => basePBPF; }
    public PBWithRarity[] PBWithRarities { get => pBWithRarities; }
    public List<SCRPT_PB> UniquePBs { get => uniquePBs; }

    public GameObject AudioPlayerPF { get => audioPlayerPF; }

    public Material OutlineMaterial { get => outlineMaterial; }
    public Material DefaultMaterial { get => defaultMaterial; }

    private void Awake()
    {
        instance = this;

        zombiesTotalWeight = 0;
        foreach (S_ZombiesWithWeight zombie in zombiesWithWeights)
        {
            zombiesTotalWeight += zombie.weight;
        }
    }
}
