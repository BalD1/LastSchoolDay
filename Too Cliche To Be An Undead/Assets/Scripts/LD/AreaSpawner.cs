using UnityEditor;
using UnityEngine;

public class AreaSpawner : MonoBehaviour
{
#if UNITY_EDITOR
	public bool debugMode;

    [SerializeField] private bool symetrical = true;

    public bool Symetrical { get => symetrical; }
#endif

    [SerializeField] private Vector2 minBounds;
	[SerializeField] private Vector2 maxBounds;
    [SerializeField] private Vector2 centerOffset;

    [SerializeField] private GameObject[] objectsPoolToSpawn;
    public GameObject GetRandomObjectInPool { get => objectsPoolToSpawn[Random.Range(0, objectsPoolToSpawn.Length)]; }

    public Vector2 BoundsMinPosition { get => (Vector2)this.transform.position + minBounds + centerOffset; }
    public Vector2 BoundsMaxPosition { get => (Vector2)this.transform.position + maxBounds + centerOffset; }

    [SerializeField] private bool isValid;
    public bool IsValid { get => isValid; }

    private void Awake()
    {
        isValid = true;
        InvokeRepeating(nameof(CheckValidness), 0, 2);
    }

    private void CheckValidness()
    {
        float distanceFromPlayer = Vector2.Distance(this.transform.position, GameManager.Player1Ref.transform.position);

        bool pastValid = isValid;

        isValid = distanceFromPlayer >= SpawnersManager.minValidDistanceFromPlayer && distanceFromPlayer <= SpawnersManager.maxValidDistanceFromPlayer;

        if (pastValid != isValid)
        {
            if (isValid) SpawnersManager.Instance.validAreaSpawners.Add(this);
            else SpawnersManager.Instance.validAreaSpawners.Remove(this);
        }
    }

    public Vector2 GetRandomPositionInBounds()
    {
        Vector2 minPos = BoundsMinPosition;
        Vector2 maxPos = BoundsMaxPosition;

        return new Vector2(x: Random.Range(minPos.x, maxPos.x),
                           y: Random.Range(minPos.y, minPos.y));
    }

    public void SpawnObject(int count = 1, GameObject objectOverride = null)
    {
        if (!isValid) return;

        for (int i = 0; i < count; i++)
        {
            GameObject objectToSpawn = objectOverride == null ? GetRandomObjectInPool : objectOverride;

            Instantiate(objectToSpawn, GetRandomPositionInBounds(), Quaternion.identity);
        }
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
        if (!symetrical) return;

        if (minBounds.x != maxBounds.x) minBounds.x = maxBounds.x * -1;
        if (minBounds.y != maxBounds.y) minBounds.y = maxBounds.y * -1; 
#endif
    }


    private void OnDrawGizmos()
	{
#if UNITY_EDITOR
		if (!debugMode) return;
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
}
