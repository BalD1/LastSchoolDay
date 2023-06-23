using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CA_CinematicPlayersMove : CA_CinematicAction
{
    [SerializeField] private List<Vector2> targetPositions;
    [SerializeField] private bool setToIdleAtEnd;

    public List<Vector2> TargetPositions => targetPositions;

    private int animsToPlay;

    public CA_CinematicPlayersMove(List<Vector2> _targetPositions, bool _setToIdleAtEnd)
    {
        Setup(_targetPositions, _setToIdleAtEnd);
    }

    public void Setup(List<Vector2> _positions, bool _setToIdleAtEnd)
    {
        this.targetPositions = _positions;
        this.setToIdleAtEnd = _setToIdleAtEnd;
    }
    public override void Execute()
    {
        animsToPlay = owner.Players.Count;
        if (animsToPlay <= 0)
        {
            this.ActionEnded(this);
            return;
        }
        if (targetPositions.Count > owner.Players.Count)
        {
            for (int i = 0; i < owner.Players.Count; i++)
                MovePlayerTo(owner.Players[i], targetPositions[i]);
            return;
        }

        for (int i = 0; i < owner.Players.Count; i++)
            MovePlayerTo(owner.Players[i], targetPositions[i % targetPositions.Count]);
    }

    private void MovePlayerTo(PlayerCharacter player, Vector2 position)
    {
        player.GetRb.simulated = false;
        AnimationReferenceAsset walkAnim = player.AnimationController.animationsData.WalkAnim;
        player.AnimationController.SetAnimation(walkAnim, true);
        player.AnimationController.FlipSkeleton(position.x > player.transform.position.x);

        float travelTime = Vector2.Distance(player.transform.position, position) / player.MaxSpeed_M;
        LeanTween.move(player.gameObject, position, travelTime).setOnComplete(() =>
        {
            OnPlayerReachedDestination(player);
        });
    }

    private void OnPlayerReachedDestination(PlayerCharacter player)
    {
        if (setToIdleAtEnd)
        {
            AnimationReferenceAsset idleAnim = player.AnimationController.animationsData.IdleAnim;
            player.AnimationController.SetAnimation(idleAnim, true);
        }
        player.GetRb.simulated = true;

        animsToPlay--;
        if (animsToPlay <= 0) this.ActionEnded(this);
    }
}
