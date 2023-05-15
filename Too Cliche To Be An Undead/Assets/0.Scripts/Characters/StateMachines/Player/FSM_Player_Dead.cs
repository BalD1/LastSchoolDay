using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_Player_Dead : FSM_Base<FSM_Player_Manager>
{
    public PlayerCharacter owner;

    public override void EnterState(FSM_Player_Manager stateManager)
    {
        owner ??= stateManager.Owner;

        owner.Weapon.IndicatorHolder.GetComponentInChildren<SpriteRenderer>().enabled = false;
        PlayersManager.Instance.RemoveAlivePlayer(owner.transform);
    }

    public override void UpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void FixedUpdateState(FSM_Player_Manager stateManager)
    {
    }

    public override void ExitState(FSM_Player_Manager stateManager)
    {
        owner.Weapon.IndicatorHolder.GetComponentInChildren<SpriteRenderer>().enabled = true;
        owner.StatsModifiers.Clear();
        owner.Attackers.Clear();
    }

    public override void Conditions(FSM_Player_Manager stateManager)
    {
    }

    public override string ToString() => "Dead";
}
