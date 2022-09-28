using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null) Debug.LogError("UIManager instance was not found.");

            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public void WindowsManager(GameManager.E_GameState newState)
    {
        switch (newState)
        {
            case GameManager.E_GameState.MainMenu:
                break;

            case GameManager.E_GameState.InGame:
                break;

            case GameManager.E_GameState.Pause:
                break;

            case GameManager.E_GameState.GameOver:
                break;

            default:
                Debug.LogError(newState + "not found in switch statement.");
                break;
        }
    }
}
