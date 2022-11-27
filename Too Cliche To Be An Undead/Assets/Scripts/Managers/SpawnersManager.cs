using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    [SerializeField] private ElementSpawner[] elementSpawners;
    private List<ElementSpawner> keycardSpawners = new List<ElementSpawner>();

    [SerializeField] private GameObject spawner_PF;
    public GameObject Spawner_PF { get => spawner_PF; }

    private void Start()
    {
        ManageKeycardSpawn();
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

        for (int i = 0; i < keysToSpawn; i++)
        {
            int randomSpawner = Random.Range(0, keycardSpawners.Count);
            keycardSpawners[randomSpawner].SpawnElement();

#if UNITY_EDITOR == false
            Destroy(keycardSpawners[randomSpawner].gameObject); 
#endif

            keycardSpawners.RemoveAt(randomSpawner);
        }

        foreach (var item in keycardSpawners)
        {
#if UNITY_EDITOR == false
            Destroy(item.gameObject);
#endif
        }
        keycardSpawners.Clear();
    }

    public void SetupArray(GameObject[] objectsArray)
    {
        keycardSpawners = new List<ElementSpawner>();
        elementSpawners = new ElementSpawner[objectsArray.Length];
        for (int i = 0; i < objectsArray.Length; i++)
        {
            elementSpawners[i] = objectsArray[i].GetComponent<ElementSpawner>();

#if UNITY_EDITOR
            CreateObjectName(objectsArray[i], elementSpawners[i]);
            elementSpawners[i].isSetup = true;
#endif

            if (elementSpawners[i].ElementToSpawn == ElementSpawner.E_ElementToSpawn.Keycard)
                keycardSpawners.Add(elementSpawners[i]);
        }
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
