using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_AudioPlayer : MonoBehaviour
{
    public AudioSource source;

    public AudioClip clip;

    [InspectorButton("Pause", ButtonWidth = 200)]
    public bool pause;

    [InspectorButton("UnPause", ButtonWidth = 200)]
    public bool unpause;

    [InspectorButton("PlayMainTheme", ButtonWidth = 200)]
    public bool playmaintheme;

    [InspectorButton("PlayBossTheme", ButtonWidth = 200)]
    public bool playbosstheme;

    [InspectorButton("PlayMenuTheme", ButtonWidth = 200)]
    public bool playmenutheme;

    public void Pause()
    {
        SoundManager.Instance.PauseMusic();
    }
    public void UnPause()
    {
        SoundManager.Instance.ResumeMusic();
    }

    public void PlayMainTheme()
    {
        SoundManager.Instance.PlayMusic(SoundManager.E_MusicClipsTags.MainScene);
    }
    public void PlayBossTheme()
    {
        SoundManager.Instance.PlayMusic(SoundManager.E_MusicClipsTags.BossMusic);
    }
    public void PlayMenuTheme()
    {
        SoundManager.Instance.PlayMusic(SoundManager.E_MusicClipsTags.MainMenu);
    }
}
