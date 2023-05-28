using UnityEngine;

[CreateAssetMenu(fileName = "NewMusicData", menuName = "Scriptable/Audio/MusicData")]
public class SCRPT_MusicData : ScriptableObject
{
    [field: SerializeField] public AudioClip StartClip { get; private set; }
    [field: SerializeField] public AudioClip LoopClip { get; private set; }
    [field: SerializeField] public AudioClip EndClip { get; private set; }
}