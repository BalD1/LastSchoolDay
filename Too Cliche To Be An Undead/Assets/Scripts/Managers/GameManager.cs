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

    #region GameStates

    public enum E_GameState
    {
        MainMenu,
        InGame,
        Pause,
        GameOver
    }

    private E_GameState gameState;
    public E_GameState GameState
    {
        get => gameState;
        set
        {
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
                break;

            case E_GameState.Pause:
                break;

            case E_GameState.GameOver:
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

    private void Awake()
    {
        instance = this;
    }

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
    public static void ChangeScene(E_ScenesNames newScene, bool async = false)
    {
        string sceneName = EnumsExtension.EnumToString(newScene);

        if (CompareCurrentScene(sceneName))
        {
            Debug.LogError(sceneName + " is already the current scene.");
            return;
        }

        if (async) SceneManager.LoadSceneAsync(sceneName);
        else SceneManager.LoadScene(sceneName);
    }
}
