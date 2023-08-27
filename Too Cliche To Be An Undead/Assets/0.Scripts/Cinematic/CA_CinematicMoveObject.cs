using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CA_CinematicMoveObject : CA_CinematicAction
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private float travelTime;
    private bool local;

    public CA_CinematicMoveObject(Transform target, Vector2 targetPosition, float travelTime, bool local)
    {
        this.target = target;
        this.targetPosition = targetPosition;
        this.travelTime = travelTime;
        this.local = local;
    }

    public override void Execute()
    {
        if (local) LeanTween.moveLocal(target.gameObject, targetPosition, travelTime).setOnComplete(ActionEnded);
        else LeanTween.move(target.gameObject, targetPosition, travelTime).setOnComplete(ActionEnded);
    }
    private void ActionEnded() => ActionEnded(this);
}
