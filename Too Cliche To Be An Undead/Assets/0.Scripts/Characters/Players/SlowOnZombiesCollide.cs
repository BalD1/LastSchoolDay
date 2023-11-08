using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowOnZombiesCollide : MonoBehaviour
{
    [SerializeField] private PlayerCharacter owner;

    [SerializeField] [Range(0,100)] private float slowdownPercentageAmount = 50;

    private const string MODIFIER_ID = "COLLID_SLOW";

    [SerializeField] [ReadOnly] private int collidingZombiesCount = 0;

    private void Reset()
    {
        if (this.TryGetComponent(out owner) == false)
        {
            owner = this.GetComponentInParent<PlayerCharacter>();
        }
    }

    private void OnDestroy()
    {
        //owner.RemoveModifier(MODIFIER_ID);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<BaseZombie>() == null) return;

        if (slowdownPercentageAmount > 0)
        {
            //float slowDownAmount = owner.MaxSpeed_M * (slowdownPercentageAmount / 100);

            //if (collidingZombiesCount == 0) owner.AddModifierUnique(MODIFIER_ID, -slowDownAmount, StatsModifier.E_StatType.Speed);
        }

        collidingZombiesCount++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<BaseZombie>() == null) return;

        collidingZombiesCount--;

        //if (collidingZombiesCount == 0 && slowdownPercentageAmount > 0) owner.RemoveModifier(MODIFIER_ID);
    }
}
