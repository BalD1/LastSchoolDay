using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour, IInteractable
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private GameObject outline;

    [SerializeField] private int interactableChances = 50;

    [SerializeField] private E_MachineStyle machineStyle;

    [SerializeField] private S_MachineAnimations blueMachineStyle;
    [SerializeField] private S_MachineAnimations yellowMachineStyle;

    [SerializeField] private SCRPT_DropTable dropTable;

    [SerializeField] private Vector2 dropPosition;

#if UNITY_EDITOR
    [SerializeField] private bool debugMode;
#endif

    private int interactorsCount;

    private S_MachineAnimations currentMachineStyle;

    [System.Serializable]
    private struct S_MachineAnimations
    {
        [SpineAnimation] public string baseState;
        [SpineAnimation] public string vendingState;
        [SpineAnimation] public string emptyState;
    }

    private enum E_MachineStyle
    {
        Blue,
        Yellow,
    }

    private bool isValid;

    private void Awake()
    {
        currentMachineStyle = machineStyle == E_MachineStyle.Blue ? blueMachineStyle : yellowMachineStyle;

        isValid = Random.Range(0, 100) <= interactableChances;

        if (!isValid)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, currentMachineStyle.emptyState, false);
            Destroy(this);
        }
    }

    public bool CanBeInteractedWith()
    {
        return isValid;
    }

    public void EnteredInRange(GameObject interactor)
    {
        if (!isValid) return;

        interactorsCount++;

        if (interactorsCount <= 1) outline.SetActive(true);
    }

    public void ExitedRange(GameObject interactor)
    {
        if (!isValid) return;

        interactorsCount--;

        if (interactorsCount <= 0) outline.SetActive(false);
    }

    public float GetDistanceFrom(Transform target)
    {
        return Vector2.Distance(this.transform.position, target.position);
    }

    public void Interact(GameObject interactor)
    {
        if (!isValid) return;

        float animDuration = skeletonAnimation.skeleton.Data.FindAnimation(currentMachineStyle.vendingState).Duration;

        skeletonAnimation.AnimationState.SetAnimation(0, currentMachineStyle.vendingState, false);

        StartCoroutine(WaitForAnimation(animDuration));

        skeletonAnimation.AnimationState.SetAnimation(0, currentMachineStyle.emptyState, false);
        outline.SetActive(false);

        isValid = false;
    }

    private IEnumerator WaitForAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);

        dropTable.DropRandom(dropPosition);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!debugMode) return;

        Gizmos.DrawWireSphere((Vector2)this.transform.position + dropPosition, 1);
#endif
    }
}
