using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashHitParticles : MonoBehaviour
{
    public static MonoPool<DashHitParticles> pool;

    public static void CheckPool()
    {
        if (pool == null)
            pool = new MonoPool<DashHitParticles>
                (_createAction: () => GameAssets.Instance.DashHitParticlesPF.Create(Vector2.zero).GetComponent<DashHitParticles>(),
                _parentContainer: GameManager.Instance.InstantiatedMiscParent);
    }
    public static void GetNext(Vector2 position) => pool.GetNext(position);

}
