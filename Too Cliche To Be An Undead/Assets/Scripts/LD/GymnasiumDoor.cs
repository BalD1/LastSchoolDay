using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class GymnasiumDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject keycardHolderPF;

    private GameObject[] keycardsHolders;

    private bool canBeInteracted = true;

    private int keycardsOfferedToDoor = 0;

    private void Start()
    {
        GameManager.Instance._onRunStarted += InstantiateKeycardHolders;
    }

    private void InstantiateKeycardHolders()
    {
        keycardsHolders = new GameObject[GameManager.NeededCards];
        for (int i = 0; i < GameManager.NeededCards; i++)
        {
            GameObject gO = Instantiate(keycardHolderPF, this.transform);

            keycardsHolders[i] = gO;
        }
    }

    public bool CanBeInteractedWith()
    {
        return canBeInteracted;
    }

    public void EnteredInRange(GameObject interactor)
    {
        for (int i = 0; i < keycardsHolders.Length; i++)
        {
            keycardsHolders[i].SetActive(true);

        }
    }

    public void ExitedRange(GameObject interactor)
    {
        for (int i = 0; i < keycardsHolders.Length; i++)
        {
            keycardsHolders[i].SetActive(true);
        }
    }

    public void Interact(GameObject interactor)
    {
        int keysToOffer = GameManager.AcquiredCards - keycardsOfferedToDoor;

        for (int i = 0; i < keysToOffer; i++)
        {
            keycardsHolders[keycardsOfferedToDoor].GetComponentInChildren<SpriteRenderer>().color = Color.white;
            keycardsOfferedToDoor += 1;
        }

        TryOpen();
    }

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);

    public void TryOpen()
    {
        if (keycardsOfferedToDoor >= GameManager.NeededCards)
        {
            canBeInteracted = false;
            animator.SetTrigger("Open");
        }
    }
}
