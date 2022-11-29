using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GymnasiumDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;

    private bool canBeInteracted = true;


    public bool CanBeInteractedWith()
    {
        return canBeInteracted;
    }

    public void EnteredInRange(GameObject interactor)
    {
    }

    public void ExitedRange(GameObject interactor)
    {
    }

    public void Interact(GameObject interactor)
    {
        TryOpen();
    }

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);

    public void TryOpen()
    {
        if (GameManager.AcquiredCards >= GameManager.NeededCards)
        {
            canBeInteracted = false;
            animator.SetTrigger("Open");
        }
    }
}
