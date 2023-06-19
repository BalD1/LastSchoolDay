using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CinematicPlayersMove : CinematicAction
{
    private List<PlayerCharacter> players;
    private List<Vector2> targetPositions;

    private int animsToPlay;

    public void Setup(List<PlayerCharacter> _players, List<Vector2> _positions)
    {
        this.players = _players;
        this.targetPositions = _positions;
        animsToPlay = players.Count;
    }
    public override void Execute()
    {
        if (animsToPlay <= 0)
        {
            this.EndAction(this);
            return;
        }
        if (targetPositions.Count > players.Count)
        {
            for (int i = 0; i < targetPositions.Count; i++)
                MovePlayerTo(players[i % players.Count], targetPositions[i]);
            return;
        }

        for (int i = 0; i < players.Count; i++)
            MovePlayerTo(players[i], targetPositions[i % targetPositions.Count]);
    }

    private void MovePlayerTo(PlayerCharacter player, Vector2 position)
    {
        AnimationReferenceAsset walkAnim = player.AnimationController.animationsData.WalkAnim;
        player.AnimationController.SetAnimation(walkAnim, true);

        float travelTime = Vector2.Distance(player.transform.position, position) / player.MaxSpeed_M;
        LeanTween.move(player.gameObject, position, travelTime).setOnComplete(() =>
        {
            OnPlayerReachedDestination(player);
        });
    }

    private void OnPlayerReachedDestination(PlayerCharacter player)
    {
        AnimationReferenceAsset idleAnim = player.AnimationController.animationsData.IdleAnim;
        player.AnimationController.SetAnimation(idleAnim, true);

        animsToPlay--;
        if (animsToPlay <= 0) this.EndAction(this);
    }
}
