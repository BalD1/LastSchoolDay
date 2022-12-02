using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BalDUtilities.Misc;
using UnityEngine.InputSystem;
using System;
using UnityEditor;
using System.Text;

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

    [SerializeField] private Transform instantiatedEntitiesParent;
    [SerializeField] private Transform instantiatedKeycardsParent;
    [SerializeField] private Transform instantiatedMiscParent;

    public Transform InstantiatedEntitiesParent { get => instantiatedEntitiesParent; }
    public Transform InstantiatedKeycardsParent { get => instantiatedKeycardsParent; }
    public Transform InstantiatedMiscParent { get => instantiatedMiscParent; }

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

    [SerializeField] private Transform[] spawnPoints;
    public Transform[] SpawnPoints { get => spawnPoints; }

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

    [SerializeField] private FightArena fightArena;

    public List<PlayersByName> playersByName;

    public delegate void D_OnSceneReload();
    public D_OnSceneReload _onSceneReload;

    public delegate void D_OnRunStarted();
    public D_OnRunStarted _onRunStarted;

    public bool hasKey;

    public static int MaxAttackers = 5;

    public static bool OPTION_DashToMouse = true;

    #region GameStates

    public enum E_GameState
    {
        MainMenu,
        InGame,
        Pause,
        Restricted,
        Win,
        GameOver
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
                break;

            case E_GameState.InGame:
                Time.timeScale = 1;
                PlayersManager.Instance.SetAllPlayersControlMapToInGame();
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
                break;

            case E_GameState.GameOver:
                Time.timeScale = 0;
                PlayersManager.Instance.SetAllPlayersControlMapToUI();
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
        Playground,
    }

    public static float gameTimeSpeed = 1f;

    public static int AcquiredCards { get; set; }

    public static int NeededCards { get; set; }

    private void Awake()
    {
        instance = this;

        isAppQuitting = false;
        AcquiredCards = 0;
        NeededCards = 0;
        Application.quitting += () => isAppQuitting = true;
    }

    private void Start()
    {
        SetPlayersByNameList();
        InitState();
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

    public void QuitLobby(int id)
    {
        if (id <= 0) return;
        DataKeeper.Instance.RemoveData(id);
        UIManager.Instance.PlayerQuitLobby(id);
    }

    private void InitState()
    {
        if (CompareCurrentScene(E_ScenesNames.MainMenu)) GameState = E_GameState.MainMenu;
        else GameState = E_GameState.InGame;
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

    public void TeleportPlayerAtCameraCenter(int playerIdx)
    {
        playersByName[playerIdx].playerScript.gameObject.transform.position = CameraManager.Instance.gameObject.transform.position;
    }

    public void StartArena()
    {
        fightArena?.SpawnNext(0);
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

        SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene()
    {
        _onSceneReload?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion
}
