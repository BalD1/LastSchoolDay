using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBase : MonoBehaviour
{
    protected float damages;
    protected Entity owner;

    private bool destroyOnTrigger;

    private float duration;
    private float timer;

    public virtual void Setup(float _damages, float _duration, Entity _owner, bool _destroyOnTrigger)
    {
        damages = _damages;
        duration = _duration;
        timer = _duration;
        owner = _owner;
        destroyOnTrigger = _destroyOnTrigger;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0) Destroy(this.gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable interactable = collision.GetComponent<IDamageable>();
        if (interactable.OnTakeDamages(damages, owner) && destroyOnTrigger) Destroy(this.gameObject); 
    }
}
