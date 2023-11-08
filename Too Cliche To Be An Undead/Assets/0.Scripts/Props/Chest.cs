using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private SO_DropTable dropTable;

    [SerializeField] private List<PlayerInteractor> interactors = new List<PlayerInteractor>();

    [SerializeField] private Sprite openedSprite;

    [SerializeField] private bool isOpen = false;

    [SerializeField] private SpriteRenderer spriteRenderer;

    public void EnteredInRange(GameObject interactor)
    {
        interactors.Add(interactor.GetComponent<PlayerInteractor>());
        spriteRenderer.material = GameAssets.Instance.OutlineMaterial;
    }

    public void ExitedRange(GameObject interactor)
    {
        interactors.Remove(interactor.GetComponent<PlayerInteractor>());
        spriteRenderer.material = GameAssets.Instance.DefaultMaterial;
    }

    public void Interact(GameObject interactor)
    {
        if (isOpen) return; 
        isOpen = true;

        dropTable.DropRandom(this.transform.position);

        this.transform.GetComponentInChildren<SpriteRenderer>().sprite = openedSprite;

        interactors.Clear();

        if (!CanBeInteractedWith()) spriteRenderer.material = GameAssets.Instance.DefaultMaterial;
    }

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);

    public bool CanBeInteractedWith()
    {
        return (isOpen == false);
    }
}
