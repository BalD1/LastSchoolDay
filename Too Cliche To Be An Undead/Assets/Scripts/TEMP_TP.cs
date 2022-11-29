using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_TP : MonoBehaviour, IInteractable
{
    [SerializeField] private FightArena fightArena;

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
        foreach (var item in GameManager.Instance.playersByName)
        {
            item.playerScript.gameObject.transform.position = fightArena.transform.position;
        }

        fightArena.SpawnNext(0);
    }

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);
}
