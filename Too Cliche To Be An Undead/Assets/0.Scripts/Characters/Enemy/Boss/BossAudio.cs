using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudio : MonoBehaviour
{
    [SerializeField] private BossZombie owner;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SCRPT_BossAudio audioData;

    private void Reset()
    {
        owner = this.GetComponentInParent<BossZombie>();
        audioSource = this.GetComponentInParent<AudioSource>();
    }
}
