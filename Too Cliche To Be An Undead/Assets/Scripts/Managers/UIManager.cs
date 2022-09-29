using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private Button firstSelectedButton_MainMenu;
    [SerializeField] private Slider firstSelectedButton_Options;

    private GameObject lastSelected;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu)) SelectButton("MainMenu");
    }

    /// <summary>
    /// Handles the selection of buttons on menu changes. <paramref name="context"/> is used to decided which button should be selected.
    /// </summary>
    /// <param name="context"></param>
    public void SelectButton(string context)
    {
        GameObject currentSelected = eventSystem.currentSelectedGameObject;

        switch (context)
        {
            case "MainMenu":
                firstSelectedButton_MainMenu?.Select();
                break;

            case "Options":
                firstSelectedButton_Options?.Select();
                break;

            case "Last":
                eventSystem.SetSelectedGameObject(lastSelected);
                break;

            default:
                Debug.LogError(context + " was not found in switch statement.");
                break;
        }

        lastSelected = currentSelected;
    }

    /// <summary>
    /// Manages the windows display using <paramref name="newState"/>.
    /// </summary>
    /// <param name="newState"></param>
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
