using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviourEventsHandler
{
    [SerializeField] protected IComponentHolder owner;
    [SerializeField] protected AudioSource audioSource;

    private void Reset()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnSubscriber()
    {
    }

    public void PlayAudioWithPitchOneShot(AudioClip clip, float pitchrange)
    {
        audioSource.pitch = Random.Range(1 - pitchrange, 1 + pitchrange);
        audioSource.PlayOneShot(clip);
    }
}
