using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private GameObject dashingZombiePF;
    [SerializeField] private GameObject cacZombiePF;
    [SerializeField] private GameObject coinPF;
    [SerializeField] private GameObject pbThumbnailPF;
    [SerializeField] private GameObject keycardPF;

    [SerializeField] private GameObject smallCoinDropPF;
    [SerializeField] private GameObject largeCoinDropPF;

    [SerializeField] private GameObject basePBPF;

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
    public GameObject DashingZombiePF { get => dashingZombiePF; }
    public GameObject CacZombiePF { get => cacZombiePF; }

    public GameObject GetRandomZombie { get => Random.Range(0, 2) == 0 ? DashingZombiePF : CacZombiePF; }

    public GameObject CoinPF { get => coinPF; }
    public GameObject PBThumbnailPF { get => pbThumbnailPF; }
    public GameObject KeycardPF { get => keycardPF; }

    public GameObject SmallCoinDropPF { get => smallCoinDropPF; }
    public GameObject LargeCoinDropPF { get => largeCoinDropPF; }

    public GameObject BasePBPF { get => basePBPF; }
    public PBWithRarity[] PBWithRarities { get => pBWithRarities; }
    public List<SCRPT_PB> UniquePBs { get => uniquePBs; }

    public Material OutlineMaterial { get => outlineMaterial; }
    public Material DefaultMaterial { get => defaultMaterial; }

    private void Awake()
    {
        instance = this;
    }
}
