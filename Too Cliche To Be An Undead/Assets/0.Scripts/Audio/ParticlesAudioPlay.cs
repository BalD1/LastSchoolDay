using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesAudioPlay : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;

    private void Start()
    {
        AudioclipPlayer.Create(this.transform.position, audioClip);
    }
}
