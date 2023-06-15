using UnityEditor;
using UnityEngine;

public class AreaSpawner : MonoBehaviourEventsHandler, IDistanceChecker
{
#if UNITY_EDITOR
	public bool debugMode;

    [SerializeField] private bool symetrical = true;

    public bool Symetrical { get => symetrical; }
#endif

    [SerializeField] public Vector2 minBounds;
	[SerializeField] public Vector2 maxBounds;
    [SerializeField] public Vector2 centerOffset;

    [SerializeField] private GameObject[] objectsPoolToSpawn;
    public GameObject GetRandomObjectInPool { get => objectsPoolToSpawn[Random.Range(0, objectsPoolToSpawn.Length)]; }

    [SerializeField] private BoxCollider2D boxCollider;

    public Vector2 BoundsMinPosition { get => (Vector2)this.transform.position + minBounds + centerOffset; }
    public Vector2 BoundsMaxPosition { get => (Vector2)this.transform.position + maxBounds + centerOffset; }

    [SerializeField] private bool isValid;
    public bool IsValid { get => isValid; }

    private void Reset()
    {
        boxCollider = this.GetComponent<BoxCollider2D>();
    }

    protected override void EventsSubscriber()
    {
        HUBDoorEventHandler.OnInteractedWithDoor += ResetTrigger;
    }

    protected override void EventsUnSubscriber()
    {
        HUBDoorEventHandler.OnInteractedWithDoor -= ResetTrigger;
    }

    protected override void Awake()
    {
        base.Awake();
        if (SpawnersManager.Instance == null) Destroy(this.gameObject);
        isValid = false;
    }

    private void ResetTrigger()
    {
        this.boxCollider.enabled = false;
        this.boxCollider.enabled = true;
    }

    public void SetValidity(bool validity)
    {
        isValid = validity;

        if (isValid) SpawnersManager.Instance.validAreaSpawners.Add(this);
        else SpawnersManager.Instance.validAreaSpawners.Remove(this);
    }

    public Vector2 GetRandomPositionInBounds()
    {
        Vector2 minPos = BoundsMinPosition;
        Vector2 maxPos = BoundsMaxPosition;

        return new Vector2(x: Random.Range(minPos.x, maxPos.x),
                           y: Random.Range(minPos.y, maxPos.y));
    }

    public void SpawnObject(int count = 1)
    {
        if (!isValid) return;

        for (int i = 0; i < count; i++)
        {
            SpawnersManager.GetNextInPool().Reenable(GetRandomPositionInBounds());
        }
    }

    public void TeleportZombieHere(NormalZombie zom)
    {
        zom.Reenable(GetRandomPositionInBounds(), false);
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.update += UpdateBounds; 
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= UpdateBounds; 
#endif
    }

    private void UpdateBounds()
    {
#if UNITY_EDITOR
        if (boxCollider == null) boxCollider = this.GetComponent<BoxCollider2D>();

        boxCollider.size = maxBounds * 2;

        if (!symetrical) return;

        if (minBounds.x != maxBounds.x) minBounds.x = maxBounds.x * -1;
        if (minBounds.y != maxBounds.y) minBounds.y = maxBounds.y * -1; 
#endif
    }

    private void OnDrawGizmos()
	{
#if UNITY_EDITOR
		if (!debugMode) return;
        UpdateBounds();
        Gizmos.color = Color.red;
        DrawBounds();
#endif
    }

    private void DrawBounds()
    {
        Vector2 selfPos = this.transform.position;

        Color c = isValid ? Color.green : Color.red;

        // bottom
        var p1 = new Vector3(BoundsMaxPosition.x, BoundsMinPosition.y);
        var p2 = new Vector3(BoundsMinPosition.x, BoundsMinPosition.y);

        Debug.DrawLine(p1, p2, c);

        // top
        var p3 = new Vector3(BoundsMaxPosition.x, BoundsMaxPosition.y);
        var p4 = new Vector3(BoundsMinPosition.x, BoundsMaxPosition.y);

        Debug.DrawLine(p3, p4, c);

        // sides
        Debug.DrawLine(p1, p3, c);
        Debug.DrawLine(p2, p4, c);
    }

    public void OnEnteredFarCheck() => SetValidity(true);
    public void OnExitedFarCheck() => SetValidity(false);

    public void OnEnteredCloseCheck() => SetValidity(false);
    public void OnExitedCloseCheck() => SetValidity(true);
}
