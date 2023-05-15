using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;
using static ButtonsImageByDevice;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private CircleCollider2D trigger;
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private SpriteRenderer promptText;
    [SerializeField] private SpriteRenderer promptGlow;

    public SpriteRenderer PromptText { get => promptText; }

    [SerializeField] private PlayerCharacter owner;

    private List<IInteractable> interactablesInRange = new List<IInteractable>();
    private List<IInteractable> interactablesToRemove = new List<IInteractable>();

    private IInteractable closestInteractable;

    private void Start()
    {
        GameManager.Instance._onSceneReload += ResetOnLoad;

        owner.D_onDeviceChange += CheckDevice;
        CheckDevice(owner.currentDeviceType);
    }

    private void CheckDevice(PlayerCharacter.E_Devices device)
    {
        promptText.sprite = ButtonsImageByDevice.Instance.GetButtonImage(E_ButtonType.Third, device);
        promptGlow.sprite = ButtonsImageByDevice.Instance.GetButtonImage(E_ButtonType.Third, device);
    }

    private void OnDestroy()
    {
        owner.D_onDeviceChange -= CheckDevice;
    }

    public void ResetOnLoad()
    {
        interactablesInRange.Clear();
        interactablesToRemove.Clear();

        closestInteractable = null;
        interactPrompt.SetActive(false);
    }

    public void InvokeInteraction(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (closestInteractable == null) return;

        InteractWithClosest();
    }

    private void InteractWithClosest()
    {
        closestInteractable?.Interact(this.gameObject);

        if (closestInteractable?.CanBeInteractedWith() == false)
        {
            CleanListSingle(closestInteractable);
            closestInteractable = null;
            SearchClosestInList();
        }
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

    public void ResetCollider()
    {
        this.trigger.enabled = false;
        this.trigger.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponentInParent<IInteractable>();

        if (interactable == null) return;
        if (interactable.CanBeInteractedWith() == false) return;

        float newChallengerDistance = interactable.GetDistanceFrom(this.transform);
        if (closestInteractable == null) SetNewClosest();
        else
        {
            float lastClosestDistance = Vector2.Distance(this.transform.position, collision.transform.position);

            if (newChallengerDistance < lastClosestDistance) SetNewClosest();
        }

        void SetNewClosest()
        {
            if (closestInteractable != null) closestInteractable.ExitedRange(this.gameObject);

            closestInteractable = interactable;

            closestInteractable.EnteredInRange(this.gameObject);
        }

        interactablesInRange.Add(interactable);

        interactPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponentInParent<IInteractable>();

        if (interactable == null) return;

        if (closestInteractable == interactable) SearchClosestInList();

        CleanListSingle(interactable);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        SearchClosestInList(false);
    }

    private void SearchClosestInList(bool resetIfNothingFound = true)
    {
        IInteractable newClosest = null;
        float closestDistance = closestInteractable != null ? closestInteractable.GetDistanceFrom(this.transform) : float.MaxValue;
        float itemDistance = float.MaxValue;

        foreach (var item in interactablesInRange)
        {
            if (item == null || item.CanBeInteractedWith() == false)
            {
                interactablesToRemove.Add(item);
                continue;
            }
            else
            {

                itemDistance = item.GetDistanceFrom(this.transform);

                if (itemDistance < closestDistance)
                {
                    newClosest = item;
                    closestDistance = itemDistance;
                }
            }
        }

        if (interactablesToRemove.Count > 0) CleanListAll();

        if ((newClosest != null) || (newClosest == null && resetIfNothingFound))
        {
            closestInteractable?.ExitedRange(this.gameObject);
            closestInteractable = newClosest;
            closestInteractable?.EnteredInRange(this.gameObject);
        }
    }
}