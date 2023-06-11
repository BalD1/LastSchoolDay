using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitFX : MonoBehaviour
{
    private static MonoPool<BossHitFX> pool;

    public static void CheckPool()
    {
        if (pool == null)
            pool = new MonoPool<BossHitFX>
                (_createAction: () => GameAssets.Instance.BossHitFX.Create(Vector2.zero).GetComponent<BossHitFX>(),
                _parentContainer: GameManager.Instance.InstantiatedMiscParent);
    }

    public static void GetNext(Vector2 position)
    {
        pool.GetNext(position);
    }

    private void OnParticleSystemStopped()
    {
        pool.Enqueue(this);
    }
}
