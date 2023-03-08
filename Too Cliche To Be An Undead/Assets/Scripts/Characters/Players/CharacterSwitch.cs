using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private GameManager.E_CharactersNames character;

    [SerializeField] private SpriteRenderer spriteRenderer;

    PlayersManager.PlayerCharacterComponents pcc = new PlayersManager.PlayerCharacterComponents();

    private void Awake()
    {
        foreach (var item in PlayersManager.Instance.CharacterComponents)
        {
            if (item.character == character)
            {
                pcc = item;
                break;
            }
        }
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
        interactor.GetComponentInParent<PlayerCharacter>().SwitchCharacter(pcc.dash, pcc.skill, pcc.stats, pcc.character, pcc.animData, pcc.audioClips);

        if (!CanBeInteractedWith()) spriteRenderer.material = GameAssets.Instance.DefaultMaterial;
    }

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);

    public bool CanBeInteractedWith()
    {
        return true;
    }
}
