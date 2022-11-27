using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GymnasiumDoor : MonoBehaviour, IInteractable
{
    public int requiredCardsCount = 0;

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

    public void TryOpen()
    {
        if (GameManager.AcquiredCards >= requiredCardsCount)
        {
            Destroy(this.gameObject);
        }
    }
}
