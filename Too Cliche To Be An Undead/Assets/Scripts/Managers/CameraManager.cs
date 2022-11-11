using Cinemachine;
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
        instance = this;
    }

    private void Start()
    {
        foreach (var item in DataKeeper.Instance.playersDataKeep)
        {
            tg_players.AddMember(item.playerInput.transform.parent, 1, 0);
        }
    }

    private void OnDestroy()
    {
        Destroy(cam_followPlayers.gameObject);
        Destroy(cam_followPlayers.gameObject);
    }
}
