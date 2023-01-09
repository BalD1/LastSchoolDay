using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Locker : MonoBehaviour, IInteractable
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [System.Serializable]
    private struct S_Animations
    {
        [SpineAnimation] public string closedIdle;
        [SpineAnimation] public string[] openAnim;

        public string GetRandomOpenAnim() => openAnim[Random.Range(0, openAnim.Length)];
    }

    [SerializeField] private GameObject sparkles;

    [SerializeField] private S_Animations bloodyLocker;
    [SerializeField] private S_Animations normalLocker;

    [SerializeField] private Transform lootSpawnPoint;

    [SerializeField] private SCRPT_DropTable dropTable;

    private bool isOpen = false;

    private bool isBloody = false;

    private void Start()
    {
        isBloody = Random.Range(0, 2) == 0;
        if (isBloody) skeletonAnimation.AnimationState.SetAnimation(0, bloodyLocker.closedIdle, false);
    }

    public void EnteredInRange(GameObject interactor)
    {
    }

    public void ExitedRange(GameObject interactor)
    {
    }

    public void Interact(GameObject interactor)
    {
        isOpen = true;

        dropTable.DropRandom(lootSpawnPoint.position);

        if (isBloody) skeletonAnimation.AnimationState.SetAnimation(0, bloodyLocker.GetRandomOpenAnim(), false);
        else skeletonAnimation.AnimationState.SetAnimation(0, normalLocker.GetRandomOpenAnim(), false);

        sparkles.SetActive(false);
    }

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);

    public bool CanBeInteractedWith()
    {
        return !isOpen;
    }
}
