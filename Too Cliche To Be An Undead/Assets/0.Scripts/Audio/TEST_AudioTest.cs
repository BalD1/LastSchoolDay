using UnityEngine;

public class TEST_AudioTest : MonoBehaviour
{
    [SerializeField] private SoundManager.E_MusicClipsTags[] musicClips;
    [SerializeField] private SCRPT_MusicData[] musicData;
    [SerializeField] private SoundManager.E_SFXClipsTags[] sfxClips;

    public SoundManager.E_MusicClipsTags[] MusicClips { get => musicClips; }
    public SCRPT_MusicData[] MusicData { get => musicData; }
    public SoundManager.E_SFXClipsTags[] SFXClips { get => sfxClips; }
}
