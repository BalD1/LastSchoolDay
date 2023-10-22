using AYellowpaper.SerializedCollections;
using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable/Entity/AnimationData/Enemies/Base")]
public class SCRPT_ZombiesAnimData : SCRPT_AnimationsData<FSM_NZ_Manager.E_NZState>
{
    [field: SerializeField] public SerializedDictionary<FSM_NZ_Manager.E_NZState, S_StateAnimationData> StateAnimationData { get; private set;}
    
    public override bool TryGetAnimationData(FSM_NZ_Manager.E_NZState key, out S_StateAnimationData animationData)
    {
        bool result = Intern_TryGetAnimationData(key, StateAnimationData, out animationData);
        return result;
    }
}