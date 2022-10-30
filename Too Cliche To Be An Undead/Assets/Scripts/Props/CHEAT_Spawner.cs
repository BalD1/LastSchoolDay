using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHEAT_Spawner : MonoBehaviour, IInteractable
{
    [SerializeField] private SCRPT_DropTable dropTable;

    public bool CanBeInteractedWith()
    {
        return true;
    }

    public void EnteredInRange(GameObject interactor)
    {
    }

    public void ExitedRange(GameObject interactor)
    {
    }

    public void Interact(GameObject interactor)
    {
        dropTable.DropRandom(this.transform.position);
    }
}
