using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private GameObject normalZombiePF;
    [SerializeField] private GameObject coinPF;
    [SerializeField] private GameObject pbThumbnailPF;
    [SerializeField] private GameObject keycardPF;

    [SerializeField] private GameObject smallCoinDropPF;
    [SerializeField] private GameObject largeCoinDropPF;

    [System.Serializable]
    public struct PBWithRarity
    {
        public E_PBRarity pbRarity;
        public GameObject[] pbsPF;
    }

    [SerializeField] private PBWithRarity[] pBWithRarities;

    [SerializeField] private List<GameObject> uniquePBs;

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
    public GameObject NormalZombiePF { get => normalZombiePF; }
    public GameObject CoinPF { get => coinPF; }
    public GameObject PBThumbnailPF { get => pbThumbnailPF; }
    public GameObject KeycardPF { get => keycardPF; }

    public GameObject SmallCoinDropPF { get => smallCoinDropPF; }
    public GameObject LargeCoinDropPF { get => largeCoinDropPF; }

    public PBWithRarity[] PBWithRarities { get => pBWithRarities; }
    public List<GameObject> UniquePBs { get => uniquePBs; }

    public Material OutlineMaterial { get => outlineMaterial; }
    public Material DefaultMaterial { get => defaultMaterial; }

    private void Awake()
    {
        instance = this;
    }
}
