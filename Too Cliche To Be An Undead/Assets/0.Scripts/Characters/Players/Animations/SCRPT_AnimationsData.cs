using AYellowpaper.SerializedCollections;
using Spine.Unity;
using UnityEngine;

public abstract class SCRPT_AnimationsData<Key> : ScriptableObject
{
    protected bool Intern_TryGetAnimationData(Key key, SerializedDictionary<Key, S_StateAnimationData> dictionary, out S_StateAnimationData animationData)
    {
        if (!dictionary.TryGetValue(key, out animationData))
        {
            this.Log($"Could not find anim {key} ({typeof(Key)}) in {animationData}.", LogsManager.E_LogType.Error);
            return false;
        }
        return true;
    }
    public abstract bool TryGetAnimationData(Key key, out S_StateAnimationData animationData);


    [System.Serializable]
    public struct S_StateAnimationData
    {
        [field: SerializeField] public AnimationReferenceAsset Asset { get; private set; }
        [field: SerializeField] public bool Loop { get; private set; }

        public float AnimationDuration { get => Asset.Animation.Duration; }
    }
}