using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private GameManager.E_CharactersNames character;

    public void EnteredInRange(GameObject interactor)
    {
    }

    public void ExitedRange(GameObject interactor)
    {
    }

    public void Interact(GameObject interactor)
    {
        PlayersManager.PlayerCharacterComponents pcc = new PlayersManager.PlayerCharacterComponents();
        bool pccIsSet = false;

        foreach (var item in PlayersManager.Instance.CharacterComponents)
        {
            if (item.character == character)
            {
                pcc = item;
                pccIsSet = true;
                break;
            }
        }

        if (!pccIsSet) return;

        interactor.GetComponentInParent<PlayerCharacter>().SwitchCharacter(pcc.dash, pcc.skill, pcc.stats, pcc.sprite);
    }

    public bool CanBeInteractedWith()
    {
        return true;
    }
}
