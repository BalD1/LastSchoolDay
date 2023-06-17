using Spine.Unity;
using UnityEngine;

public class CameraAnims : MonoBehaviour
{
    [SerializeField] [SpineAnimation] private string rightAnim, leftAnim, frontAnim;

    [SerializeField] private SkeletonAnimation skeleton;

    [SerializeField] private float xPosThreshold;

    private string animToPlay;

    private bool isVisible;

    private Transform p1Transform;

    private void Start()
    {
        if (PlayerInputsManager.P1Inputs == null)
        {
            PlayerInputsEvents.OnPlayerInputsCreated += WaitForP1Created;
            return;
        } 
        p1Transform = PlayerInputsManager.P1Inputs.transform;
    }

    private void WaitForP1Created(PlayerInputs inputs)
    {
        PlayerInputsEvents.OnPlayerInputsCreated -= WaitForP1Created;
        p1Transform = inputs.transform;
    }

    private void Update()
    {
        if (!isVisible) return;
        if (p1Transform == null) return;

        CheckAnimToPlay();
        SetAnimation();
    }

    private void CheckAnimToPlay()
    {
        float playerPosX = p1Transform.position.x;
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

    private void OnDestroy()
    {
        PlayerInputsEvents.OnPlayerInputsCreated -= WaitForP1Created;
    }
}
