using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePropsDetection : MonoBehaviour
{
    [SerializeField] private ProjectileBase owner;

    private void Reset()
    {
        owner = this.GetComponentInParent<ProjectileBase>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) owner.HitWall(collision.contacts.RandomElement().point);
    }
}
