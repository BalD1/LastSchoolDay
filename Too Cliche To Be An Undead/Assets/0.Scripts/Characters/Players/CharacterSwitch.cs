using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] SO_CharactersComponents pcc;

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
        interactor.GetComponentInParent<PlayerCharacter>().SwitchCharacter(pcc);

        if (!CanBeInteractedWith()) spriteRenderer.material = GameAssets.Instance.DefaultMaterial;
    }

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);

    public bool CanBeInteractedWith()
    {
        return true;
    }
}
