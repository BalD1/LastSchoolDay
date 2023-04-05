using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesDestroyOnEnd : MonoBehaviour
{
    private ParticleSystem.MainModule main;

    private void Start()
    {
        main = this.GetComponent<ParticleSystem>().main;

        main.stopAction = ParticleSystemStopAction.Destroy;
    }

    private void EndAction()
    {
        Destroy(this.gameObject);
    }

}
