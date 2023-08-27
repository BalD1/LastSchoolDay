using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnersManager : Singleton<SpawnersManager>
{
    [SerializeField] [Range(0, 10)] private int maxKeycardsToSpawn = 5;
    [SerializeField] [Range(0, 10)] private int minKeycardsToSpawn = 3;

    private static MonoPool<NormalZombie> zombiesPool;

    private Queue<NormalZombie> zombiesToTeleport = new Queue<NormalZombie>();
    public Queue<NormalZombie> ZombiesToTeleport { get => zombiesToTeleport; }

    [SerializeField] private CanvasGroup stampsCounter;
    [SerializeField] private Image uiFiller;
    [SerializeField] private TextMeshProUGUI uiCounter;
    [SerializeField] private TextMeshProUGUI uiStamp;

    [field: SerializeField] public AnimationCurve MaxZombiesInSchoolByTime { get; private set; }
    [field: SerializeField] public AnimationCurve ZombiesSpawnCooldown { get; private set; }
    [field: SerializeField] public AnimationCurve SpawnsBreakupsDurations { get; private set; }
    [field: SerializeField] public AnimationCurve SpawnsBreakupsCooldowns { get; private set; }

    [ReadOnly]
    [SerializeField] private float currentBreakup_TIMER;

    [ReadOnly]
    [SerializeField] private float timerBeforeNextBreakup = 10;

    [ReadOnly]
    [SerializeField] private bool isInBreakup;

    [ReadOnly]
    [SerializeField] private int spawnStamp;

    public delegate void D_StampChange(int newStamp);
    public D_StampChange D_stampChange;

    [SerializeField] private int maxStamp;

    [ReadOnly]
    [SerializeField] private int currentZombiesInSchool = 0;

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

    private float lastSpawnedZombieTime;

    private bool allPlayersAreInClassroom = false;

#if UNITY_EDITOR
    [HideInInspector] public bool showAreaSpawnersBounds = false;

    [SerializeField] private bool debugMode;
#endif

    protected override void EventsSubscriber()
    {
        TutorialEvents.OnTutorialStarted += OnTutorialStarted;
        TutorialEvents.OnTutorialEnded += OnTutorialEnded;
        HUBDoorEventHandler.OnInteractedWithDoor += ManageKeycardSpawn;
        GameManagerEvents.OnRunStarted += ActivateSpawns;
        RoomSpawnersEvents.OnEnteredRoomSpawner += ForceBreakup;
        GymnasiumCinematicEvents.OnGymnasiumCinematicStarted += OnGymnasiumCinematicStarted;
    }

    protected override void EventsUnSubscriber()
    {
        TutorialEvents.OnTutorialStarted -= OnTutorialStarted;
        TutorialEvents.OnTutorialEnded -= OnTutorialEnded;
        HUBDoorEventHandler.OnInteractedWithDoor -= ManageKeycardSpawn;
        GameManagerEvents.OnRunStarted -= ActivateSpawns;
        RoomSpawnersEvents.OnEnteredRoomSpawner -= ForceBreakup;
        GymnasiumCinematicEvents.OnGymnasiumCinematicStarted -= OnGymnasiumCinematicStarted;
    }

    private void OnTutorialStarted() => stampsCounter.alpha = 0;
    private void OnTutorialEnded() => stampsCounter.alpha = 1;
    private void ManageStampsUIState(bool state) => stampsCounter.alpha = state ? 1 : 0;

    private void Update()
    {
        if (spawnsAreAllowed == false) return;
        ManageBreakups();
        TrySpawnZombies();
        EvaluateStamp();
    }

    private void OnGymnasiumCinematicStarted()
    {
        AllowSpawns(false);
    }

    public void ForceBreakup()
    {
        if (isInBreakup) currentBreakup_TIMER = SpawnsBreakupsDurations.Evaluate(SpawnStamp);
        else timerBeforeNextBreakup = 0;
    }
    public void EndBreakup()
    {
        currentBreakup_TIMER = 0;
        isInBreakup = false;
        timerBeforeNextBreakup = SpawnsBreakupsCooldowns.Evaluate(SpawnStamp);
    }

    private void ManageBreakups()
    {
        if (allPlayersAreInClassroom) return;
        if (isInBreakup)
        {
            // manage current breakup timer
            currentBreakup_TIMER -= Time.deltaTime;
            if (currentBreakup_TIMER > 0) return;
            EndBreakup();
            return;
        }

        timerBeforeNextBreakup -= Time.deltaTime;
        if (timerBeforeNextBreakup > 0) return;

        isInBreakup = true;
        currentBreakup_TIMER = SpawnsBreakupsDurations.Evaluate(SpawnStamp);
    }

    public void OnAllPlayersInClassroom()
    {
        allPlayersAreInClassroom = true;
        isInBreakup = true;
        currentBreakup_TIMER = SpawnsBreakupsDurations.Evaluate(SpawnStamp) / 2;
    }

    public void PlayersExitedClassroom()
    {
        allPlayersAreInClassroom = false;
    }

    private void ActivateSpawns() => AllowSpawns(true);
    public void AllowSpawns(bool allow)
    {
        spawnsAreAllowed = allow;
        ManageStampsUIState(GameManager.Instance.IsInTutorial == false && spawnsAreAllowed);

        if (allow == false) return;

        uiFiller.fillAmount = 1;
        stamp_TIMER = timeBetweenStamps;
        maxZombiesInSchool = (int)MaxZombiesInSchoolByTime.Evaluate(spawnStamp);
        spawnCooldown = ZombiesSpawnCooldown.Evaluate(spawnStamp);
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
            stamp_TIMER -= allPlayersAreInClassroom ? (Time.deltaTime / 2) : Time.deltaTime;
            uiFiller.fillAmount = stamp_TIMER / timeBetweenStamps;
            uiCounter.text = stamp_TIMER.ToString("F0");

            return;
        }

        spawnStamp++;
        Vector2 worldPosOfUI = Camera.main.ScreenToWorldPoint(UIManager.Instance.StampWorldTextSpawnPos.position);
        TextPopup txt = TextPopup.Create("Zombies +", Camera.main.transform, false);
        txt.transform.position = worldPosOfUI;
        LeanTween.scale(txt.gameObject, Vector3.one * 1.3f, .3f).setLoopPingPong(1).setIgnoreTimeScale(true);
        LeanTween.moveLocalY(txt.gameObject, .01f, .25f).setIgnoreTimeScale(true);

        uiFiller.fillAmount = 1;

        D_stampChange?.Invoke(spawnStamp);

        uiStamp.text = spawnStamp.ToString();

        RectTransform stampContainerRT = stampsCounter.GetComponent<RectTransform>();
        LeanTween.scale(stampContainerRT, Vector3.one * 1.3f, .25f).setLoopPingPong(1);

        stamp_TIMER = timeBetweenStamps;
        maxZombiesInSchool = (int)MaxZombiesInSchoolByTime.Evaluate(spawnStamp);
        spawnCooldown = ZombiesSpawnCooldown.Evaluate(spawnStamp);
    }

    private void SpawnNext()
    {
        if (validAreaSpawners.Count <= 0) return;
        if (lastSpawnedZombieTime + ZombiesSpawnCooldown.Evaluate(spawnStamp) > Time.time) return;

        lastSpawnedZombieTime = Time.time;
        validAreaSpawners[Random.Range(0, validAreaSpawners.Count)].SpawnObject();
    }

    public static NormalZombie GetNextInPool() => zombiesPool.GetNext();
    public static void CheckPool()
    {
        if (zombiesPool == null)
            zombiesPool = new MonoPool<NormalZombie>
                (_createAction: () => GameAssets.Instance.GetRandomZombie.Create(Vector2.zero).GetComponent<NormalZombie>(),
                _parentContainer: GameManager.Instance.InstantiatedEntitiesParent);
    }
    public static void Enqueue(NormalZombie nz) => zombiesPool.Enqueue(nz);
    public static MonoPool<NormalZombie> GetPool() => zombiesPool;

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
        this.SpawnedKeycards();
        foreach (var item in keycardSpawners) Destroy(item.gameObject);
        keycardSpawners.Clear();
    }

    private void SpawnSingleCard(int remainingSpawns)
    {
        if (remainingSpawns <= 0) return;
        int randomSpawner = Random.Range(0, keycardSpawners.Count - 1);
        if (randomSpawner >= keycardSpawners.Count) return;

        Keycard key = keycardSpawners[randomSpawner].SpawnElement().GetComponent<Keycard>();
        key.onPickup += () => this.PickedupCard(key);

        Destroy(keycardSpawners[randomSpawner].gameObject);
        keycardSpawners.RemoveAt(randomSpawner);

        this.SpawnedKeycardSingle(key);
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
        elementSpawners[idx].CreateObjectName();
#endif

        if (elementSpawners[idx].ElementToSpawn == ElementSpawner.E_ElementToSpawn.Keycard)
            keycardSpawners.Add(elementSpawners[idx]);
    }

    private void OnEnable()
    {
        if (minKeycardsToSpawn > maxKeycardsToSpawn) minKeycardsToSpawn = maxKeycardsToSpawn;
    }

    public void SetDebugMode(bool state)
    {
#if UNITY_EDITOR
        debugMode = state;
#endif
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

        sb.AppendLine("Zombies in School : ");
        sb.Append(currentZombiesInSchool.ToString());
        sb.Append(" / ");
        sb.AppendLine(maxZombiesInSchool.ToString());

        sb.AppendLine("Zombies Spawn cooldown : ");
        float time = Time.time - lastSpawnedZombieTime;
        sb.Append((time < 0 ? 0 : time).ToString("F1"));
        sb.Append(" / ");
        sb.AppendLine(spawnCooldown.ToString("F1"));

        sb.Append("Is in breakup ? ");
        sb.AppendLine(isInBreakup.ToString());

        sb.Append(isInBreakup ? "Breakup Timer : " : "Next breakup : ");
        sb.AppendLine(isInBreakup ? currentBreakup_TIMER.ToString("F1") : timerBeforeNextBreakup.ToString("F1"));

        Rect r = new Rect(10, Screen.height / 2, Screen.width, 150);
        GUI.Label(r, sb.ToString());
#endif
    }
}
