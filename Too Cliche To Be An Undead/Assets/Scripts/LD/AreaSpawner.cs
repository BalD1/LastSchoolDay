using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AreaSpawner : MonoBehaviour
{
#if UNITY_EDITOR
	[SerializeField] private bool debugMode;
#endif

	[SerializeField] private Vector2 minBounds;
	[SerializeField] private Vector2 maxBounds;
    [SerializeField] private Vector2 centerOffset;

#if UNITY_EDITOR
    [SerializeField] private bool symetrical = true;

    public bool Symetrical { get => symetrical; }
#endif

    public Vector2 BoundsMinPosition { get => (Vector2)this.transform.position + minBounds + centerOffset; }
    public Vector2 BoundsMaxPosition { get => (Vector2)this.transform.position + maxBounds + centerOffset; }

    private void Update()
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

    void DrawBounds()
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
