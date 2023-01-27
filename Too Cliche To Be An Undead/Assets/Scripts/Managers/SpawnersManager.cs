using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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

    private Queue<GameObject> zombiesPool = new Queue<GameObject>();
    public Queue<GameObject> ZombiesPool { get => zombiesPool; }

    [field: SerializeField] public AnimationCurve maxZombiesInSchoolByTime;

    [ReadOnly]
    [SerializeField] private int spawnStamp;

    [ReadOnly]
    [SerializeField] private int maxStamp;

    [ReadOnly]
    [SerializeField] private int currentZombiesInSchool;
    public int CurrentZombiesInSchool { get => currentZombiesInSchool; }

    public const int minValidDistanceFromPlayer = 5;
    public const int maxValidDistanceFromPlayer = 15;

    [SerializeField] private List<ElementSpawner> elementSpawners = new List<ElementSpawner>();
    [SerializeField] private List<ElementSpawner> keycardSpawners = new List<ElementSpawner>();

    public List<ElementSpawner> ElementSpawners { get => elementSpawners; }
    public List<ElementSpawner> KeycardSpawners { get => keycardSpawners; }

    [SerializeField] private AreaSpawner[] areaSpawners;
    public AreaSpawner[] AreaSpawners { get => areaSpawners; }

    public List<AreaSpawner> validAreaSpawners = new List<AreaSpawner>();

    [SerializeField] private GameObject spawner_PF;
    public GameObject Spawner_PF { get => spawner_PF; }

#if UNITY_EDITOR
    [HideInInspector] public bool showAreaSpawnersBounds = false; 
#endif

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        return;

        foreach (var item in areaSpawners)
        {
            item.SpawnObject(3);
        }
    }

    private void Update()
    {
        TrySpawnZombies();
    }

    private void TrySpawnZombies()
    {
        if (currentZombiesInSchool < maxZombiesInSchoolByTime.Evaluate(0)) SpawnNext();
    }

    private void SpawnNext()
    {
        if (validAreaSpawners.Count <= 0) return;

        int zombiesToSpawn = (int)maxZombiesInSchoolByTime.Evaluate(0) - currentZombiesInSchool;

        int zombiesByArea = zombiesToSpawn / validAreaSpawners.Count;

        foreach (var item in validAreaSpawners)
        {
            item.SpawnObject(zombiesByArea);
        }
    }

    public void AddZombie()
    {
        if (GameManager.Instance.IsInTutorial) return;
        currentZombiesInSchool++;
    }
    public void RemoveZombie()
    {
        if (GameManager.Instance.IsInTutorial) return;
        currentZombiesInSchool--;
    }

    public void ForceSpawnAll()
    {
        foreach (var item in elementSpawners) item.SpawnElement();
    }

    public void ManageKeycardSpawn()
    {
        int keysToSpawn = Random.Range(minKeycardsToSpawn, maxKeycardsToSpawn + 1);

        SpawnSingleCard(keysToSpawn);

        foreach (var item in keycardSpawners) Destroy(item.gameObject);

        keycardSpawners.Clear();

        UIManager.Instance.UpdateKeycardsCounter();
    }

    private void SpawnSingleCard(int remainingSpawns)
    {
        if (remainingSpawns <= 0) return;

        int randomSpawner = Random.Range(0, keycardSpawners.Count - 1);

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
    public void SetupAreaSpawners(GameObject[] spawners)
    {
        areaSpawners = new AreaSpawner[spawners.Length];

        for (int i = 0; i < spawners.Length; i++)
        {
            areaSpawners[i] = spawners[i].GetComponent<AreaSpawner>();
        }
    }

    public void AddSingleToArray(GameObject element, int idx = -1)
    {
        if (idx == -1)
        {
            elementSpawners.Add(null);
            idx = elementSpawners.Count - 1;
        }

        if (idx >= elementSpawners.Count) elementSpawners.Add(element.GetComponent<ElementSpawner>());
        else elementSpawners[idx] = element.GetComponent<ElementSpawner>();

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
            case ElementSpawner.E_ElementToSpawn.RandomBaseZombie:
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
