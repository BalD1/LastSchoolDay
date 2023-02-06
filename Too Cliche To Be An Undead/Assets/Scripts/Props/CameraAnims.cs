using Spine.Unity;
using UnityEngine;

public class CameraAnims : MonoBehaviour
{
    [SerializeField] [SpineAnimation] private string rightAnim, leftAnim, frontAnim;

    [SerializeField] private SkeletonAnimation skeleton;

    [SerializeField] private float xPosThreshold;

    private string animToPlay;

    private bool isVisible;

    private void Update()
    {
        if (!isVisible) return;

        CheckAnimToPlay();
        SetAnimation();
    }

    private void CheckAnimToPlay()
    {
        float playerPosX = GameManager.Player1Ref.transform.position.x;
        float selfPosX = this.transform.position.x;

        if (selfPosX - xPosThreshold > playerPosX) animToPlay = leftAnim;
        else if (selfPosX + xPosThreshold < playerPosX) animToPlay = rightAnim;
        else animToPlay = frontAnim;
    }

    private void SetAnimation()
    {
        if (skeleton.AnimationState.GetCurrent(0).Animation.Name == animToPlay) return;

        skeleton.AnimationState.SetAnimation(0, animToPlay, false);
    }

    private void OnBecameVisible() => isVisible = true;

    private void OnBecameInvisible() => isVisible = false;
}
