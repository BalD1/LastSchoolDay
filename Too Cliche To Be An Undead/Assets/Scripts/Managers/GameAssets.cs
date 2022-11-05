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
    [SerializeField] private GameObject coinPF;
    [SerializeField] private GameObject pbThumbnailPF;

    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Material defaultMaterial;

    public GameObject PlayerPF { get => playerPF; }
    public GameObject DamagesPopupPF { get => damagesPopupPF; }
    public GameObject TextPopupPF { get => textPopupPF; }
    public GameObject TrainingDummyPF { get => trainingDummyPF; }
    public GameObject CoinPF { get => coinPF; }
    public GameObject PBThumbnailPF { get => pbThumbnailPF; }

    public Material OutlineMaterial { get => outlineMaterial; }
    public Material DefaultMaterial { get => defaultMaterial; }

    private void Awake()
    {
        instance = this;
    }
}
