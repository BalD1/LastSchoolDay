using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsParticlesCallback : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        CoinsOptimizer.Instance.ParticlesStopped();
        Destroy(this.gameObject);
    }
}
