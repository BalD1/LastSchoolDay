using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScalerByPlayers : EnemyScalerByPlayers
{
    [SerializeField] private AnimationCurve recoverTimerModifier;

    protected override void Start()
    {
        base.Start();

        (owner as BossZombie).recoverTimerModifier = recoverTimerModifier[IGPlayersManager.PlayersCount].value;
    }
}
