using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowOnZombiesCollide : MonoBehaviour
{
    [SerializeField] private PlayerCharacter owner;

    private void Reset()
    {
        if (this.TryGetComponent(out owner) == false)
        {
            owner = this.GetComponentInParent<PlayerCharacter>();
        }
    }

    private void Awake()
    {
        owner.d_EnteredTrigger += TriggerEnter;
        owner.d_ExitedTrigger += TriggerExit;
    }

    private void TriggerEnter(Collider2D e)
    {

    }

    private void TriggerExit(Collider2D e)
    {

    }
}
