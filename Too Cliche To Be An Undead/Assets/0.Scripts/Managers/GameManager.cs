using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BalDUtilities.Misc;
using UnityEngine.InputSystem;
using System;

public class GameManager : Singleton<GameManager>
{
    public enum E_CharactersNames
    {
        Shirley,
        Whitney,
        Jason,
        Nelson,
        None,
    }

    public static bool isAppQuitting { get; private set; }

    [SerializeField] private PlayerCharacter player1Ref;
    public static PlayerCharacter Player1Ref { get => Instance.player1Ref; }

    [SerializeField] private int playersCount;
    public int PlayersCount { get => playersCount; }

    [SerializeField] private Shop shop;
    public Shop GetShop { get => shop; }

    private bool isInTutorial;
    [SerializeField] public bool IsInTutorial { get; private set; }

    public delegate void D_OnPlayerIsSetup(int playerIdx);
    public D_OnPlayerIsSetup D_onPlayerIsSetup;

    public delegate void D_BossFightStarted();
    public D_BossFightStarted D_bossFightStarted;

    public delegate void D_BossFightEnded();
    public D_BossFightEnded D_bossFightEnded;

    public int currentAliveBossesCount = 0;

    [SerializeField] private Transform instantiatedEntitiesParent;
    [SerializeField] private Transform instantiatedKeycardsParent;
    [SerializeField] private Transform instantiatedMiscParent;
    [SerializeField] private Transform instantiatedProjectilesParent;

    public Transform InstantiatedEntitiesParent { get => instantiatedEntitiesParent; }
    public Transform InstantiatedKeycardsParent { get => instantiatedKeycardsParent; }
    public Transform InstantiatedMiscParent { get => instantiatedMiscParent; }
    public Transform InstantiatedProjectilesParent { get => instantiatedProjectilesParent; }

    [System.Serializable]
    public class PlayersByName
    {
        public string playerName;
        public PlayerCharacter playerScript;

        public PlayersByName(string _playerName, PlayerCharacter _playerScript)
        {
            this.playerName = _playerName;
            this.playerScript = _playerScript;
        }
    }

    [SerializeField] private Transform[] tutoSpawnPoints;
    public Transform[] TutoSpawnPoints { get => tutoSpawnPoints; }

    [SerializeField] private Transform[] ingameSpawnPoints;
    public Transform[] IngameSpawnPoints { get => ingameSpawnPoints; }

    [SerializeField] private bool allowEnemies = true;
    public bool AllowEnemies
    {
        get => allowEnemies;
        set
        {
            allowEnemies = value;
            SetActiveEveryEnemies(value);
        }
    }

    [SerializeField] public ShopTutorial ShopTuto;

    [SerializeField] private SpineGymnasiumDoor gymnasiumDoor;
    public SpineGymnasiumDoor GymnasiumDoor { get => gymnasiumDoor; }

    public List<PlayersByName> playersByName;

    public delegate void D_OnSceneReload();
    public D_OnSceneReload _onSceneReload;

    public bool hasKey;

    public static int MaxAttackers = 5;

    public static bool OPTION_DashToMouse = true;

    private bool firstPassInGameStateFlag = false;

    public bool AllowQuitLobby { get; set; } = true;

    private LTDescr gameoverCinematicTween;

    #region GameStates

    public enum E_GameState
    {
        MainMenu,
        InGame,
        Pause,
        Restricted,
        Win,
        GameOver,
        Cinematic,
        None,
    }

    private E_GameState gameState;
    public E_GameState GameState
    {
        get => gameState;
        set
        {
            gameState = value;
            ProcessStateChange(value);

            if (UIManager.Instance)
                UIManager.Instance.WindowsManager(value);
        }
    }

    private void ProcessStateChange(E_GameState newState)
    {
        switch (newState)
        {
            case E_GameState.MainMenu:
                DataKeeper.Instance.alreadyPlayedTuto = false;
                break;

            case E_GameState.InGame:
                Time.timeScale = 1;
                if (firstPassInGameStateFlag == false) firstPassInGameStateFlag = true;
                break;

            case E_GameState.Pause:
                Time.timeScale = 0;
                break;

            case E_GameState.Restricted:
                break;

            case E_GameState.Win:
                Time.timeScale = 0;
                PlayerEndStatsManager.Instance.KeepScores();
                break;

            case E_GameState.GameOver:
                UIManager.Instance.FadeAllHUD(false);
                UIManager.Instance.SetBlackBars(true, .2f);
                gameoverCinematicTween = CameraManager.Instance.MoveCamera(PlayersManager.Instance.LastDeadPlayerTransform.position, () =>
                {
                    LeanTween.delayedCall(2, () => UIManager.Instance.ShowGameOverScreen());
                }, 1); 
                break;

            case E_GameState.Cinematic:
                break;

            default:
                Debug.LogError(newState + "not found in switch statement.");
                break;
        }
        this.GameStateChange(newState);
    }

    #endregion

    public enum E_ScenesNames
    {
        MainMenu,
        MainScene,
        Map,
        MapTiles,
        Playground,
        LoadingScreen,
    }

    public static float gameTimeSpeed = 1f;

    private int acquiredCards;
    public int AcquiredCards
    {
        get => acquiredCards;
        set
        {
            acquiredCards = value;

            if (acquiredCards >= neededCards && neededCards > 0) GymnasiumDoor.SetMiniampGoalState(true);
        }
    }

    private int neededCards;
    public int NeededCards { get => neededCards; set => neededCards = value; }

    protected override void EventsSubscriber()
    {
        PlayerInputsEvents.OnPauseCall += HandlePause;
        CinematicManagerEvents.OnChangeCinematicState += OnChangeCinematicState;
        DialogueManagerEvents.OnStartDialogue += SetStateToCinematicIfNotAlready;
        DialogueManagerEvents.OnEndDialogue += OnDialogueEnded;
        SpawnersManagerEvents.OnSpawnedKeycardSingle += OnSpawnedSingleKeycard;
        SpawnersManagerEvents.OnPickedupCard += OnPickedUpKeycard;
        HUBDoorEventHandler.OnInteractedWithDoor += OnHubDoorOpened;

        if (!CompareCurrentScene(E_ScenesNames.MainMenu))
        {
            UIManagerEvents.OnEnteredUI += OnEnteredUI;
            UIManagerEvents.OnExitedUI += OnExitedUI;
        }
    }

    protected override void EventsUnSubscriber()
    {
        PlayerInputsEvents.OnPauseCall -= HandlePause;
        CinematicManagerEvents.OnChangeCinematicState -= OnChangeCinematicState;
        DialogueManagerEvents.OnStartDialogue -= SetStateToCinematicIfNotAlready;
        DialogueManagerEvents.OnEndDialogue -= OnDialogueEnded;
        SpawnersManagerEvents.OnSpawnedKeycardSingle -= OnSpawnedSingleKeycard;
        SpawnersManagerEvents.OnPickedupCard -= OnPickedUpKeycard;
        HUBDoorEventHandler.OnInteractedWithDoor -= OnHubDoorOpened;

        if (!CompareCurrentScene(E_ScenesNames.MainMenu))
        {
            UIManagerEvents.OnEnteredUI -= OnEnteredUI;
            UIManagerEvents.OnExitedUI -= OnExitedUI;
        }
    }

    private void OnHubDoorOpened()
        => this.RunStarted();
    private void OnEnteredUI()
    {
        if (GameState == E_GameState.InGame) GameState = E_GameState.Pause;
    }
    private void OnExitedUI()
        => GameState = E_GameState.InGame;

    protected override void Awake()
    {
        base.Awake();

        LeanTween.cancelAll();
        instance = this;

        isAppQuitting = false;
        Application.quitting += () => isAppQuitting = true;

        InputSystem.settings.SetInternalFeatureFlag("DISABLE_SHORTCUT_SUPPORT", true);

        D_bossFightStarted += () => currentAliveBossesCount++;
        D_bossFightEnded += () => currentAliveBossesCount--;

        SetPlayersByNameList();
    }

    protected override void Start()
    {
        base.Start();
        AcquiredCards = 0;
        NeededCards = 0;

        InitState();

        if (GameState == E_GameState.InGame)
        {
            DataKeeper.Instance.runsCount++;

            IsInTutorial = (DataKeeper.Instance.skipTuto == false && DataKeeper.Instance.alreadyPlayedTuto == false);
            if (!IsInTutorial)
            {
                SoundManager.Instance.PlayMusic(SoundManager.E_MusicClipsTags.InLobby);
            }
            else SoundManager.Instance.PlayMusic(SoundManager.E_MusicClipsTags.MainScene);

            ZombieAudio.currentZombiesHurtCount = 0;
        }
        else if (CompareCurrentScene(E_ScenesNames.MainMenu))
        {
            PlayerCharacter.SetMoney(0);
            PlayerCharacter.SetLevel(0);
            if (DataKeeper.Instance != null)
            {
                DataKeeper.Instance.money = 0;
                DataKeeper.Instance.maxLevel = 0;
                DataKeeper.Instance.unlockedLevels = new List<int>();
                DataKeeper.Instance.runsCount = 0;
            }
        }
    }

    private void OnChangeCinematicState(bool state)
    {
        GameState = state ? E_GameState.Cinematic : E_GameState.InGame;
    }
    private void SetStateToCinematicIfNotAlready(bool fromCinematic)
    {
        if (GameState != E_GameState.Cinematic) GameState = E_GameState.Cinematic;
    }
    private void OnDialogueEnded(bool fromCinematic)
    {
        if (!fromCinematic) GameState = E_GameState.InGame;
    }

    public void SetPlayersByNameList()
    {
        if (DataKeeper.Instance.IsPlayerDataKeepSet())
        {
            foreach (var item in DataKeeper.Instance.playersDataKeep)
            {
                var p = new PlayersByName(item.playerName, item.playerInput.GetComponentInParent<PlayerCharacter>());
                playersByName.Add(p);
                playersCount++;
            }
        }
    }
    public void AddPlayerToList(DataKeeper.PlayerDataKeep data)
    {
        var p = new PlayersByName(data.playerName, data.playerInput.GetComponentInParent<PlayerCharacter>());
        playersByName.Add(p);
        playersCount++;
    }
    public void RemovePlayerAt(int idx)
    {
        playersByName.RemoveAt(idx);
        playersCount--;
    }

    private void InitState()
    {
        if (CompareCurrentScene(E_ScenesNames.MainMenu))
        {
            GameState = E_GameState.MainMenu;
            Time.timeScale = 1;
        }
        else
        {
            GameState = E_GameState.InGame;
        }
    }

    private void OnSpawnedSingleKeycard(Keycard card)
        => NeededCards++;
    private void OnPickedUpKeycard(Keycard card)
        => AcquiredCards++;

    public void CancelGameOver()
    {
        CameraManager.Instance.TG_Players.AddMember(Player1Ref.transform, 1, 0);
        LeanTween.cancel(gameoverCinematicTween.uniqueId);
        PlayersManager.Instance.SetAllPlayersControlMapToInGame();
        UIManager.Instance.FadeAllHUD(true);
        UIManager.Instance.SetBlackBars(false, .2f);
        CameraManager.Instance.EndCinematic();
        UIManager.Instance.HideGameOverScreen();
    }

    public void HandlePause()
    {
        if (GameState.Equals(E_GameState.InGame))
        {
            GameState = E_GameState.Pause;
        }
    }

    public void SetPlayer1(PlayerCharacter p1) => this.player1Ref = p1;

    public void SetActiveEveryEnemies(bool active)
    {
        if (active)
        {
            foreach (var item in Resources.FindObjectsOfTypeAll<EnemyBase>())
            {
                item.gameObject.SetActive(active);
                item.transform.GetChild(0).gameObject.SetActive(active);
            }
        }
        else
        {
            foreach (var item in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                item.SetActive(active);
            }
        }
    }

    public void TeleportAllPlayers(Vector2 position)
    {
        foreach (var item in playersByName)
        {
            PlayerCharacter player = item.playerScript;
            if (!player.IsAlive()) continue;
            player.gameObject.transform.position = position;
        }
    }

    public void TeleportPlayerAtCameraCenter(int playerIdx)
    {
        Vector3 pos = CameraManager.Instance.gameObject.transform.position;
        pos.z = 0;
        playersByName[playerIdx].playerScript.gameObject.transform.position = pos;
    }

    public Transform GetSpawnPoint(int playerId)
    {
#if UNITY_EDITOR
        if (CompareCurrentScene(E_ScenesNames.MainScene) == false)
            return ReturnSpawnPointsOrSelf(IngameSpawnPoints, playerId); 
#endif

        if (DataKeeper.Instance.skipTuto || DataKeeper.Instance.alreadyPlayedTuto)
            return ReturnSpawnPointsOrSelf(IngameSpawnPoints, playerId);
        else
            return ReturnSpawnPointsOrSelf(TutoSpawnPoints, playerId);

    }

    private Transform ReturnSpawnPointsOrSelf(Transform[] points, int idx)
    {
        if (points.Length > 0) return points[idx];
        else
        {
            Debug.LogErrorFormat($"Spawnpoints of {points} were not set");
            return this.transform;
        }
    }

    public static bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return (layerMask.value & (1 << obj.layer)) > 0;
    }

    #region Scenes

    /// <summary> <para>
    /// Returns true if the current scene is <paramref name="sceneName"/>. </para>
    /// <para> Uses <seealso cref="BalDUtilities.Misc.EnumsExtension"/> from <seealso cref="BalDUtilities.Misc"/>
    /// </para> </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public static bool CompareCurrentScene(E_ScenesNames sceneName)
    {
        return EnumsExtension.EnumToString(sceneName).Equals(SceneManager.GetActiveScene().name);
    }
    public static bool CompareCurrentScene(string sceneName)
    {
        return sceneName.Equals(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Changes the scene to <paramref name="newScene"/>.
    /// </summary>
    /// <param name="newScene"></param>
    public static void ChangeScene(E_ScenesNames newScene, bool allowReload = false)
    {
        string sceneName = EnumsExtension.EnumToString(newScene);

        if (!allowReload)
        {
            if (CompareCurrentScene(sceneName))
            {
                Debug.LogError(sceneName + " is already the current scene.");
                return;
            }
        }

        LoadScene(newScene.ToString());
    }

    public void ReloadScene()
    {
        _onSceneReload?.Invoke();

        PlayerEndStatsManager.Instance.KeepScores();

        LoadScene(SceneManager.GetActiveScene().name);
    }

    private static void LoadScene(string sceneName)
    {
        LeanTween.cancelAll();

        LoadingScreenManager.SceneToLoad = sceneName;
        SceneManager.LoadScene(E_ScenesNames.LoadingScreen.ToString());
    }

    #endregion
}
