using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnersManager : MonoBehaviour
{
    private static SpawnersManager instance;
    public static SpawnersManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] [Range(0, 10)] private int maxKeycardsToSpawn = 5;
    [SerializeField] [Range(0, 10)] private int minKeycardsToSpawn = 3;

    [SerializeField] private List<ElementSpawner> elementSpawners = new List<ElementSpawner>();
    [SerializeField] private List<ElementSpawner> keycardSpawners = new List<ElementSpawner>();

    [SerializeField] private GameObject spawner_PF;
    public GameObject Spawner_PF { get => spawner_PF; }

    private void Awake()
    {
        instance = this;
    }

    public void ForceSpawnAll()
    {
        foreach (var item in elementSpawners)
        {
            item.SpawnElement();
        }
    }

    public void ManageKeycardSpawn()
    {
        int keysToSpawn = Random.Range(minKeycardsToSpawn, maxKeycardsToSpawn + 1);

        SpawnSingleCard(keysToSpawn);

        foreach (var item in keycardSpawners)
        {
            Destroy(item.gameObject);
        }
        keycardSpawners.Clear();

        UIManager.Instance.UpdateKeycardsCounter();
    }

    private void SpawnSingleCard(int remainingSpawns)
    {
        if (remainingSpawns <= 0) return;

        int randomSpawner = Random.Range(0, keycardSpawners.Count);

        if (randomSpawner >= keycardSpawners.Count) return;

        keycardSpawners[randomSpawner].SpawnElement();
        GameManager.NeededCards += 1;

        Destroy(keycardSpawners[randomSpawner].gameObject);

        keycardSpawners.RemoveAt(randomSpawner);

        SpawnSingleCard(remainingSpawns - 1);
    }

    public void SetupArray(GameObject[] objectsArray)
    {
        keycardSpawners = new List<ElementSpawner>();
        elementSpawners = new List<ElementSpawner>();
        for (int i = 0; i < objectsArray.Length; i++)
        {
            AddSingleToArray(objectsArray[i], i);
        }
    }

    public void AddSingleToArray(GameObject element, int idx = -1)
    {
        if (idx == -1)
        {
            elementSpawners.Add(null);
            idx = elementSpawners.Count - 1;
        }
        elementSpawners[idx] = element.GetComponent<ElementSpawner>();

#if UNITY_EDITOR
        CreateObjectName(element, elementSpawners[idx]);
#endif

        if (elementSpawners[idx].ElementToSpawn == ElementSpawner.E_ElementToSpawn.Keycard)
            keycardSpawners.Add(elementSpawners[idx]);
    }

    private void CreateObjectName(GameObject gO, ElementSpawner es)
    {
#if UNITY_EDITOR
        StringBuilder objectName = new StringBuilder("SPAWNER");
        switch (es.ElementToSpawn)
        {
            case ElementSpawner.E_ElementToSpawn.Zombie:
                objectName.Append("_ZOM");
                break;

            case ElementSpawner.E_ElementToSpawn.Keycard:
                objectName.Append("_KEY");
                break;

            case ElementSpawner.E_ElementToSpawn.Coins:
                objectName.Append("_COIN");
                break;

            default:
                objectName.Append("_UNDEFINED");
                break;
        }

        objectName.Append(es.DestroyAfterSpawn ? "_DESTR" : "");
        objectName.Append(es.SpawnAtStart ? "_START" : "");

        gO.name = objectName.ToString();
#endif
    }

    private void OnEnable()
    {
        if (minKeycardsToSpawn > maxKeycardsToSpawn) minKeycardsToSpawn = maxKeycardsToSpawn;
    }
}
