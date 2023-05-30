using Spine.Unity;
using UnityEngine;

public class YardDoor : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [SerializeField] [SpineAnimation] private string openAnimation;

    [SerializeField] private BoxCollider2D blockCollision;

    [SerializeField] private bool destroyScriptOnOpen = true;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCharacter player = collision.GetComponent<PlayerCharacter>();
        if (player == null) return;
        if (player.StateManager.ToString() == "Dying") return;

        OpenDoor();
    }

    public virtual void OpenDoor()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, openAnimation, false);

        if (blockCollision != null) blockCollision.enabled = false;

        Destroy(this);
    }
}
