using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseController))]
public class BaseAnimController : MonoBehaviour
{
    private BaseController controller;

    private SkeletonAnimation skeletonAnimation;

    public AnimationReferenceAsset idleAnim;
    public AnimationReferenceAsset walkAnim;
    public AnimationReferenceAsset attackAnim;

    private bool isWalking = false;
    private bool isAttacking = false;

    private void Awake()
    {
        controller = this.GetComponent<BaseController>();
        skeletonAnimation = this.GetComponent<SkeletonAnimation>();
        if (skeletonAnimation == null) skeletonAnimation = this.GetComponentInChildren<SkeletonAnimation>();
        skeletonAnimation.AnimationState.SetAnimation(0, idleAnim, true);
    }

    private void Update()
    {
        if (isAttacking) return;
        if (controller.body.velocity != Vector2.zero)
        {
            if (!isWalking)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, walkAnim, true);
                isWalking = true;
            }
            Vector3 s = skeletonAnimation.transform.localScale;
            s.x = controller.horizontal > 0 ? 1 : -1;
            skeletonAnimation.transform.localScale = s;
        }
        else if (controller.body.velocity == Vector2.zero)
        {
            if (isWalking)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, idleAnim, true);
                isWalking = false;
            }
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 s = skeletonAnimation.transform.localScale;
            s.x = mousePos.x > this.transform.position.x ? 1 : -1;
            skeletonAnimation.transform.localScale = s;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isAttacking = true;
            controller.canMove = false;
            controller.body.velocity = Vector2.zero;
            skeletonAnimation.AnimationState.SetAnimation(0, attackAnim, false);
            LeanTween.delayedCall(attackAnim.Animation.Duration, () =>
            {
                skeletonAnimation.AnimationState.SetAnimation(0, idleAnim, true);
                isAttacking = false;
                controller.canMove = true;
            });
        }
    }
}
