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
            //if (instance == null) Debug.LogError("CameraManager not found.");

            return instance;
        }
    }

    [SerializeField] private CinemachineVirtualCamera cam_followPlayers;
    [SerializeField] private CinemachineTargetGroup tg_players;

    private Camera mainCam;

    public CinemachineVirtualCamera CAM_followPlayers { get => cam_followPlayers; }
    public CinemachineTargetGroup TG_Players { get => tg_players; }

    [SerializeField] private Transform[] markers;
    [SerializeField] private BoxCollider2D[] markersColliders;

    public Transform[] Markers { get => markers; }

    [SerializeField] private float maxDistance;

    [SerializeField] private List<Transform> invisiblePlayers = new List<Transform>();
    [SerializeField] private List<Transform> playersToRemoveFromList = new List<Transform>();

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
        mainCam = this.GetComponent<Camera>();

        UIManager.Instance.AddMakersInCollidersArray(markersColliders);
    }

    private void LateUpdate()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCam);

        for (int i = 0; i < invisiblePlayers.Count; i++)
        {
            Vector3 minScreenBounds = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0));
            Vector3 maxScreenBounds = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

            markers[i].position = new Vector3(Mathf.Clamp(invisiblePlayers[i].position.x, minScreenBounds.x + 2, maxScreenBounds.x - 2), 
                                              Mathf.Clamp(invisiblePlayers[i].position.y, minScreenBounds.y + 2, maxScreenBounds.y - 2),
                                              0);

            float dist = Vector2.Distance(invisiblePlayers[i].position, markers[i].position);
            float markerScale = Mathf.Clamp01(1 - (dist / maxDistance));

            Vector3 v = markers[i].transform.localScale;
            v.x = markerScale;
            v.y = markerScale;
            markers[i].transform.localScale = v;

            if (dist > maxDistance) invisiblePlayers[i].transform.position = this.transform.position;
        }

        foreach (var item in playersToRemoveFromList)
        {
            if (invisiblePlayers.Contains(item)) invisiblePlayers.Remove(item);
        }
        playersToRemoveFromList.Clear();
    }

    private void OnSceneLoaded()
    {
        SetArray();
    }

    private void SetArray()
    {
        Array.Clear(tg_players.m_Targets, 0, tg_players.m_Targets.Length);

        List<GameManager.PlayersByName> pbn = GameManager.Instance.playersByName;

        tg_players.AddMember(pbn[0].playerScript.transform, 2, 0);

        if (pbn.Count <= 1) return;

        for (int i = 1; i < pbn.Count; i++)
        {
            tg_players.AddMember(pbn[i].playerScript.transform, 1, 0);
        }
    }

    public void PlayerBecameInvisible(Transform player, int playerIdx)
    {
        if (invisiblePlayers.Contains(player)) return;

        invisiblePlayers.Add(player);
        int idx = invisiblePlayers.IndexOf(player);

        GameManager.E_CharactersNames character = DataKeeper.Instance.playersDataKeep[playerIdx].character;

        if (markers == null || ReferenceEquals(markers, null)) return;

        markers[idx].localScale = Vector3.one;
        markers[idx].GetComponent<SpriteRenderer>().sprite = UIManager.Instance.GetBasePortrait(character);
        markers[idx].gameObject.SetActive(true);
    }

    public void PlayerBecameVisible(Transform player)
    {
        markers[invisiblePlayers.IndexOf(player)].gameObject.SetActive(false);
        playersToRemoveFromList.Add(player);
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
