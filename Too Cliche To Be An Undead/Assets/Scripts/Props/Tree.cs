using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private SpineColorModifier spineColor;

    private int entitiesBehindCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity e = collision.transform.GetComponentInParent<Entity>();

        if (e == null) return;

        AddEntity();
        e.D_onDeathOf += OnEntityDeath;

        if (entitiesBehindCount <= 1)
            spineColor.SwitchToModifiedColor(.5f);
    }

    private void AddEntity() => entitiesBehindCount++;
    private void RemoveEntity() => entitiesBehindCount--;

    private void OnEntityDeath(Entity e)
    {
        RemoveEntity();
        e.D_onDeathOf -= OnEntityDeath;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Entity e = collision.transform.GetComponentInParent<Entity>();

        if (e == null) return;

        RemoveEntity();
        e.D_onDeathOf -= OnEntityDeath;

        if (entitiesBehindCount <= 0)
            spineColor.SwitchToBaseColor(.5f);
    }
}
