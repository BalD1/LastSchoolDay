using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void EnteredInRange(GameObject interactor);
    public void ExitedRange(GameObject interactor);
    public void Interact(GameObject interactor);
    public bool CanBeInteractedWith();

    public float GetDistanceFrom(Transform target);
}
