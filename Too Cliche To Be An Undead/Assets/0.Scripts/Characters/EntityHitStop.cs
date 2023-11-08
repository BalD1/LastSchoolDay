using System.Collections;
using UnityEngine;

public class EntityHitStop : MonoBehaviour
{
    [SerializeField] protected Entity owner;

    [SerializeField] protected float stopTime = .10f;

    private Coroutine hitStopCoroutine;

    private void Reset()
    {
        owner = this.GetComponentInParent<Entity>();
    }

    protected virtual void Awake()
    {
        //owner.OnPushed += OnPushed;
        //owner.OnReset += OnReset;
    }

    protected virtual void OnReset()
    {
        if (hitStopCoroutine != null)
        {
            StopCoroutine(hitStopCoroutine);
        }
    }

    protected virtual void OnPushed()
    {
        if (this.gameObject.activeInHierarchy)
            hitStopCoroutine = StartCoroutine(HitStop());
    }

    protected virtual IEnumerator HitStop()
    {
        //owner.SkeletonAnimation.AnimationState.TimeScale = 0;
        yield return new WaitForSeconds(stopTime);
        //owner.SkeletonAnimation.AnimationState.TimeScale = 1;
    }
}
