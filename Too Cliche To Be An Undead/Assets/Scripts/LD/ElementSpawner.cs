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

    [SerializeField] private bool wasInstantiatedInRoom = false;

    public bool DestroyAfterSpawn { get => destroyAfterSpawn; }
    public bool SpawnAtStart { get => spawnAtStart; }

    public void Setup(E_ElementToSpawn _elementToSpawn, bool _destroyAfterSpawn, bool _spawnAtStart)
    {
        elementToSpawn = _elementToSpawn;
        destroyAfterSpawn = _destroyAfterSpawn;
        spawnAtStart = _spawnAtStart;
    }

    private void Start()
    {
        if (wasInstantiatedInRoom) SpawnersManager.Instance.AddSingleToArray(this.gameObject);

        if (spawnAtStart) SpawnElement();
    }

    public void SpawnElement()
    {
        switch (elementToSpawn)
        {
            case E_ElementToSpawn.Zombie:
                GameObject zom = Instantiate(GameAssets.Instance.NormalZombiePF, this.transform.position, Quaternion.identity);
                zom.transform.parent = GameManager.Instance.InstantiatedEntitiesParent;
                break;

            case E_ElementToSpawn.Keycard:
                GameObject key = Instantiate(GameAssets.Instance.KeycardPF, this.transform.position, Quaternion.identity);
                key.transform.parent = GameManager.Instance.InstantiatedKeycardsParent;
                break;

            case E_ElementToSpawn.Coins:
                GameObject cns = Instantiate(GameAssets.Instance.CoinPF, this.transform.position, Quaternion.identity);
                cns.transform.parent = GameManager.Instance.InstantiatedMiscParent;
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
#endif
    }
}
