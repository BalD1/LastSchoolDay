using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private CircleCollider2D trigger;
    [SerializeField] private GameObject interactPrompt;

    private List<IInteractable> interactablesInRange = new List<IInteractable>();
    private List<IInteractable> interactablesToRemove = new List<IInteractable>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) InvokeInteraction();
    }

    private void LateUpdate()
    {
        if (interactablesToRemove.Count > 0) interactablesInRange.RemoveAll(x => interactablesToRemove.Contains(x));
    }

    private void InvokeInteraction()
    {
        foreach (var item in interactablesInRange)
        {
            item.Interact();
        }
    }

    public void RemoveInteractable(IInteractable i)
    {
        interactablesToRemove.Add(i);
        if (interactablesInRange.Count == 0) interactPrompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.transform.root.GetComponent<IInteractable>();

        if (interactable == null) return;
        if (interactable.CanBeInteractedWith() == false) return;

        interactable.EnteredInRange(this.gameObject);
        interactablesInRange.Add(interactable);
        interactPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.transform.root.GetComponent<IInteractable>();

        if (interactable == null) return;

        interactable.ExitedRange(this.gameObject);
        RemoveInteractable(interactable);
    }
}
