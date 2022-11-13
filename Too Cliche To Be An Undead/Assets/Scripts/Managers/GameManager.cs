using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BalDUtilities.Misc;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) Debug.LogError("GameManager Instance not found.");

            return instance;
        }
    }

    public enum E_CharactersNames
    {
        Shirley,
        Whitney,
        Jason,
        Nelson,
    }

    [SerializeField] private PlayerCharacter player1Ref;
    public static PlayerCharacter Player1Ref { get => Instance.player1Ref; }

    [SerializeField] private Shop shop;
    public Shop GetShop { get => shop; }

    [System.Serializable]
    public class PlayersByName
    {
        public string playerName;
        public PlayerCharacter playerScript;
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

    public List<PlayersByName> playersByName;

    public delegate void D_OnSceneReload();
    public D_OnSceneReload _onSceneReload;

    public bool hasKey;

    public static int MaxAttackers = 5;

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
        T_flo,
        Test_Kankan,
    }

    public static float gameTimeSpeed = 1f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (DataKeeper.Instance.IsPlayerDataKeepSet())
        {
            foreach (var item in DataKeeper.Instance.playersDataKeep)
            {
                var p = new PlayersByName();
                p.playerName = item.playerName;
                p.playerScript = item.playerInput.GetComponentInParent<PlayerCharacter>();
                playersByName.Add(p);
            }
        }    
        InitState();
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
        else if (CompareCurrentScene(E_ScenesNames.MainScene)) GameState = E_GameState.InGame;
#if UNITY_EDITOR
        else if (CompareCurrentScene(E_ScenesNames.T_flo)) GameState = E_GameState.InGame;
        else if (CompareCurrentScene(E_ScenesNames.Test_Kankan)) GameState = E_GameState.InGame;
#endif
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
    public static void ChangeScene(E_ScenesNames newScene, bool allowReload = false, bool async = false)
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

        if (async) SceneManager.LoadSceneAsync(sceneName);
        else SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene()
    {
        _onSceneReload?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion
}
