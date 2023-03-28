using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using BalDUtilities.Misc;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null) Debug.LogError("SoundManager instance not found.");
            return instance;
        }
    }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfx2DSource;
    [SerializeField] private AudioSource[] soundsSource;

    [SerializeField] private AudioMixer mainMixer;

    public const string masterVolParam = "MasterVol";
    public const string musicVolParam = "MusicVol";
    public const string sfxVolparam = "SFXVol";

    [Header("Sliders")]
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Space]
    [SerializeField] private float volMultiplier = 30f;











    [System.Serializable]
    public enum E_SFXClipsTags
    {
        Clic,
    }

    [System.Serializable]
    public enum E_MusicClipsTags
    {
        MainMenu,
        MainScene,
        BossMusic,
        InLobby,
    }

    [System.Serializable]
    public struct ClipByTag<T>
    {
#if UNITY_EDITOR
        public string inEditorName;
#endif
        public T tag;
        public AudioClip clip;
    }
    [SerializeField] private ClipByTag<E_MusicClipsTags>[] musicClipsByTag;
    [SerializeField] private ClipByTag<E_SFXClipsTags>[] sfxClipsByTag;











    [System.Serializable]
    public struct SFXClipByTag
    {
#if UNITY_EDITOR
        public string inEditorName;
#endif
        public E_SFXClipsTags tag;
        public AudioClip clip;
    }

    [System.Serializable]
    public struct MusicClipByTag
    {
#if UNITY_EDITOR
        public string inEditorName;
#endif
        public E_MusicClipsTags tag;
        public AudioClip clip;
    }

    [SerializeField] private MusicClipByTag[] musicClipsByTag2;
    [SerializeField] private SFXClipByTag[] sfxClipsByTag2;












    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadSlidersValue();

        GameManager.Instance.D_bossFightStarted += TryStartBossMusic;
        GameManager.Instance.D_bossFightEnded += TryEndBossMusic;
    }

    private void TryStartBossMusic()
    {
        if (GameManager.Instance.currentAliveBossesCount > 1) return;

        PlayMusic(E_MusicClipsTags.BossMusic);
    }

    private void TryEndBossMusic()
    {
        if (GameManager.Instance.currentAliveBossesCount > 0) return;

        PlayMusic(E_MusicClipsTags.MainScene);
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

    /// <summary>
    /// Plays the sound with <paramref name="key"/> as tag, stored in <seealso cref="sfxClipsByTag"/>.
    /// </summary>
    /// <param name="key"></param>
    public void Play2DSFX(E_SFXClipsTags key)
    {
        foreach (var item in sfxClipsByTag2)
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
        foreach (var item in sfxClipsByTag2)
        {
            if (EnumsExtension.EnumToString(item.tag).Equals(key))
            {
                sfx2DSource.PlayOneShot(item.clip);
                return;
            }
        }

        Debug.LogError("Could not find " + key + " in sfxClipsByTag");
    }

    public void PauseMusic() => musicSource.Pause();
    public void ResumeMusic() => musicSource.UnPause();
    public void StopMusic() => musicSource.Stop();
    public void PlayMusic(E_MusicClipsTags musicTag)
    {
        foreach (var item in musicClipsByTag2)
        {
            if (item.tag.Equals(musicTag))
            {
                musicSource.clip = item.clip;
                musicSource.Play();
            }
        }
    }
}
