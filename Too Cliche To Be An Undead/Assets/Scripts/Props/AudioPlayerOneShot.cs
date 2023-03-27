using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerOneShot : MonoBehaviour
{
    public static void Create(Vector2 position, AudioClip clipToPlay)
    {
        GameObject gO = new GameObject();
        gO.transform.position = position;

        AudioSource s = gO.AddComponent<AudioSource>();
        s.playOnAwake = false;
        s.loop = false;
        s.clip = clipToPlay;

        s.Play();

        Destroy(gO, clipToPlay.length + .5f);
    }
}
