using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SCRPT_BossPatern;

public class AttacksPatern : MonoBehaviour
{
    [SerializeField] private BossZombie owner;

    [SerializeField] private SCRPT_BossPatern paterns;

    [field: SerializeField] public SCRPT_BossPatern.S_Paterns currentPatern { get; private set; }

    private Queue<S_AttackAndCooldown> attacks;

    [field: SerializeField] [field: ReadOnly] public int currentStage { get; private set; }

    public delegate void D_PaternEnded();
    public D_PaternEnded D_paterneEnded;

    private void Awake()
    {
        StartNewPatern();
    }

    public void StartNewPatern()
    {
        attacks = new Queue<S_AttackAndCooldown>();
        currentPatern = paterns.GetRandomPaternOfStage(currentStage);
        foreach (var item in currentPatern.attackWithCooldown) attacks.Enqueue(item);

        GetNextAttack();
    }

    public S_AttackAndCooldown? GetNextAttack()
    {
        if (attacks.Count <= 0)
        {
            D_paterneEnded?.Invoke();
            return null;
        }

        S_AttackAndCooldown nextAttack = attacks.Dequeue();
        owner.CurrentAttack = nextAttack;
        return nextAttack;
    }

    public void NextStage()
    {
        currentStage++;
        StartNewPatern();
        SoundManager.Instance.ChangeMusicMixerPitch(paterns.pitchModifierOnStageChange);
    }
}
