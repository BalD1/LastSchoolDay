using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_DialogueTest : MonoBehaviour, IInteractable
{
    [SerializeField] private string dialogueToCall = "test";
    
    public bool CanBeInteractedWith()
    {
        return true;
    }

    public void EnteredInRange(GameObject interactor)
    {
    }

    public void ExitedRange(GameObject interactor)
    {
    }

    public float GetDistanceFrom(Transform target)
    {
        return Vector2.Distance(this.transform.position, target.position);
    }

    public void Interact(GameObject interactor)
    {
        DialogueManager.Instance.TryStartDialogue(dialogueToCall, true);
    }
}
