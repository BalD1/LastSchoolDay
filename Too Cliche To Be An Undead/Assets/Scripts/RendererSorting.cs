using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RendererSorting : MonoBehaviour
{
    private const int sortingOrderBase = 5000;
    [SerializeField] private int sortingOrderOffset = 0;

    [SerializeField] private float pivotOffset = 0;

    [SerializeField] private bool isStatic = false;

    [SerializeField] private Renderer objectRenderer;

    [SerializeField, ReadOnly] private bool isVisible;

#if UNITY_EDITOR
    [SerializeField] private bool debugMode = false; 
#endif

    private float update_TIMER;
    private const float update_COOLDOWN = .1f;

    private const int yOrderPrecision = 1000;

    private void Reset()
    {
        objectRenderer = this.GetComponent<Renderer>();

        isStatic = false;
        pivotOffset = 0;
        sortingOrderOffset = 0;
    }

    private void Awake()
    {
        if (objectRenderer == null)
            objectRenderer = this.GetComponent<Renderer>();

        if (objectRenderer == null)
        {
            objectRenderer = this.GetComponent<Renderer>();

            if (objectRenderer != null) return;

#if !UNITY_EDITOR
            Destroy(this);
#else
            Debug.LogErrorFormat($"Object Renderer {objectRenderer} of {this.gameObject.name} is null or invalid.", this.gameObject);
#endif
            return;
        }
    }

    private void LateUpdate()
    {
        if (!isVisible && !isStatic) return;

        update_TIMER -= Time.deltaTime;
        if (update_TIMER <= 0)
        {
            update_TIMER = update_COOLDOWN;
            objectRenderer.sortingOrder = (int)((sortingOrderBase + sortingOrderOffset) - (this.transform.position.y * yOrderPrecision) + (pivotOffset * yOrderPrecision));
#if !UNITY_EDITOR
            if (isStatic) Destroy(this); 
#endif
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!debugMode) return;

        Vector2 positionWithOffset = this.transform.position;
        positionWithOffset.y -= pivotOffset;
        Gizmos.DrawWireSphere(positionWithOffset, 1);
    }
#endif

    private void OnBecameVisible()
    {
        isVisible = true;
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }
}
