using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Animator animator;

    [SerializeField] private Transform lootSpawnPoint;

    [SerializeField] private SCRPT_DropTable dropTable;

    private bool isOpen = false;

    public void EnteredInRange(GameObject interactor)
    {
        spriteRenderer.material = GameAssets.Instance.OutlineMaterial;
    }

    public void ExitedRange(GameObject interactor)
    {
        spriteRenderer.material = GameAssets.Instance.DefaultMaterial;
    }

    public void Interact(GameObject interactor)
    {
        isOpen = true;
        animator.SetTrigger("Open");

        dropTable.DropRandom(lootSpawnPoint.position);
        spriteRenderer.material = GameAssets.Instance.DefaultMaterial;
    }

    public bool CanBeInteractedWith()
    {
        return !isOpen;
    }
}
