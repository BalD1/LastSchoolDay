using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void EnteredInRange(GameObject interactor);
    public void ExitedRange(GameObject interactor);
    public void Interact();
    public bool CanBeInteractedWith();
}
