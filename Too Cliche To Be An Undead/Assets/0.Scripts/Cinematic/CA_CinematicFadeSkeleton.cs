
using Pathfinding;
using Spine;
using Spine.Unity;
using System;
using UnityEngine;

[System.Serializable]
public class CA_CinematicFadeSkeleton : CA_CinematicAction
{
    [SerializeField] private Skeleton targetSkeleton;
    [SerializeField] private float alphaGoal;
    [SerializeField] private float timeGoal;

    public CA_CinematicFadeSkeleton(float alphaGoal, float time, Skeleton skeleton)
    {
        Setup(alphaGoal, time, skeleton);
    }

    public void Setup(float alphaGoal, float time, Skeleton skeleton)
    {
        this.alphaGoal = alphaGoal;
        this.timeGoal = time;
        this.targetSkeleton = skeleton;
    }

    public override void Execute()
    {
        LeanTween.value(targetSkeleton.GetColor().a, alphaGoal, timeGoal).setOnUpdate((float alpha) =>
        {
            Color c = targetSkeleton.GetColor();
            c.a = alpha;
            targetSkeleton.SetColor(c);
        }).setOnComplete(ActionEnded);
    }

    private void ActionEnded() => ActionEnded(this);
}
