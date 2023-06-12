using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Locker : MonoBehaviour, IInteractable
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [SerializeField] private int openableChance = 50;

    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioSource source;

    [System.Serializable]
    private struct S_Animations
    {
        [SpineAnimation] public string closedIdle;
        [SpineAnimation] public string[] openAnim;

        public string GetRandomOpenAnim() => openAnim[Random.Range(0, openAnim.Length)];
    }

    [SerializeField] private GameObject sparkles;
    [SerializeField] private GameObject outline;

    private int interactorsCount = 0;

    [SerializeField] private S_Animations bloodyLocker;
    [SerializeField] private S_Animations normalLocker;

    [SerializeField] private Transform lootSpawnPoint;

    [SerializeField] private SCRPT_DropTable dropTable;

    private bool isOpen = false;

    private bool isBloody = false;

    private void Start()
    {
        bool isOpenable = Random.Range(0, 100) <= openableChance;

        if (!isOpenable)
        {
            isBloody = Random.Range(0, 2) == 0;
            bool isOpen = Random.Range(0, 2) == 0;

            if (isOpen) skeletonAnimation.AnimationState.SetAnimation(0, normalLocker.GetRandomOpenAnim(), false);
            else if (!isOpen && isBloody) skeletonAnimation.AnimationState.SetAnimation(0, bloodyLocker.closedIdle, false);
            else if (!isOpen && !isBloody) skeletonAnimation.AnimationState.SetAnimation(0, normalLocker.closedIdle, false);

            Destroy(sparkles);

            Destroy(this);
            return;
        }

        isBloody = Random.Range(0, 2) == 0;
        if (isBloody) skeletonAnimation.AnimationState.SetAnimation(0, bloodyLocker.closedIdle, false);
    }

    public void EnteredInRange(GameObject interactor)
    {
        if (interactor == null && !isOpen) return;

        interactorsCount++;

        if (interactorsCount <= 1) outline.SetActive(true);
    }

    public void ExitedRange(GameObject interactor)
    {
        if (interactor == null && !isOpen) return;

        interactorsCount--;

        if (interactorsCount <= 0) outline.SetActive(false);
    }

    public void Interact(GameObject interactor)
    {
        isOpen = true;

        dropTable.DropRandom(lootSpawnPoint.position);

        if (isBloody) skeletonAnimation.AnimationState.SetAnimation(0, bloodyLocker.GetRandomOpenAnim(), false);
        else skeletonAnimation.AnimationState.SetAnimation(0, normalLocker.GetRandomOpenAnim(), false);

        sparkles.SetActive(false);

        outline.SetActive(false);

        source.PlayOneShot(openSound);

        Destroy(this);
    }

    public float GetDistanceFrom(Transform target)
    {
        if (target != null && this.transform != null)
        return Vector2.Distance(this.transform.position, target.position);

        return -1;
    }

    public bool CanBeInteractedWith()
    {
        return !isOpen;
    }
}
