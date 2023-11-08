using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using BalDUtilities.Misc;
using System;
using System.Collections;
using AYellowpaper.SerializedCollections;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfx2DSource;
    [SerializeField] private AudioSource[] soundsSource;

    [SerializeField] private AudioMixer mainMixer;

    public const string masterVolParam = "MasterVol";
    public const string musicVolParam = "MusicVol";
    public const string sfxVolparam = "SFXVol";

    public const string masterPitchParam = "MasterPitch";
    public const string musicPitchParam = "MusicPitch";
    public const string sfxPitchParam = "SFXPitch";

    [Header("Sliders")]
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Space]
    [SerializeField] private float volMultiplier = 30f;

    [SerializeField] private float musicFadeSpeed = 1f;

    [SerializeField] private float musicStartLoopDelayModifier = -.1f;

    private SCRPT_MusicData currentPlayingMusicData;

    private Coroutine musicFadeCoroutine;

    public enum E_SFXClipsTags
    {
        Clic,
        StampChange,
    }

    public enum E_MusicClipsTags
    {
        MainMenu,
        MainScene,
        BossMusic,
        InLobby,
        TitleScreen,
    }

    public enum E_MusicFadeBehaviour
    {
        FadeIn,
        FadeOut,
        Pause,
        Resume
    }

    [field: SerializeField] public SerializedDictionary<E_MusicClipsTags, AudioClip> MusicClipsWithTags { get; private set; }
    [field: SerializeField] public SerializedDictionary<E_SFXClipsTags, AudioClip> SFXClipsWithTags { get; private set; }

    protected override void EventsSubscriber()
    {
        UIScreenBaseEvents.OnOpenScreen += OnScreenStateChange;
        UIScreenBaseEvents.OnCloseScreen += OnScreenStateChange;
        GameManagerEvents.OnRunStarted += PlayMainSceneMusic;
    }

    protected override void EventsUnSubscriber()
    {
        UIScreenBaseEvents.OnOpenScreen -= OnScreenStateChange;
        UIScreenBaseEvents.OnCloseScreen -= OnScreenStateChange;
        GameManagerEvents.OnRunStarted -= PlayMainSceneMusic;
    }

    protected override void Start()
    {
        base.Start();
        LoadSlidersValue();

        ChangeMainMixerPitch(1);
        ChangeMusicMixerPitch(1);
        ChangeSFXMixerPitch(1);

        GameManager.Instance.D_bossFightEnded += TryEndBossMusic;
        if (SpawnersManager.ST_InstanceExists())
            SpawnersManager.Instance.D_stampChange += PlayStampChangeSoundEffect; 
    }

    private void OnScreenStateChange(UIScreenBase screen, bool ignoreTweens)
    {
        this.Play2DSFX(E_SFXClipsTags.Clic);
    }

    private void PlayStampChangeSoundEffect(int stamp)
    {
        Play2DSFX(E_SFXClipsTags.StampChange);
    }

    private void TryStartBossMusic()
    {
        if (GameManager.Instance.currentAliveBossesCount > 1) return;

        PlayMusic(E_MusicClipsTags.BossMusic);
    }

    private void TryEndBossMusic()
    {
        if (GameManager.Instance.currentAliveBossesCount > 0) return;

        StopMusic();
    }

    /// <summary>
    /// Loads the sliders value from <seealso cref="SaveManager.GetSavedFloatKey(SaveManager.E_SaveKeys)"/>
    /// </summary>
    private void LoadSlidersValue()
    {
        mainSlider.value = SaveManager.GetSavedFloatKey(SaveManager.E_SaveKeys.F_MasterVolume);
        musicSlider.value = SaveManager.GetSavedFloatKey(SaveManager.E_SaveKeys.F_MusicVolume);
        sfxSlider.value = SaveManager.GetSavedFloatKey(SaveManager.E_SaveKeys.F_SFXVolume);
    }

    public void OnMainSliderValueChange(float value) => HandleSliderChange(value, masterVolParam, SaveManager.E_SaveKeys.F_MasterVolume);
    public void OnMusicSliderValueChange(float value) => HandleSliderChange(value, musicVolParam, SaveManager.E_SaveKeys.F_MusicVolume);
    public void OnSFXSliderValueChange(float value) => HandleSliderChange(value, sfxVolparam, SaveManager.E_SaveKeys.F_SFXVolume);

    /// <summary>
    /// Changes the <paramref name="param"/> key in the <seealso cref="mainMixer"/> with <paramref name="value"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="param"></param>
    /// <param name="key"></param>
    private void HandleSliderChange(float value, string param, SaveManager.E_SaveKeys key)
    {
        float newVol = 0;
        if (value > 0) newVol = Mathf.Log10(value) * volMultiplier;
        else newVol = -80f;

        mainMixer.SetFloat(param, newVol);
        SaveManager.SetSavedKey(key, value);
    }

    public void ChangeMixerPitch(string param, float newPitch) => mainMixer.SetFloat(param, newPitch);
    public void ChangeMainMixerPitch(float newPitch) => ChangeMixerPitch(masterPitchParam, newPitch);
    public void ChangeMusicMixerPitch(float newPitch) => ChangeMixerPitch(musicPitchParam, newPitch);
    public void ChangeSFXMixerPitch(float newPitch) => ChangeMixerPitch(sfxPitchParam, newPitch);

    /// <summary>
    /// Plays the sound with <paramref name="key"/> as tag, stored in <seealso cref="sfxClipsByTag"/>.
    /// </summary>
    /// <param name="key"></param>
    public void Play2DSFX(E_SFXClipsTags key)
    {
        if (!SFXClipsWithTags.TryGetValue(key, out AudioClip result))
        {
            this.Log($"Could not find {key} SFX in dictionnary.", CustomLogger.E_LogType.Error);
            return;
        }
        sfx2DSource.PlayOneShot(result);
    }

    private void PlayMainSceneMusic() => PlayMusic(E_MusicClipsTags.MainScene);


    public void PlayMusic(E_MusicClipsTags musicTag)
    {
        if (musicSource.isPlaying)
        {
            FadeOutMusic(() => PlayMusic(musicTag));
            return;
        }

        if (!MusicClipsWithTags.TryGetValue(musicTag, out AudioClip result))
        {
            this.Log($"Could not find {musicTag} music in dictionnary.", CustomLogger.E_LogType.Error);
            return;
        }

        musicSource.clip = result;
        FadeInMusic();
    }
    public void StopMusic()
        => FadeMusic(E_MusicFadeBehaviour.FadeOut, null);
    public void StopMusic(Action endAction)
        => FadeMusic(E_MusicFadeBehaviour.FadeOut, endAction);

    public void PauseMusic()
        => FadeMusic(E_MusicFadeBehaviour.Pause, null);
    public void PauseMusic(Action endAction)
        => FadeMusic(E_MusicFadeBehaviour.Pause, endAction);

    public void ResumeMusic()
        => FadeMusic(E_MusicFadeBehaviour.Resume, null);
    public void ResumeMusic(Action endAction)
        => FadeMusic(E_MusicFadeBehaviour.Resume, endAction);

    private void FadeInMusic()
        => FadeMusic(E_MusicFadeBehaviour.FadeIn, null);
    private void FadeInMusic(Action endAction)
        => FadeMusic(E_MusicFadeBehaviour.FadeIn, endAction);

    private void FadeOutMusic()
        => FadeMusic(E_MusicFadeBehaviour.FadeOut, null);
    private void FadeOutMusic(Action endAction)
        => FadeMusic(E_MusicFadeBehaviour.FadeOut, endAction);

    private void FadeMusic(E_MusicFadeBehaviour behaviour, Action endAction)
    {
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        musicFadeCoroutine = StartCoroutine(FadeMusicCoroutine(behaviour, endAction));
    }
    private IEnumerator FadeMusicCoroutine(E_MusicFadeBehaviour behaviour, Action endAction)
    {
        bool fadeIn = false;
        switch (behaviour)
        {
            case E_MusicFadeBehaviour.FadeIn:
                fadeIn = true;
                musicSource.Play();
                break;
            case E_MusicFadeBehaviour.Resume:
                fadeIn = true;
                musicSource.UnPause();
                break;
        }

        while (fadeIn ? musicSource.volume < 1 : musicSource.volume > 0)
        {
            musicSource.volume += fadeIn ? Time.deltaTime * musicFadeSpeed : -(Time.deltaTime * musicFadeSpeed);
            yield return null;
        }

        musicFadeCoroutine = null;
        switch (behaviour)
        {
            case E_MusicFadeBehaviour.FadeOut:
                musicSource.Stop();
                break;
            case E_MusicFadeBehaviour.Pause:
                musicSource.Pause();
                break;
        }
        endAction?.Invoke();
    }
}
