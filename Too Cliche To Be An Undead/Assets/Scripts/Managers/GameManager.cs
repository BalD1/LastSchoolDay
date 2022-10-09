using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BalDUtilities.Misc;

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

    [SerializeField] private PlayerCharacter playerRef;
    public PlayerCharacter PlayerRef { get => playerRef; }

    #region GameStates

    public enum E_GameState
    {
        MainMenu,
        InGame,
        Pause,
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
                break;

            case E_GameState.Pause:
                Time.timeScale = 0;
                break;

            case E_GameState.Win:
                Time.timeScale = 0;
                break;

            case E_GameState.GameOver:
                Time.timeScale = 0;
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
        T_qua
    }

    public static float gameTimeSpeed = 1f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitState();
    }

    private void InitState()
    {
        if (CompareCurrentScene(E_ScenesNames.MainMenu)) GameState = E_GameState.MainMenu;
        else if (CompareCurrentScene(E_ScenesNames.MainScene)) GameState = E_GameState.InGame;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) HandlePause();
    }

    public void HandlePause()
    {
        if (GameState.Equals(E_GameState.InGame))
        {
            GameState = E_GameState.Pause;
            return;
        }

        UIManager.Instance.CloseYoungerMenu();
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

    #endregion
}
