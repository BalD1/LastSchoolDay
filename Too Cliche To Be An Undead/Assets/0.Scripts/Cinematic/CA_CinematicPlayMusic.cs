using UnityEngine;

[System.Serializable]
public class CA_CinematicPlayMusic : CA_CinematicAction
{
    [SerializeField] private SoundManager.E_MusicClipsTags musicToPlay;
    [SerializeField] private bool withFade;

    public CA_CinematicPlayMusic(SoundManager.E_MusicClipsTags _musicToPlay)
        => Setup(musicToPlay);
    public CA_CinematicPlayMusic(SoundManager.E_MusicClipsTags _musicToPlay, bool _withFade)
        => Setup(musicToPlay, _withFade);

    public void Setup(SoundManager.E_MusicClipsTags _musicToPlay, bool _withFade = true)
    {
        this.musicToPlay = _musicToPlay;
        this.withFade = _withFade;
    }

    public override void Execute()
    {
        if (SoundManager.Instance == null)
        {
            this.Log("UIManager Instance was not set. Skipping Cinematic Action.");
            this.ActionEnded(this);
            return;
        }

        if (withFade) SoundManager.Instance.PlayMusicWithFade(musicToPlay, true);
        else SoundManager.Instance.PlayMusic(musicToPlay);

        this.ActionEnded(this);
    }
}
