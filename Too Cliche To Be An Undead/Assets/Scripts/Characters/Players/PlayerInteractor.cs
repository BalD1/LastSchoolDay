using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private CircleCollider2D trigger;
    [SerializeField] private GameObject interactPrompt;

    [SerializeField] private List<IInteractable> interactablesInRange = new List<IInteractable>();
    private List<IInteractable> interactablesToRemove = new List<IInteractable>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) InvokeInteraction();
    }

    private void InvokeInteraction()
    {
        foreach (var item in interactablesInRange)
        {
            item.Interact();
            interactablesToRemove.Add(item);
        }
        CleanListAll();
    }

    public void CleanListSingle(IInteractable i)
    {
        interactablesInRange.Remove(i);
        if (interactablesInRange.Count == 0) interactPrompt.SetActive(false);
    }
    public void CleanListAll()
    {
        if (interactablesToRemove.Count <= 0) return;

        interactablesInRange.RemoveAll(x => interactablesToRemove.Contains(x));
        interactablesToRemove.Clear();

        if (interactablesInRange.Count == 0) interactPrompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.transform.parent.GetComponent<IInteractable>();

        if (interactable == null) return;
        if (interactable.CanBeInteractedWith() == false) return;

        interactable.EnteredInRange(this.gameObject);
        interactablesInRange.Add(interactable);
        interactPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.transform.parent.GetComponent<IInteractable>();

        if (interactable == null) return;

        interactable.ExitedRange(this.gameObject);
        CleanListSingle(interactable);
    }
}
