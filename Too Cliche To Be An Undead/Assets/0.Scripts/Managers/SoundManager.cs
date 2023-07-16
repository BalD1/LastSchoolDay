using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using BalDUtilities.Misc;
using System;

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

    private LTDescr delayedMusicInvoke;

    [System.Serializable]
    public enum E_SFXClipsTags
    {
        Clic,
        StampChange,
    }

    [System.Serializable]
    public enum E_MusicClipsTags
    {
        MainMenu,
        MainScene,
        BossMusic,
        InLobby,
        TitleScreen,
    }

    [System.Serializable]
    public struct MusicClips
    {
#if UNITY_EDITOR
        public string inEditorName;
#endif
        public E_MusicClipsTags tag;
        public AudioClip clip;
    }

    [System.Serializable]
    public struct SFXClips
    {
#if UNITY_EDITOR
        public string inEditorName;
#endif
        public E_SFXClipsTags tag;
        public AudioClip clip;
    }

    [SerializeField] private MusicClips[] musicClipsByTag;
    [SerializeField] private SFXClips[] sfxClipsByTag;

    [SerializeField] private SCRPT_MusicData bossSpawnMusic;

    private bool isStopping = false;

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

    private void Start()
    {
        LoadSlidersValue();

        ChangeMainMixerPitch(1);
        ChangeMusicMixerPitch(1);
        ChangeSFXMixerPitch(1);

        GameManager.Instance.D_bossFightEnded += TryEndBossMusic;
        if (SpawnersManager.Instance != null)
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
        foreach (var item in sfxClipsByTag)
        {
            if (item.tag.Equals(key))
            {
                sfx2DSource.PlayOneShot(item.clip);
                return;
            }
        }

        Debug.LogError("Could not find " + EnumsExtension.EnumToString(key) + " in sfxClipsByTag");
    }

    /// <summary>
    /// Plays the sound with <paramref name="key"/> as tag, stored in <seealso cref="sfxClipsByTag"/>.
    /// </summary>
    /// <param name="key"></param>
    public void Play2DSFX(string key)
    {
        foreach (var item in sfxClipsByTag)
        {
            if (EnumsExtension.EnumToString(item.tag).Equals(key))
            {
                sfx2DSource.PlayOneShot(item.clip);
                return;
            }
        }

        Debug.LogError("Could not find " + key + " in sfxClipsByTag");
    }

    private void PlayMainSceneMusic() => PlayMusic(E_MusicClipsTags.MainScene);

    public void PauseMusic() => FadeMusic(true, () => musicSource.Pause());
    public void ResumeMusic() => PlayActionAndFadeMusic(false, () => musicSource.UnPause());
    public void StopMusic(Action endAction = null)
    {
        StopDelayedMusicPlay();
        if (isStopping) return;

        isStopping = true;

        if (currentPlayingMusicData == null)
        {
            FadeMusic(true, () =>
            {
                musicSource.Stop();
                endAction?.Invoke();
                isStopping = false;
            });
            return;
        }

        musicSource.Stop();
        AudioClip endClip = currentPlayingMusicData.EndClip;
        if (endClip != null)
        {
            musicSource.PlayOneShot(endClip);
            LeanTween.delayedCall(endClip.length, () =>
            {
                isStopping = false;
                endAction?.Invoke();
            });
            return;
        }
        isStopping = false;
        endAction?.Invoke();
    }
    public void PlayMusic(E_MusicClipsTags musicTag)
    {
        StopDelayedMusicPlay();
        if (musicSource.isPlaying)
        {
            StopMusic(() => PlayMusic(musicTag));
            return;
        }

        AudioClip musicToPlay = null;

        foreach (var item in musicClipsByTag)
        {
            if (item.tag.Equals(musicTag))
            {
                musicToPlay = item.clip;
                break;
            }
        }

        musicSource.clip = musicToPlay;
        FadeMusic(false);
    }
    public void PlayMusic(SCRPT_MusicData musicData)
    {
        if (musicData == null) return;
        StopDelayedMusicPlay();

        if (musicSource.isPlaying)
        {
            StopMusic(() => PlayMusic(musicData));
            return;
        }

        musicSource.volume = 1;
        musicSource.PlayOneShot(musicData.StartClip);
        delayedMusicInvoke = LeanTween.delayedCall(musicData.StartClip.length - musicStartLoopDelayModifier, () =>
        {
            musicSource.clip = musicData.LoopClip;
            musicSource.Play();
        }).setIgnoreTimeScale(true);
    }
    public void PlayBossMusic() => PlayMusic(bossSpawnMusic);
    public void PlayMusicWithFade(E_MusicClipsTags musicToPlay, bool fadeOut)
    {
        musicSource.volume = fadeOut ? 1 : 0;

        LeanTween.value(musicSource.volume, fadeOut ? 0 : 1, musicFadeSpeed).setIgnoreTimeScale(true).setOnUpdate((float val) =>
        {
            musicSource.volume = val;
        });
        foreach (var item in musicClipsByTag)
        {
            if (item.tag.Equals(musicToPlay))
            {
                musicSource.clip = item.clip;
                musicSource.Play();
                break;
            }
        }
    }

    private void FadeMusic(bool fadeOut, Action onComplete = null)
    {
        if (!fadeOut) musicSource.Play();
        LeanTween.value(musicSource.volume, fadeOut ? 0 : 1, musicFadeSpeed).setIgnoreTimeScale(true).setOnUpdate((float val) =>
        {
            musicSource.volume = val;
        }).setOnComplete(onComplete);
    }
    private void PlayActionAndFadeMusic(bool fadeOut, Action beforeAction)
    {
        beforeAction?.Invoke();
        LeanTween.value(musicSource.volume, fadeOut ? 0 : 1, musicFadeSpeed).setIgnoreTimeScale(true).setOnUpdate((float val) =>
        {
            musicSource.volume = val;
        });
    }

    private void StopDelayedMusicPlay()
    {
        if (delayedMusicInvoke == null) return;
        LeanTween.cancel(delayedMusicInvoke.uniqueId);
    }
}
