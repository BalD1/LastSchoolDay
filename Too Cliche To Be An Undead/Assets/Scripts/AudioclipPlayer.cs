using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioclipPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource source;

    public static AudioclipPlayer Create(Vector2 position, AudioClip clipToPlay)
    {
        GameObject gO = GameAssets.Instance.AudioclipPlayerPF.Create(position);

        AudioclipPlayer clipPlayer = gO.GetComponent<AudioclipPlayer>();

        clipPlayer.Setup(clipToPlay);

        return clipPlayer;
    }

    public void Setup(AudioClip _clipToPlay)
    {
        source.PlayOneShot(_clipToPlay);

        Destroy(this.gameObject, _clipToPlay.length + 1);
    }
}
