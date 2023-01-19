using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VendingMachine : MonoBehaviour, IInteractable
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private GameObject outline;

    [SerializeField] private int interactableChances = 50;

    [SerializeField] private int price = 25;

    [SerializeField] private E_MachineStyle machineStyle;

    [SerializeField] private S_MachineAnimations blueMachineStyle;
    [SerializeField] private S_MachineAnimations yellowMachineStyle;

    [SerializeField] private SCRPT_DropTable dropTable;

    [SerializeField] private Vector2 dropPosition;

    [SerializeField] private GameObject priceComponents;

    [SerializeField] private Animator animator;

    [SerializeField] private TextMeshPro priceText;

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
            return;
        }

        priceText.text = price.ToString();
    }

    public bool CanBeInteractedWith()
    {
        return isValid;
    }

    public void EnteredInRange(GameObject interactor)
    {
        if (interactor == null || !isValid) return;

        interactorsCount++;

        if (interactorsCount > 1) return;

        outline.SetActive(true);
        priceComponents.SetActive(true);
    }

    public void ExitedRange(GameObject interactor)
    {
        if (interactor == null || !isValid) return;

        interactorsCount--;

        if (interactorsCount > 0) return;

        outline.SetActive(false);
        priceComponents.SetActive(false);
    }

    public float GetDistanceFrom(Transform target)
    {
        return Vector2.Distance(this.transform.position, target.position);
    }

    public void Interact(GameObject interactor)
    {
        if (!isValid) return;

        if (PlayerCharacter.HasEnoughMoney(price) == false)
        {
            animator.SetTrigger("shake");
            return;
        }

        PlayerCharacter.RemoveMoney(price, false);

        float animDuration = skeletonAnimation.skeleton.Data.FindAnimation(currentMachineStyle.vendingState).Duration;

        skeletonAnimation.AnimationState.SetAnimation(0, currentMachineStyle.vendingState, false);

        StartCoroutine(WaitForAnimation(animDuration));

        skeletonAnimation.AnimationState.AddAnimation(0, currentMachineStyle.emptyState, false, .25f);
        outline.SetActive(false);

        isValid = false;
    }

    private IEnumerator WaitForAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);

        dropTable.DropRandom((Vector2)this.transform.position + dropPosition);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!debugMode) return;

        Gizmos.DrawWireSphere((Vector2)this.transform.position + dropPosition, 1);
#endif
    }
}
