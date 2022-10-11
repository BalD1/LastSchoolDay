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

    [SerializeField] private GameObject damagesPopupPF;

    public GameObject DamagesPopupPF { get => damagesPopupPF; }

    private void Awake()
    {
        instance = this;
    }
}
