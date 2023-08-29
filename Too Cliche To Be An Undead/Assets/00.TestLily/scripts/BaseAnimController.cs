using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimController : MonoBehaviour
{
    public BaseController controller;

    public SkeletonAnimation skeletonAnimation;

    public string idleAnim;
    [SpineAnimation]
    public string walkAnim;
    public AnimationReferenceAsset attackAnim;

    private bool isWalking = false;
    private bool isAttacking = false;

    private void Awake()
    {
        if (skeletonAnimation == null) skeletonAnimation = this.GetComponent<SkeletonAnimation>();
        if (skeletonAnimation == null) skeletonAnimation = this.GetComponentInChildren<SkeletonAnimation>();
        if (skeletonAnimation == null) return;
        skeletonAnimation.AnimationState.SetAnimation(0, idleAnim, true);
    }

    private void Update()
    {
        if (isAttacking) return;
        if (controller.body.velocity != Vector2.zero)
        {
            if (!isWalking)
            {
                skeletonAnimation?.AnimationState.SetAnimation(0, walkAnim, true);
                isWalking = true;
            }
            Vector3 s = this.transform.localScale;
            s.x = controller.horizontal > 0 ? 1 : -1;
            this.transform.localScale = s;
        }
        else if (controller.body.velocity == Vector2.zero)
        {
            if (isWalking)
            {
                skeletonAnimation?.AnimationState.SetAnimation(0, idleAnim, true);
                isWalking = false;
            }
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 s = this.transform.localScale;
            s.x = mousePos.x > this.transform.position.x ? 1 : -1;
            this.transform.localScale = s;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isAttacking = true;
            controller.canMove = false;
            controller.body.velocity = Vector2.zero;
            skeletonAnimation?.AnimationState.SetAnimation(0, attackAnim, false);
            LeanTween.delayedCall(skeletonAnimation == null ? 1 : attackAnim.Animation.Duration, () =>
            {
                skeletonAnimation?.AnimationState.SetAnimation(0, idleAnim, true);
                isAttacking = false;
                controller.canMove = true;
            });
        }
    }
}
