using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHEAT_Spawner : MonoBehaviour, IInteractable
{
    [SerializeField] private SO_DropTable dropTable;

    [SerializeField] private SpriteRenderer spriteRenderer;

    public bool CanBeInteractedWith()
    {
        return true;
    }

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
        dropTable.DropRandom(this.transform.position);

        if (!CanBeInteractedWith()) spriteRenderer.material = GameAssets.Instance.DefaultMaterial;
    }

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);
}
