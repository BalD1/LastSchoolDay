using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticles : MonoBehaviour
{
    private static MonoPool<BloodParticles> pool;

    public static void CheckPool()
    {
        if (pool == null)
            pool = new MonoPool<BloodParticles>
                (_createAction: () => GameAssets.Instance.BloodParticlesPF.Create(Vector2.zero).GetComponent<BloodParticles>(),
                _parentContainer: GameManager.Instance.InstantiatedMiscParent);
    }

    public static void GetNext(Vector2 position)
    {
        pool.GetNext(position);
    }

    private void OnParticleSystemStopped()
    {
        this.gameObject.SetActive(false);
        pool.Enqueue(this);
    }
}
