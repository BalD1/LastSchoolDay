using Cinemachine;
using System;
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
    private CinemachineBasicMultiChannelPerlin bmcp;

    [field: SerializeField] public Camera minimapCamera { get; private set; }
    [field: SerializeField] public Vector2 minimapCenterPosition { get; private set; }
    [field: SerializeField] public float minimapOverviewSize { get; private set; }
    [field: SerializeField] public float minimapNormalSize { get; private set; }

    [SerializeField] private Transform volumeTrigger;

    private float shake_TIMER;
    private float shake_DURATION;
    private float shake_startingIntensity;

    private Camera mainCam;

    public CinemachineVirtualCamera CAM_followPlayers { get => cam_followPlayers; }
    public CinemachineTargetGroup TG_Players { get => tg_players; }

    [SerializeField] private Transform[] markers;
    [SerializeField] private BoxCollider2D[] markersColliders;

    public Transform[] Markers { get => markers; }

    [SerializeField] private float maxDistance;
    [SerializeField] private float scaleMultiplier = .5f;

    [SerializeField] private bool hardFocusOnP1 = false;

    [SerializeField] private List<Transform> invisiblePlayersTransform = new List<Transform>();
    [SerializeField] private List<Transform> playersToRemoveFromList = new List<Transform>();

    private List<PlayerCharacter> players = new List<PlayerCharacter>();

    private bool cinematicMode = false;

    private void Awake()
    {
        cam_followPlayers.transform.SetParent(null);
        tg_players.transform.SetParent(null);

        DontDestroyOnLoad(cam_followPlayers);
        DontDestroyOnLoad(tg_players);

        bmcp = cam_followPlayers.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        GameManager.Instance._onSceneReload += OnSceneLoaded;

        instance = this;
    }

    private void Start()
    {
        SetArray();
        mainCam = this.GetComponent<Camera>();

        UIManager.Instance.AddMakersInCollidersArray(markersColliders);

        AttachMinimapCamera();
    }

    private void Update()
    {
        if (shake_TIMER > 0)
        {
            shake_TIMER -= Time.deltaTime;
            bmcp.m_AmplitudeGain = Mathf.Lerp(shake_startingIntensity, 0f, 1 - (shake_TIMER / shake_DURATION));
        }

        volumeTrigger.SetLocalPositionZ(this.transform.position.z * -1);
    }

    private void LateUpdate()
    {
        if (cinematicMode) return;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCam);

        for (int i = 0; i < invisiblePlayersTransform.Count; i++)
        {
            Vector2 minScreenBounds = mainCam.ScreenToWorldPoint(new Vector3(0, 0));
            Vector2 maxScreenBounds = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

            markers[i].position = new Vector2(Mathf.Clamp(invisiblePlayersTransform[i].position.x, minScreenBounds.x + .5f, maxScreenBounds.x - .5f),
                                              Mathf.Clamp(invisiblePlayersTransform[i].position.y, minScreenBounds.y + .5f, maxScreenBounds.y - .5f));

            float dist = Vector2.Distance(invisiblePlayersTransform[i].position, markers[i].position);
            float markerScale = Mathf.Clamp01(1 - (dist / maxDistance));

            Vector2 v = markers[i].transform.localScale;
            v.x = markerScale * scaleMultiplier;
            v.y = markerScale * scaleMultiplier;
            markers[i].transform.localScale = v;

            if (dist > maxDistance)
            {
                Vector2 newPos = GameManager.Instance.playersByName[0].playerScript.transform.position;
                GameManager.Instance.TeleportPlayerAtCameraCenter(players[i].PlayerIndex);
            }
        }

        foreach (var item in playersToRemoveFromList)
        {
            if (invisiblePlayersTransform.Contains(item)) invisiblePlayersTransform.Remove(item);
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

        // add every alive players to the tg group
        for (int i = 0; i < pbn.Count; i++)
        {
            if (pbn[i].playerScript.IsAlive() == false) continue;

            tg_players.AddMember(pbn[i].playerScript.transform, 1, 0);
        }

        // if we want to focus on P1, and if he is alive, increase it's weight
        if (hardFocusOnP1 && pbn[0].playerScript.IsAlive())
            tg_players.m_Targets[0].weight++;
    }

    public void PlayerBecameInvisible(PlayerCharacter playerScript)
    {
        if (cinematicMode) return;
        if (invisiblePlayersTransform.Contains(playerScript.PivotOffset.transform)) return;

        invisiblePlayersTransform.Add(playerScript.PivotOffset.transform);
        players.Add(playerScript);
        int idx = invisiblePlayersTransform.Count - 1;

        GameManager.E_CharactersNames character = playerScript.GetCharacterName();

        playerScript.d_OnDeath += OnPlayerDeath;

        if (markers == null || ReferenceEquals(markers, null)) return;

        markers[idx].localScale = Vector3.one;
        markers[idx].GetComponent<SpriteRenderer>().sprite = UIManager.Instance.GetBasePortrait(character);
        markers[idx].gameObject.SetActive(true);
    }

    public void PlayerBecameVisible(PlayerCharacter playerScript)
    {
        if (cinematicMode) return;
        int indexOfPlayer = invisiblePlayersTransform.IndexOf(playerScript.PivotOffset.transform);

        if (indexOfPlayer >= markers.Length) return;
        if (indexOfPlayer < 0) return;

        markers[indexOfPlayer].gameObject.SetActive(false);
        playersToRemoveFromList.Add(playerScript.PivotOffset.transform);
        players.Remove(playerScript);
    }

    private void OnPlayerDeath()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].IsAlive()) continue;
            markers[i].gameObject.SetActive(false);
            playersToRemoveFromList.Add(invisiblePlayersTransform[i]);
            players.RemoveAt(i);
        }
    }

    public void ShakeCamera(float intensity, float duration)
    {
        bmcp.m_AmplitudeGain = intensity;
        shake_startingIntensity = intensity;
        
        shake_DURATION = duration;
        shake_TIMER = duration;
    }

    public void TeleportCamera(Vector2 pos)
    {
        cam_followPlayers.transform.position = pos;
    }

    public void MoveCamera(Vector2 pos, Action onCompleteAction, float duration = 2, LeanTweenType type = LeanTweenType.easeInOutQuart)
    {
        cinematicMode = true;
        Array.Clear(tg_players.m_Targets, 0, tg_players.m_Targets.Length);
        tg_players.m_Targets = new CinemachineTargetGroup.Target[0];
        cam_followPlayers.Follow = null;

        LeanTween.move(cam_followPlayers.gameObject, pos, duration).setEase(type).setOnComplete(onCompleteAction);
    }
    public void MoveCamera(Vector2 pos, float duration = 2, LeanTweenType type = LeanTweenType.easeInOutQuart) => MoveCamera(pos, null, duration, type);

    public void ZoomCamera(float amount, float duration, Action onCompleteAction, LeanTweenType type = LeanTweenType.easeInOutQuart)
    {
        if (cinematicMode == false) return;

        LeanTween.value(cam_followPlayers.gameObject, cam_followPlayers.m_Lens.OrthographicSize, cam_followPlayers.m_Lens.OrthographicSize + amount, duration).setEase(type).setOnUpdate((float val) =>
        {
            cam_followPlayers.m_Lens.OrthographicSize = val;
        }).setOnComplete(onCompleteAction);
    }

    public void SetMinimapToOverview(float leanTime = .5f)
    {
        ChangeMiniampCameraState(null, false, minimapCenterPosition, minimapOverviewSize, leanTime);

    }
    public void AttachMinimapCamera(float leanTime = .5f)
    {
        ChangeMiniampCameraState(this.transform, true, Vector2.zero, minimapNormalSize, leanTime);
    }
    private void ChangeMiniampCameraState(Transform parent, bool localPos, Vector2 targetPos, float targetSize, float leanTime)
    {
        if (minimapCamera == null) return;

        minimapCamera.transform.parent = parent;

        Vector3 minimapCamPos = targetPos;
        minimapCamPos.z = this.transform.position.z;

        if (localPos) LeanTween.moveLocal(minimapCamera.gameObject, minimapCamPos, leanTime).setEaseSpring();
        else LeanTween.move(minimapCamera.gameObject, minimapCamPos, leanTime).setEaseSpring();

        LeanTween.value(minimapCamera.gameObject, minimapCamera.orthographicSize, targetSize, leanTime).setOnUpdate((float val) =>
        {
            minimapCamera.orthographicSize = val;
        }).setEaseSpring();
    }

    public void SetTriggerParent(Transform newParent)
    {
        volumeTrigger.parent = newParent;
        volumeTrigger.localPosition = Vector3.zero;
    }

    public void EndCinematic()
    {
        cinematicMode = false;
        cam_followPlayers.Follow = tg_players.transform;
        SetArray();
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
