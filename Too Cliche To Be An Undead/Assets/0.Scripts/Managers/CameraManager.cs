using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private CinemachineVirtualCamera cam_followPlayers;
    [SerializeField] private CinemachineTargetGroup tg_players;
    private CinemachineBasicMultiChannelPerlin bmcp;

    [field: SerializeField] public Camera minimapCamera { get; private set; }
    [field: SerializeField] public Vector2 minimapCenterPosition { get; private set; }
    [field: SerializeField] public float minimapOverviewSize { get; private set; }
    [field: SerializeField] public float minimapNormalSize { get; private set; }

    [SerializeField] private Transform volumeTrigger;

    [SerializeField] private Transform customMovementTarget;

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

    private List<PlayerCharacter> invisiblePlayers = new List<PlayerCharacter>();

    private bool cinematicMode = false;

    protected override void EventsSubscriber()
    {
        FSM_Player_Events.OnEnteredDeath += OnPlayerDeath;
        IGPlayersManagerEvents.OnPlayerCreated += AddPlayerToArray;
        CinematicManagerEvents.OnChangeCinematicState += OnCinematicStateChange;
    }

    protected override void EventsUnSubscriber()
    {
        FSM_Player_Events.OnEnteredDeath -= OnPlayerDeath;
        IGPlayersManagerEvents.OnPlayerCreated -= AddPlayerToArray;
        CinematicManagerEvents.OnChangeCinematicState -= OnCinematicStateChange;
    }

    protected override void Awake()
    {
        base.Awake();
        bmcp = cam_followPlayers.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    protected override void Start()
    {
        base.Start();
        mainCam = this.GetComponent<Camera>();
        AttachMinimapCamera();
    }

    private void Update()
    {
        if (shake_TIMER > 0)
        {
            shake_TIMER -= Time.unscaledDeltaTime;
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
                GameManager.Instance.TeleportPlayerAtCameraCenter(invisiblePlayers[i].PlayerIndex);
            }
        }

        foreach (var item in playersToRemoveFromList)
        {
            if (invisiblePlayersTransform.Contains(item)) invisiblePlayersTransform.Remove(item);
        }
        playersToRemoveFromList.Clear();
    }

    private void OnCinematicStateChange(bool isInCinematic)
    {
        this.cinematicMode = isInCinematic;

        if (!isInCinematic)
        {
            cam_followPlayers.Follow = tg_players.transform;
        }
        else
        {
            cam_followPlayers.Follow = null;
        }
    }

    private void AddPlayerToArray(PlayerCharacter player)
    {
        tg_players.AddMember(player.transform, 1, 0);
    }

    private void OnPlayerDeath(PlayerCharacter player)
    {
        int playerIdx = invisiblePlayers.IndexOf(player);
        markers[playerIdx].gameObject.SetActive(false);
        playersToRemoveFromList.Add(invisiblePlayersTransform[playerIdx]);
        invisiblePlayers.RemoveAt(playerIdx);
        this.TG_Players.RemoveMember(player.transform);
    }

    public void ShakeCamera(float intensity, float duration)
    {
        bmcp.m_AmplitudeGain = intensity;
        shake_startingIntensity = intensity;
        
        shake_DURATION = duration;
        shake_TIMER = duration;
    }

    public LTDescr MoveCamera(Vector2 pos, Action onCompleteAction, float duration = 2, LeanTweenType type = LeanTweenType.easeInOutQuart)
    {
        cinematicMode = true;
        cam_followPlayers.Follow = customMovementTarget;

        return LeanTween.move(customMovementTarget.gameObject, pos, duration).setEase(type).setOnComplete(onCompleteAction);
    }

    public void ZoomCamera(float amount, float duration, Action onCompleteAction, LeanTweenType type = LeanTweenType.easeInOutQuart)
    {
        if (cinematicMode == false) return;

        LeanTween.value(cam_followPlayers.gameObject, cam_followPlayers.m_Lens.OrthographicSize, cam_followPlayers.m_Lens.OrthographicSize - amount, duration).setEase(type).setOnUpdate((float val) =>
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

    public void EndCinematic()
    {
        cinematicMode = false;
        cam_followPlayers.Follow = tg_players.transform;
    }
}
