using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;
using UnityEngine.Rendering;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private CircleCollider2D trigger;
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private TextMeshPro promptText;

    [SerializeField] private PlayerCharacter owner;

    [SerializeField] private List<IInteractable> interactablesInRange = new List<IInteractable>();
    private List<IInteractable> interactablesToRemove = new List<IInteractable>();

    public void InvokeInteraction(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        foreach (var item in interactablesInRange)
        {
            item.Interact(this.gameObject);
            if (item.CanBeInteractedWith() == false)
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

    private void SetPrompt()
    {
        interactPrompt.SetActive(true);
        string res = "E";

        InputDevice d = owner.Inputs.devices[0];

        if (d is XInputController) res = "Y";
        else if (d is DualShockGamepad) res = "TRIANGLE";
        else if (d is SwitchProControllerHID) res = "X";

        promptText.text = $"Press {res} to interact";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.transform.parent.GetComponent<IInteractable>();

        if (interactable == null) return;
        if (interactable.CanBeInteractedWith() == false) return;

        interactable.EnteredInRange(this.gameObject);

        interactablesInRange.Add(interactable);
        SetPrompt();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.transform.parent.GetComponent<IInteractable>();

        if (interactable == null) return;

        interactable.ExitedRange(this.gameObject);
        CleanListSingle(interactable);
    }
}