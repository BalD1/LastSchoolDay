using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : ImageChangeOnController
{
    [SerializeField] private CircleCollider2D trigger;
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private SpriteRenderer promptText;
    [SerializeField] private SpriteRenderer promptGlow;

    public SpriteRenderer PromptText { get => promptText; }

    private List<IInteractable> interactablesInRange = new List<IInteractable>();
    private List<IInteractable> interactablesToRemove = new List<IInteractable>();

    private IInteractable closestInteractable;

    protected override void EventsSubscriber()
    {
        if (targetPlayer.PlayerIndex == 0) PlayerInputsEvents.OnDeviceChange += CheckDevice;
        targetPlayer.OnInteractInput += InteractWithClosest;
    }

    protected override void EventsUnSubscriber()
    {
        if (targetPlayer.PlayerIndex == 0) PlayerInputsEvents.OnDeviceChange -= CheckDevice;
        targetPlayer.OnInteractInput -= InteractWithClosest;
    }

    protected override void Start()
    {
        base.Start();
        CheckDevice(targetPlayer.PlayerInputsComponent.currentDeviceType);
    }

    protected override void CheckDevice(PlayerInputsManager.E_Devices device)
    {
        promptText.sprite = imagesHolder.GetButtonImage(btnType, device);
        promptGlow.sprite = imagesHolder.GetButtonImage(btnType, device);
    }

    private void InteractWithClosest()
    {
        closestInteractable?.Interact(this.gameObject);

        if (closestInteractable == null) return;
        if (closestInteractable.CanBeInteractedWith() == false)
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
        if (interactablesInRange.Count == 0)
        {
            interactPrompt.SetActive(false);
            return;
        }
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