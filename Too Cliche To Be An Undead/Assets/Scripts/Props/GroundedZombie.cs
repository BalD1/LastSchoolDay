using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedZombie : MonoBehaviour
{
    [SerializeField] private int damagesAmount = 10;
    [SerializeField] private float stunDuration = 1.0f;

    [SerializeField] private float attack_COOLDOWN = 3f;
    private float attack_TIMER;

    [SerializeField] private SkeletonAnimation skeleton;

    [SerializeField][SpineAnimation] private string idleAnim;
    [SerializeField][SpineAnimation] private string attackAnim;

    private void Update()
    {
        if (attack_TIMER > 0) attack_TIMER -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attack_TIMER > 0) return;

        PlayerCharacter player = collision.gameObject.GetComponentInParent<PlayerCharacter>();

        if (player == null) return;
        if (player.StateManager.ToString() == "Pushed") return;

        skeleton.AnimationState.SetAnimation(0, attackAnim, false);
        skeleton.AnimationState.AddAnimation(0, idleAnim, true, .25f);

        player.Stun(stunDuration, false, true);
        player.OnTakeDamages(damagesAmount);

        attack_TIMER = attack_COOLDOWN;
    }
}
