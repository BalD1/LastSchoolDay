using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_AudioPlayer : MonoBehaviour
{
    public AudioSource source;

    public AudioClip clip;

    [InspectorButton("play")]
    public bool Play;

    public void play()
    {
        source.PlayOneShot(clip);
    }
}
