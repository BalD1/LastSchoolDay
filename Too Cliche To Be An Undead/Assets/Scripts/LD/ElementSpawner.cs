using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementSpawner : MonoBehaviour
{

    public enum E_ElementToSpawn
    {
        RandomBaseZombie,
        Keycard,
        Coins,
        DashingZombie,
        CaCZombie,
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
            case E_ElementToSpawn.RandomBaseZombie:
                Spawn(GameAssets.Instance.GetRandomZombie, GameManager.Instance.InstantiatedEntitiesParent);
                break;

            case E_ElementToSpawn.Keycard:
                Spawn(GameAssets.Instance.KeycardPF, GameManager.Instance.InstantiatedKeycardsParent);
                break;

            case E_ElementToSpawn.Coins:
                Spawn(GameAssets.Instance.CoinPF, GameManager.Instance.InstantiatedMiscParent);
                break;

            case E_ElementToSpawn.CaCZombie:
                Spawn(GameAssets.Instance.CacZombiePF, GameManager.Instance.InstantiatedEntitiesParent);
                break;

            case E_ElementToSpawn.DashingZombie:
                Spawn(GameAssets.Instance.DashingZombiePF, GameManager.Instance.InstantiatedEntitiesParent);
                break;

            default:
                Debug.LogError(elementToSpawn + " not found in switch statement.");
                break;
        }

        if (destroyAfterSpawn) Destroy(this.gameObject);
    }

    private void Spawn(GameObject objPF, Transform parent)
    {
        GameObject instantiated = Instantiate(objPF, this.transform.position, Quaternion.identity);
        instantiated.transform.parent = parent;
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (this.gameObject.CompareTag("ElementSpawner") == false) this.gameObject.tag = "ElementSpawner";
#endif
    }
}
