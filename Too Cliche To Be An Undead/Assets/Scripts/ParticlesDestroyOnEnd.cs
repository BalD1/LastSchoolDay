using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesDestroyOnEnd : MonoBehaviour
{
    private ParticleSystem particles;

    void Start()
    {
        particles = this.GetComponent<ParticleSystem>();

        Destroy(this.gameObject, particles.main.duration);
    }

}
