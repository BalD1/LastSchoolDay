using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayersManager : MonoBehaviour
{
    private static PlayersManager instance;
    public static PlayersManager Instance
    {
        get
        {
            if (instance == null) Debug.LogError("PlayersManager Instance not found.");

            return instance;
        }
    }

    [SerializeField] private PlayerPanelsManager panelsManager;

    [SerializeField] private PlayerCharacter player1;

    [ReadOnly] [SerializeField] private List<PlayerInput> playerInputs = new List<PlayerInput>();

    [SerializeField] private InputAction joinAction;
    [SerializeField] private InputAction leaveAction;

    public event Action<PlayerInput> PlayerJoined;
    public event Action<PlayerInput> PlayerLeft;

    private void Awake()
    {
        instance = this;

        joinAction.performed += context => JoinAction(context);

        leaveAction.performed += context => LeaveAction(context);

        DontDestroyOnLoad(this);
    }

    public void EnableActions()
    {
        joinAction.Enable();
        leaveAction.Enable();
    }

    public void DisableActions()
    {
        joinAction.Disable();
        leaveAction.Disable();
    }

    public void SetupPanels(int idx)
    {
        if (panelsManager != null)
        {
            panelsManager.SetupPanel(idx);
        }
    }

    private void OnPlayerJoined(PlayerInput input)
    {
        playerInputs.Add(input);

        if (PlayerJoined != null) PlayerJoined(input);


    }

    private void OnPlayerLeft(PlayerInput input)
    {
        playerInputs.Remove(input);
        DataKeeper.Instance.RemoveData(input);

        if (PlayerLeft != null) PlayerLeft(input);
    }

    private void JoinAction(InputAction.CallbackContext context)
    {
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }

    private void LeaveAction(InputAction.CallbackContext context)
    {
    }
}
