using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioclipPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource source;

    public static AudioclipPlayer Create(Vector2 position, AudioClip clipToPlay, float pitchRange = 0)
    {
        Vector3 offsetedPos = position;
        offsetedPos.z = -10;

        GameObject gO = GameAssets.Instance.AudioclipPlayerPF.Create(offsetedPos);

        AudioclipPlayer clipPlayer = gO.GetComponent<AudioclipPlayer>();

        clipPlayer.Setup(clipToPlay);

        return clipPlayer;
    }
    public static AudioclipPlayer Create(Vector2 position, SCRPT_EntityAudio.S_AudioClips audioData)
    {
        return Create(position, audioData.clip, audioData.pitchRange);
    }

    public void Setup(AudioClip _clipToPlay, float pitchRange = 0)
    {
        source.pitch = Random.Range(1 - pitchRange, 1 + pitchRange);
        source.PlayOneShot(_clipToPlay);

        Destroy(this.gameObject, _clipToPlay.length + 1);
    }
    public void Setup(SCRPT_EntityAudio.S_AudioClips audioData)
    {
        Setup(audioData.clip, audioData.pitchRange);
    }
}
