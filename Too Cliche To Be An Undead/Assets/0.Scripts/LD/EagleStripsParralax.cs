using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleStripsParralax : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private float alphaTransparencyGoal = .3f;
    [SerializeField] private float transitionSpeed = .25f;

    private int entitiesBehindCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity e = collision.transform.GetComponentInParent<Entity>();

        if (e == null) return;

        AddEntity();
        //e.D_onDeathOf += OnEntityDeath;

        if (entitiesBehindCount > 1) return;

        foreach (var item in spriteRenderers)
        {
            item.LeanAlpha(alphaTransparencyGoal, transitionSpeed);
        }
    }

    private void AddEntity() => entitiesBehindCount++;
    private void RemoveEntity() => entitiesBehindCount--;

    private void OnEntityDeath(Entity e)
    {
        RemoveEntity();
        //e.D_onDeathOf -= OnEntityDeath;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Entity e = collision.transform.GetComponentInParent<Entity>();

        if (e == null) return;

        RemoveEntity();
        //e.D_onDeathOf -= OnEntityDeath;

        if (entitiesBehindCount > 0) return;

        foreach (var item in spriteRenderers)
        {
            item.LeanAlpha(1, transitionSpeed);
        }
    }
}
