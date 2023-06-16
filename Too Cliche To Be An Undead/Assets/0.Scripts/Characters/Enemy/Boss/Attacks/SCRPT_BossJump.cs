using Spine;
using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "BossJump", menuName = "Scriptable/Entity/Enemy/Boss/Jump Attack")]
public class SCRPT_BossJump : SCRPT_EnemyAttack
{
    private BossZombie boss;

    [SerializeField] private float attackRange = 2;

    [SerializeField] private float jumpDistance;
    [SerializeField] private float jumpSpeed;

    [SerializeField] private AnimationReferenceAsset bossJumpAnim;
    [SerializeField] private AnimationReferenceAsset bossLandAnim;

    [SerializeField] private SCRPT_EntityAudio.S_AudioClips audioClip;

    private float baseSkeletonPositionY;

    private float delayBeforeFade = .75f;
    private float fadeDuration = .25f;

    private float delayBeforeLandAnim = .8f;

    public override void OnStart(EnemyBase owner)
    {
        boss = owner as BossZombie;
        boss.CallJumpStarted();
        SCRPT_EnemyAttack currentAttack = boss.CurrentAttack.attack;
        baseSkeletonPositionY = boss.SkeletonHolder.transform.localPosition.y;

        owner.SkeletonAnimation.AnimationState.SetAnimation(0, bossJumpAnim, false);
        // Wait for the jump anim to play 
        LeanTween.delayedCall(bossJumpAnim.Animation.Duration / 2, () =>
        {
            // Set goal color to faded out
            Skeleton bossSkeleton = boss.SkeletonAnimation.skeleton;
            Color goalColor = bossSkeleton.GetColor();
            goalColor.a = 0;

            // Wait before fading out
            LeanTween.delayedCall(delayBeforeFade, () =>
            {
                // fade out, so the player can't see the boss teleporting
                LeanTween.value(boss.SkeletonAnimation.gameObject, bossSkeleton.GetColor(), goalColor, fadeDuration).setOnUpdate((Color c) =>
                {
                    bossSkeleton.SetColor(c);
                });
            });

            // Fake the boss jump, simply move his skeleton
            LeanTween.moveLocalY(boss.SkeletonHolder.gameObject, jumpDistance, jumpSpeed).setOnComplete(() =>
            {
                // teleport the boss on top of target after the fake jump
                boss.transform.position = boss.CurrentPositionTarget;

                // wait before playing the land anim
                if (owner.IsAlive())
                {
                    LeanTween.delayedCall(delayBeforeLandAnim, () =>
                    {
                        owner.SkeletonAnimation.AnimationState.SetAnimation(0, bossLandAnim, false);
                    });
                }

                // fade in
                goalColor.a = 1;
                LeanTween.value(boss.SkeletonAnimation.gameObject, bossSkeleton.GetColor(), goalColor, fadeDuration).setOnUpdate((Color c) =>
                {
                    bossSkeleton.SetColor(c);
                });

                // fake landing
                LeanTween.moveLocalY(boss.SkeletonHolder.gameObject, baseSkeletonPositionY, jumpSpeed).setOnComplete(() =>
                {
                    // force reset position and color, safeguard to offsets
                    boss.SkeletonHolder.transform.SetLocalPositionY(0);
                    bossSkeleton.SetColor(goalColor);

                    CameraManager.Instance.ShakeCamera(2.5f, 1);

                    owner.GetAudioSource.pitch = Random.Range(1 - audioClip.pitchRange, 1 + audioClip.pitchRange);
                    owner.GetAudioSource.PlayOneShot(audioClip.clip);
                    owner.GetAudioSource.pitch = 1;

                    LogsManager.Log(this.GetType(), "Jump Ended", Time.timeSinceLevelLoad, owner.gameObject);
                    boss.CallJumpEnded();

                    if (owner.IsAlive())
                    {
                        // get target in range and damage them
                        Collider2D[] hitEntities = Physics2D.OverlapCircleAll((Vector2)owner.attackTelegraph.transform.position, attackRange, entitiesToAffect);

                        foreach (var item in hitEntities)
                        {
                            Entity e = item.GetComponent<Entity>();

                            if (e == null) continue;

                            boss.OnHitEntity(e, false);
                        }

                        if (hitEntities.Length > 0) boss.HitStop(boss.HitStop_DURATION);
                    }
                });

                if (owner.IsAlive())
                {
                    Vector2 telegraphSize = currentAttack.telegraphVectorSize != Vector2.zero ? currentAttack.telegraphVectorSize : new Vector2(currentAttack.AttackDistance, currentAttack.AttackDistance);

                    owner.attackTelegraph.Setup(telegraphSize, currentAttack.attackOffset, Quaternion.identity, currentAttack.telegraphSprite, 1);
                }
            });
        });
    }

    public override void OnUpdate(EnemyBase owner)
    {
    }

    public override void OnExit(EnemyBase owner)
    {
        owner.GetRb.velocity = Vector2.zero;
        owner.StartAttackTimer(0, true);
        if (boss != null)
        boss.attackStarted = false;
    }
}