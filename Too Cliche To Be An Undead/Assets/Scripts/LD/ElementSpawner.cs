using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementSpawner : MonoBehaviour
{

    public enum E_ElementToSpawn
    {
        Zombie,
        Keycard,
        Coins,
    }

    [SerializeField] private E_ElementToSpawn elementToSpawn;
    public E_ElementToSpawn ElementToSpawn { get => elementToSpawn; }

    [SerializeField] private bool destroyAfterSpawn;
    [SerializeField] private bool spawnAtStart = true;

    public bool DestroyAfterSpawn { get => destroyAfterSpawn; }
    public bool SpawnAtStart { get => spawnAtStart; }

#if UNITY_EDITOR
    public bool isSetup; 
#endif

    public void Setup(E_ElementToSpawn _elementToSpawn, bool _destroyAfterSpawn, bool _spawnAtStart)
    {
        elementToSpawn = _elementToSpawn;
        destroyAfterSpawn = _destroyAfterSpawn;
        spawnAtStart = _spawnAtStart;
    }

    private void Start()
    {
        if (spawnAtStart) SpawnElement();
    }

    public void SpawnElement()
    {
        switch (elementToSpawn)
        {
            case E_ElementToSpawn.Zombie:
                Instantiate(GameAssets.Instance.NormalZombiePF, this.transform.position, Quaternion.identity);
                break;

            case E_ElementToSpawn.Keycard:
                break;

            case E_ElementToSpawn.Coins:
                Instantiate(GameAssets.Instance.CoinPF, this.transform.position, Quaternion.identity);
                break;

            default:
                Debug.LogError(elementToSpawn + " not found in switch statement.");
                break;
        }

        if (destroyAfterSpawn) Destroy(this.gameObject);
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (this.gameObject.CompareTag("ElementSpawner") == false) this.gameObject.tag = "ElementSpawner";

        if (!isSetup) Debug.Log("Please setup array in SpawnersManager");
#endif
    }
}
