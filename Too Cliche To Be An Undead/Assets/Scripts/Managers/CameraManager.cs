using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;
    public static CameraManager Instance
    {
        get
        {
            if (instance == null) Debug.LogError("CameraManager not found.");

            return instance;
        }
    }

    [SerializeField] private CinemachineVirtualCamera cam_followPlayers;
    [SerializeField] private CinemachineTargetGroup tg_players;

    public CinemachineVirtualCamera CAM_followPlayers { get => cam_followPlayers; }
    public CinemachineTargetGroup TG_Players { get => tg_players; }

    private void Awake()
    {
        DontDestroyOnLoad(cam_followPlayers);
        DontDestroyOnLoad(tg_players);

        GameManager.Instance._onSceneReload += OnSceneLoaded;

        instance = this;
    }

    private void Start()
    {
        SetArray();
    }

    private void OnSceneLoaded()
    {
        SetArray();
    }

    private void SetArray()
    {
        Array.Clear(tg_players.m_Targets, 0, tg_players.m_Targets.Length);

        foreach (var item in DataKeeper.Instance.playersDataKeep)
        {
            tg_players.AddMember(item.playerInput.transform.parent, 1, 0);
        }

    }

    private void OnDestroy()
    {
        GameManager.Instance._onSceneReload -= OnSceneLoaded;

        if (cam_followPlayers != null)
            Destroy(cam_followPlayers.gameObject);

        if (tg_players != null)
            Destroy(tg_players.gameObject);
    }
}
