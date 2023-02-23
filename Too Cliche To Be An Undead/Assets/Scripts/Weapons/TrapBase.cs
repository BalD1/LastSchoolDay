using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBase : MonoBehaviour
{
    private float damages;
    private SCRPT_EntityStats.E_Team team;

    private bool destroyOnTrigger;

    private float duration;
    private float timer;

    public void Setup(float _damages, float _duration, SCRPT_EntityStats.E_Team _team, bool _destroyOnTrigger)
    {
        damages = _damages;
        duration = _duration;
        timer = _duration;
        team = _team;
        destroyOnTrigger = _destroyOnTrigger;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0) Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable interactable = collision.GetComponent<IDamageable>();
        if (interactable.OnTakeDamages(damages, team) && destroyOnTrigger) Destroy(this.gameObject); 
    }
}
