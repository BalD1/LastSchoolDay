using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedZombie : Entity
{
    [SerializeField] private float stunDuration = 1.0f;

    [SerializeField][SpineAnimation] private string idleAnim;
    [SerializeField][SpineAnimation] private string attackAnim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attack_TIMER > 0) return;

        PlayerCharacter player = collision.gameObject.GetComponentInParent<PlayerCharacter>();

        if (player == null) return;
        if (player.StateManager.ToString() == "Pushed") return;

        SkeletonAnimation.AnimationState.SetAnimation(0, attackAnim, false);
        SkeletonAnimation.AnimationState.AddAnimation(0, idleAnim, true, .25f);

        player.Stun(stunDuration, false, true);
        player.OnTakeDamages(MaxDamages_M, this, RollCrit());

        attack_TIMER = MaxAttCD_M;
    }
}
