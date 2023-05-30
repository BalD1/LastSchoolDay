using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BalDUtilities.Misc;
using UnityEngine.InputSystem;
using System;
using static UnityEditor.Progress;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            //if (instance == null) Debug.LogError("GameManager Instance not found.");

            return instance;
        }
    }

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
    [SerializeField] public bool IsInTutorial
    {   
        get => isInTutorial;
        set
        {
            isInTutorial = value;

            D_tutorialState?.Invoke(value);
        }
    }

    public delegate void D_OnPlayerIsSetup(int playerIdx);
    public D_OnPlayerIsSetup D_onPlayerIsSetup;

    public delegate void D_TutorialState(bool isActive);
    public D_TutorialState D_tutorialState;

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

    public delegate void D_OnRunStarted();
    public D_OnRunStarted _onRunStarted;

    public bool hasKey;

    public static int MaxAttackers = 5;

    public static bool OPTION_DashToMouse = true;

    private bool firstPassInGameStateFlag = false;

    public bool allowQuitLobby = true;

    public event Action OnRunStarted;
    public void RunStarted() => OnRunStarted?.Invoke();

    #region GameStates

    public enum E_GameState
    {
        MainMenu,
        InGame,
        Pause,
        Restricted,
        Win,
        GameOver,
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
                else PlayersManager.Instance.SetAllPlayersControlMapToInGame();
                break;

            case E_GameState.Pause:
                Time.timeScale = 0;
                PlayersManager.Instance.SetAllPlayersControlMapToUI();
                break;

            case E_GameState.Restricted:
                break;

            case E_GameState.Win:
                Time.timeScale = 0;
                PlayersManager.Instance.SetAllPlayersControlMapToUI();
                PlayerEndStatsManager.Instance.KeepScores();
                break;

            case E_GameState.GameOver:
                PlayersManager.Instance.SetAllPlayersControlMapToUI();

                UIManager.Instance.FadeAllHUD(false);
                UIManager.Instance.SetBlackBars(true, .2f);
                CameraManager.Instance.MoveCamera(PlayersManager.Instance.LastDeadPlayerTransform.position, () =>
                {
                    LeanTween.delayedCall(2, () => UIManager.Instance.ShowGameOverScreen());
                }, 1); 
                break;

            default:
                Debug.LogError(newState + "not found in switch statement.");
                break;
        }
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

    private void Awake()
    {
        LeanTween.cancelAll();
        instance = this;

        isAppQuitting = false;
        Application.quitting += () => isAppQuitting = true;

        InputSystem.settings.SetInternalFeatureFlag("DISABLE_SHORTCUT_SUPPORT", true);

        D_bossFightStarted += () => currentAliveBossesCount++;
        D_bossFightEnded += () => currentAliveBossesCount--;

        SetPlayersByNameList();
    }

    private void Start()
    {
        AcquiredCards = 0;
        NeededCards = 0;

        InitState();

        if (GameState == E_GameState.InGame)
        {
            DataKeeper.Instance.runsCount++;
            UIManager.Instance.UpdateKeycardsCounter(-1);

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
            DataKeeper.Instance.runsCount = 0;
            LeanTween.delayedCall(1, () =>PlayersManager.Instance.CreateP1()).setIgnoreTimeScale(true);
            SoundManager.Instance.PlayMusic(SoundManager.E_MusicClipsTags.MainMenu);
        }
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

    public void QuitLobby(int id)
    {
        if (allowQuitLobby == false) return;
        if (id <= 0) return;
        DataKeeper.Instance.RemoveData(id);
        UIManager.Instance.PlayerQuitLobby(id);
    }

    private void InitState()
    {
        if (CompareCurrentScene(E_ScenesNames.MainMenu))
        {
            GameState = E_GameState.MainMenu;
            Time.timeScale = 1;
            UIManager.Instance.InstantFadeScreen(true);
        }
        else
        {
            UIManager.Instance.InstantFadeScreen(true);
            GameState = E_GameState.InGame;

            GameStartScreenFade();
        }
    }

    private void GameStartScreenFade()
    {
        UIManager.Instance.FadeScreen(fadeOut: false, onCompleteAction: ScreenFadeEnd, time: 2);
    }

    private void ScreenFadeEnd()
    {
        if (DataKeeper.Instance.skipTuto || DataKeeper.Instance.alreadyPlayedTuto)
        {
            // TODO : Replace by simply deactivating players inputs
            if (!DialogueManager.IsDialogueActive && DataKeeper.Instance.runsCount != 2)
                PlayersManager.Instance.SetAllPlayersControlMapToInGame();
        }
    }

    public void HandlePause()
    {
        if (UIManager.Instance.OpenMenusQueues.Count > 0)
        {
            UIManager.Instance.CloseYoungerMenu();
            return;
        }

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
    public void TeleportAllPlayers(List<Vector2> positions)
    {
        for (int i = 0; i < playersByName.Count; i++)
        {
            PlayerCharacter player = playersByName[i].playerScript;
            Vector2 pos = positions[i % (positions.Count - 1)];

            if (!player.IsAlive()) continue;

            player.gameObject.transform.position = pos;
        }
    }
    public void TeleportAllPlayers(List<Transform> transforms)
    {
        List<Vector2> positions = new List<Vector2>();
        foreach (var item in transforms)
        {
            positions.Add(item.position);
        }

        TeleportAllPlayers(positions);
    }

    public void MoveAllPlayers(List<Vector2> positions, Action endAction = null)
    {
        int playersUnfinishedAnimations = playersByName.Count;
        for (int i = 0; i < playersByName.Count; i++)
        {
            PlayerCharacter player = playersByName[i].playerScript;
            player.StateManager.SwitchState(player.StateManager.cinematicState);
            PlayerAnimationController animationController = player.AnimationController;
            animationController.SetAnimation(animationController.animationsData.WalkAnim, true);

            Vector2 pos = positions[i % (positions.Count - 1)];

            if (!player.IsAlive()) continue;

            player.AnimationController.FlipSkeleton(pos.x > player.transform.position.x);

            float travelTime = Vector2.Distance(player.transform.position, pos) / player.MaxSpeed_M;

            player.gameObject.transform.LeanMove(pos, travelTime)
                .setOnComplete(() =>
                {
                    playersUnfinishedAnimations--;
                    player.StateManager.SwitchState(player.StateManager.idleState);
                    if (playersUnfinishedAnimations <= 0)
                        endAction?.Invoke();
                });
        }
    }
    public void MoveAllPlayers(List<Transform> transforms, Action endAction = null)
    {
        List<Vector2> positions = new List<Vector2>();
        foreach (var item in transforms)
        {
            positions.Add(item.position);
        }

        MoveAllPlayers(positions, endAction);
    }

    public void TeleportPlayerAtCameraCenter(int playerIdx)
    {
        playersByName[playerIdx].playerScript.gameObject.transform.position = CameraManager.Instance.gameObject.transform.position;
    }

    public void SetAllPlayersStateTo(FSM_Base<FSM_Player_Manager> newState)
    {
        foreach (var item in playersByName)
        {
            item.playerScript.StateManager.SwitchState(newState);
        }
    }
    public List<T> SetAllPlayersStateTo<T>(FSM_Player_Manager.E_PlayerState newState) where T : FSM_Base<FSM_Player_Manager>
    {
        List<T> playersState = new List<T>();
        foreach (var item in playersByName)
        {
            playersState.Add(item.playerScript.StateManager.SwitchState<T>(newState));
        }

        return playersState;
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

        TextPopup.popupPool.Clear();
        HealthPopup.popupPool.Clear();
        ProjectileBase.ResetQueue();

        LoadingScreenManager.SceneToLoad = sceneName;
        SceneManager.LoadScene(E_ScenesNames.LoadingScreen.ToString());
    }

    #endregion
}
