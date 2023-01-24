using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[ExecuteInEditMode]
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

    public Vector2 GetRandomPositionInBounds { get => new Vector2(Random.Range(BoundsMinPosition.x, BoundsMaxPosition.x),
                                                                  Random.Range(BoundsMinPosition.y, BoundsMaxPosition.y));}

    [field: SerializeField] public bool isValid { get; private set; }

    private void Awake()
    {
        isValid = true;
        InvokeRepeating(nameof(CheckValidness), 0, 2);
    }

    private void CheckValidness()
    {
        float distanceFromPlayer = Vector2.Distance(this.transform.position, GameManager.Player1Ref.transform.position);
        isValid = distanceFromPlayer >= SpawnersManager.minValidDistanceFromPlayer && distanceFromPlayer <= SpawnersManager.maxValidDistanceFromPlayer;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!symetrical) return;

        if (minBounds.x != maxBounds.x) minBounds.x = maxBounds.x * -1;
        if (minBounds.y != maxBounds.y) minBounds.y = maxBounds.y * -1; 
#endif
    }
    public void SpawnObject(int count = 1, GameObject objectOverride = null)
    {
        if (!isValid) return;

        for (int i = 0; i < count; i++)
        {
            GameObject objectToSpawn = objectOverride == null ? GetRandomObjectInPool : objectOverride;

            Instantiate(objectToSpawn, GetRandomPositionInBounds, Quaternion.identity);
        }
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

        // bottom
        var p1 = new Vector3(BoundsMaxPosition.x, BoundsMinPosition.y);
        var p2 = new Vector3(BoundsMinPosition.x, BoundsMinPosition.y);

        Debug.DrawLine(p1, p2, Color.yellow);

        // top
        var p3 = new Vector3(BoundsMaxPosition.x, BoundsMaxPosition.y);
        var p4 = new Vector3(BoundsMinPosition.x, BoundsMaxPosition.y);

        Debug.DrawLine(p3, p4, Color.yellow);

        // sides
        Debug.DrawLine(p1, p3, Color.green);
        Debug.DrawLine(p2, p4, Color.cyan);
    }
}
