using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private SCRPT_Dash dashSwitch;
    [SerializeField] private SCRPT_Skill skillSwitch;
    [SerializeField] private SCRPT_EntityStats statsSwitch;
    [SerializeField] private Sprite spriteSwitch;


    public void EnteredInRange(GameObject interactor)
    {
    }

    public void ExitedRange(GameObject interactor)
    {
    }

    public void Interact(GameObject interactor)
    {
        interactor.GetComponentInParent<PlayerCharacter>().SwitchCharacter(dashSwitch, skillSwitch, statsSwitch, spriteSwitch);
    }

    public bool CanBeInteractedWith()
    {
        return true;
    }
}
