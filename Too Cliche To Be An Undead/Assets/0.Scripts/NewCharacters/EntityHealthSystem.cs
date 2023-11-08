using System.Collections;
using UnityEngine;

public class EntityHealthSystem : HealthSystem
{
    [SerializeField] private float onHitFlashTime = .1f;
    private bool isFlashing = false;

    private AnimationControllerBase ownerAnimationController;

    public override void InflictDamages(INewDamageable.DamagesData damagesData)
    {
        base.InflictDamages(damagesData);
        if (!isFlashing) StartCoroutine(FlashSkeleton());
    }

    private IEnumerator FlashSkeleton()
    {
        isFlashing = true;
        ownerAnimationController.SetSkeletonColor(Color.red);
        yield return new WaitForSeconds(onHitFlashTime);
        ownerAnimationController.SetSkeletonColor(Color.white);
        isFlashing = false;
    }
}
