using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private SCRPT_DropTable dropTable;

    [SerializeField] private List<PlayerInteractor> interactors = new List<PlayerInteractor>();

    [SerializeField] private bool isOpen = false;

    public void EnteredInRange(GameObject interactor)
    {
        interactors.Add(interactor.GetComponent<PlayerInteractor>());
    }

    public void ExitedRange(GameObject interactor)
    {
        interactors.Remove(interactor.GetComponent<PlayerInteractor>());
    }

    public void Interact()
    {
        if (isOpen) return; 
        isOpen = true;

        dropTable.DropRandom(this.transform.position);

        this.transform.GetComponentInChildren<SpriteRenderer>().color = Color.red;

        interactors.Clear();
    }

    public bool CanBeInteractedWith()
    {
        return (isOpen == false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
    }
}
