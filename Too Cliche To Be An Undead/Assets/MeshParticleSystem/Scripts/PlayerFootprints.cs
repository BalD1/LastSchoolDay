using System;
using UnityEngine;

public class PlayerFootprints : MonoBehaviourEventsHandler
{
    [SerializeField] private PlayerCharacter owner;

    [SerializeField] private float allowfootprints_DURATION = 2;
    private float allowfootprints_TIMER;

    [SerializeField] private float footprintsSpawn_COOLDOWN;
    private float footprintsSpawn_TIMER;

    [SerializeField] private float footprints_LIFETIME;

    [SerializeField] private float delayBetweenSteps = .1f;

    [SerializeField] private float yOffset;

    [SerializeField] private float delayBetweenStepsAudio = .2f;
    private float footSteps_TIMER;

    private bool leftPrint;

    private bool allowFootSteps = true;

    protected override void Awake()
    {
        base.Awake();
        footprintsSpawn_TIMER = footprintsSpawn_COOLDOWN;
        footSteps_TIMER = delayBetweenStepsAudio;
    }

    protected override void EventsSubscriber()
    {
        owner.OnSteppedIntoTrigger += OwnerSteppedInTrigger;
        owner.OnSuccessfulAttack += OwnerAttacked;
        owner.OnStateChange += OnOwnerStateChange;
    }

    protected override void EventsUnSubscriber()
    {
        owner.OnSteppedIntoTrigger -= OwnerSteppedInTrigger;
        owner.OnSuccessfulAttack -= OwnerAttacked;
        owner.OnStateChange -= OnOwnerStateChange;
    }   

    private void OnOwnerStateChange(FSM_Player_Manager.E_PlayerState newState)
    {
        allowFootSteps = newState == FSM_Player_Manager.E_PlayerState.Dashing ||
                         newState == FSM_Player_Manager.E_PlayerState.InSkill;
    }

    private void OwnerSteppedInTrigger(Type triggerType)
    {
        if (triggerType.Equals(typeof(BloodStamps)))
            allowfootprints_TIMER = allowfootprints_DURATION;
    }

    private void OwnerAttacked(bool isBigHit) => allowfootprints_TIMER = allowfootprints_DURATION;

    private void Update()
    {
        if (!allowFootSteps) return;

        UpdateFootPrint();
        UpdateFootStep();
    }

    private void UpdateFootPrint()
    {
        if (allowfootprints_TIMER <= 0) return;

        allowfootprints_TIMER -= Time.deltaTime;

        if (footprintsSpawn_TIMER > 0)
        {
            footprintsSpawn_TIMER -= Time.deltaTime;
            return;
        }

        if (owner.PlayerMotor.Velocity == Vector2.zero) return;

        SpawnFootprint();
        LeanTween.delayedCall(delayBetweenSteps, SpawnFootprint);

        footprintsSpawn_TIMER = footprintsSpawn_COOLDOWN;
    }

    private void UpdateFootStep()
    {
        if (footSteps_TIMER > 0)
        {
            footSteps_TIMER -= Time.deltaTime;
            return;
        }

        if (owner.StateManager.CurrentState.ToString() == "Idle" || owner.PlayerMotor.Velocity == Vector2.zero) return;

        footSteps_TIMER = delayBetweenStepsAudio;

        owner.OnFootPrint?.Invoke();
    }

    private void SpawnFootprint()
    {
        Vector2 pos = this.transform.position;

        pos.y += leftPrint ? yOffset : -yOffset;
        leftPrint = !leftPrint;

        FootprintParticleSystemHandler.Instance.SpawnFootprint(pos, owner.PlayerMotor.LastDirection, footprints_LIFETIME);
    }
}
