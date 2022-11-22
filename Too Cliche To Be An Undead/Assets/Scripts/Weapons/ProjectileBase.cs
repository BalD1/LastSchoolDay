using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D body;

    [SerializeField] private float force;

    private SCRPT_EntityStats.E_Team team;
    public SCRPT_EntityStats.E_Team Team { get => team; }

    private float damages;
    private int critChances;

    private Vector2 direction;

    public void Fire(Vector2 direction, float _damages, int _critChances, SCRPT_EntityStats.E_Team _team)
    {
        this.body.velocity = direction * force;
        damages = _damages;
        critChances = _critChances;
        team = _team;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponentInParent<IDamageable>();

        if (damageable == null)
        {
            ProjectileBase proj = collision.GetComponent<ProjectileBase>();
            if (proj != null)
                if (proj.Team.Equals(this.team)) return;

            Debug.Log(collision.name);
            Destroy(this.gameObject);
            return;
        }

        bool isCrit = Random.Range(0, 100) <= critChances;

        if (damageable.OnTakeDamages(damages, team, isCrit)) Destroy(this.gameObject);
    }
}
