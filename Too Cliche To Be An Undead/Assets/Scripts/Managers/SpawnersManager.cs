using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    private Queue<NormalZombie> zombiesPool = new Queue<NormalZombie>();
    public Queue<NormalZombie> ZombiesPool { get => zombiesPool; }

    private Queue<NormalZombie> zombiesToTeleport = new Queue<NormalZombie>();
    public Queue<NormalZombie> ZombiesToTeleport { get => zombiesToTeleport; }

    [SerializeField] private CanvasGroup stampsCounter;
    [SerializeField] private Image uiFiller;
    [SerializeField] private TextMeshProUGUI uiCounter;

    [field: SerializeField] public AnimationCurve maxZombiesInSchoolByTime { get; private set; }
    [field: SerializeField] public AnimationCurve zombiesSpawnCooldown { get; private set; }
    [field: SerializeField] public AnimationCurve spawnsBreakupsDurations { get; private set; }
    [field: SerializeField] public AnimationCurve spawnsBreakupsCooldowns { get; private set; }

    [ReadOnly]
    [SerializeField] private float currentBreakup_TIMER;

    [ReadOnly]
    [SerializeField] private float timerBeforeNextBreakup = 10;

    [ReadOnly]
    [SerializeField] private bool isInBreakup;

    [ReadOnly]
    [SerializeField] private int spawnStamp;

    [SerializeField] private int maxStamp;

    [ReadOnly]
    [SerializeField] private int currentZombiesInSchool;

    [ReadOnly]
    [SerializeField] private int maxZombiesInSchool;

    [ReadOnly]
    [SerializeField] private float spawnCooldown;

    public int CurrentZombiesInSchool { get => currentZombiesInSchool; }

    public const int minValidDistanceFromPlayer = 5;

    public const float timeBetweenStamps = 60;

    [SerializeField] private bool spawnsAreAllowed = false;

    [ReadOnly]
    [SerializeField] private float stamp_TIMER;

    [SerializeField] private List<ElementSpawner> elementSpawners = new List<ElementSpawner>();
    [SerializeField] private List<ElementSpawner> keycardSpawners = new List<ElementSpawner>();

    public int SpawnStamp { get => spawnStamp; }

    public List<ElementSpawner> ElementSpawners { get => elementSpawners; }
    public List<ElementSpawner> KeycardSpawners { get => keycardSpawners; }

    [SerializeField] private AreaSpawner[] areaSpawners;
    public AreaSpawner[] AreaSpawners { get => areaSpawners; }

    public List<AreaSpawner> validAreaSpawners = new List<AreaSpawner>();

    [SerializeField] private GameObject spawner_PF;
    public GameObject Spawner_PF { get => spawner_PF; }

#if UNITY_EDITOR
    [HideInInspector] public bool showAreaSpawnersBounds = false;

    [SerializeField] private bool debugMode;
#endif

    private void Awake()
    {
        instance = this;

        ManageStampsUIState(GameManager.Instance.IsInTutorial == false && spawnsAreAllowed);
        GameManager.Instance.D_tutorialState += ManageStampsUIState;
    }

    private void OnDestroy()
    {
        GameManager.Instance.D_tutorialState -= ManageStampsUIState;
    }

    private void ManageStampsUIState(bool state) => stampsCounter.alpha = state ? 1 : 0;

    private void Update()
    {
        if (spawnsAreAllowed == false) return;
        TrySpawnZombies();
        EvaluateStamp();
        ManageBreakups();
    }

    private void ManageBreakups()
    {
        if (isInBreakup)
        {
            // manage current breakup timer
            currentBreakup_TIMER -= Time.deltaTime;
            if (currentBreakup_TIMER > 0) return;

            // when the timer is <= 0, exit the breakup, set the duration before next, return
            isInBreakup = false;
            timerBeforeNextBreakup = spawnsBreakupsCooldowns.Evaluate(SpawnStamp);

            return;
        }

        timerBeforeNextBreakup -= Time.deltaTime;
        if (timerBeforeNextBreakup > 0) return;

        isInBreakup = true;
        currentBreakup_TIMER = spawnsBreakupsDurations.Evaluate(SpawnStamp);
    }

    public void AllowSpawns(bool allow)
    {
        spawnsAreAllowed = allow;
        ManageStampsUIState(GameManager.Instance.IsInTutorial == false && spawnsAreAllowed);

        if (allow == false) return;

        LeanTween.cancel(uiFiller.gameObject);
        LeanTween.value(1, 0, timeBetweenStamps).setOnUpdate((float val) =>
        {
            uiFiller.fillAmount = val;
        });

        stamp_TIMER = timeBetweenStamps;
        maxZombiesInSchool = (int)maxZombiesInSchoolByTime.Evaluate(spawnStamp);
        spawnCooldown = zombiesSpawnCooldown.Evaluate(spawnStamp);
    }

    private void TrySpawnZombies()
    {
        if (isInBreakup) return;

        if (currentZombiesInSchool < maxZombiesInSchool) SpawnNext();
    }

    public void ResetSpawners()
    {
        spawnStamp = 0;
        stamp_TIMER = timeBetweenStamps;
    }

    private void EvaluateStamp()
    {
        if (spawnStamp >= maxStamp) return;
        if (stamp_TIMER > 0)
        {
            stamp_TIMER -= Time.deltaTime;
            uiCounter.text = stamp_TIMER.ToString("F0");

            return;
        }

        LeanTween.cancel(uiFiller.gameObject);
        LeanTween.value(1, 0, timeBetweenStamps).setOnUpdate((float val) =>
        {
            uiFiller.fillAmount = val;
        });

        spawnStamp++;

        stamp_TIMER = timeBetweenStamps;
        maxZombiesInSchool = (int)maxZombiesInSchoolByTime.Evaluate(spawnStamp);
        spawnCooldown = zombiesSpawnCooldown.Evaluate(spawnStamp);
    }

    private void SpawnNext()
    {
        if (validAreaSpawners.Count <= 0) return;
        if (zombiesPool.Count > 0)
            if (zombiesPool.Peek().timeOfDeath > Time.timeSinceLevelLoad - spawnCooldown) return;

        validAreaSpawners[Random.Range(0, validAreaSpawners.Count)].SpawnObject();
    }

    public void TeleportZombie(NormalZombie zom)
    {
        if (validAreaSpawners.Count <= 0) return;

        validAreaSpawners[Random.Range(0, validAreaSpawners.Count)].TeleportZombieHere(zom);
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

        UIManager.Instance.UpdateKeycardsCounter(-1);
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

            case ElementSpawner.E_ElementToSpawn.IdleZombie:
                objectName.Append("_IDLEZOM");
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

    private void OnGUI()
    {
#if UNITY_EDITOR
        if (!debugMode) return;

        StringBuilder sb = new StringBuilder();
        sb.Append("Stamp : ");
        sb.AppendLine(SpawnStamp.ToString());

        sb.Append("Next Stamp Timer : ");
        sb.AppendLine(stamp_TIMER.ToString("F1"));

        sb.Append("Max Zombies in School : ");
        sb.AppendLine(maxZombiesInSchool.ToString());

        sb.Append("Zombies Spawn cooldown : ");
        sb.AppendLine(spawnCooldown.ToString("F1"));

        sb.Append("Is in breakup ? ");
        sb.AppendLine(isInBreakup.ToString());

        sb.Append(isInBreakup ? "Breakup Timer : " : "Next breakup : ");
        sb.AppendLine(isInBreakup ? currentBreakup_TIMER.ToString("F1") : timerBeforeNextBreakup.ToString("F1"));

        Rect r = new Rect(10, Screen.height / 2, Screen.width, 100);
        GUI.Label(r, sb.ToString());
#endif
    }
}
