using AYellowpaper.SerializedCollections;
using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable/Entity/AnimationData/Players/Base")]
public abstract class SCRPT_AnimationsData<T> : ScriptableObject
{
    [field: SerializeField] public SerializedDictionary<T, S_StateAnimationData> StateAnimationData { get; private set;}

    public bool TryGetAnimationData(T key, out S_StateAnimationData animationData)
    {
        if (!StateAnimationData.TryGetValue(key, out animationData))
        {
            this.Log($"Could not find anim {key} ({typeof(T)}) in {animationData}.", LogsManager.E_LogType.Error);
            return false;
        }
        return true;
    }

    [System.Serializable]
    public struct S_StateAnimationData
    {
#if UNITY_EDITOR
        public string EDITOR_InspectorName;
#endif
        [field: SerializeField] public AnimationReferenceAsset Asset { get; private set; }
        [field: SerializeField] public bool Loop { get; private set; }
    }
}