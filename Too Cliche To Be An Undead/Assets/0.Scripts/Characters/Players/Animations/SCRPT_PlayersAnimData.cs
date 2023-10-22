using AYellowpaper.SerializedCollections;
using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayersAnimationData", menuName = "Scriptable/Entity/AnimationData/Players/Base")]
public class SCRPT_PlayersAnimData : SCRPT_AnimationsData<FSM_Player_Manager.E_PlayerState>
{
    [field: SerializeField] public SerializedDictionary<FSM_Player_Manager.E_PlayerState, S_StateAnimationData> StateAnimationData { get; private set;}
    [field: Header("Player general Animations")]

    [field: SerializeField] public Sprite[] arms { get; private set; }

    public override bool TryGetAnimationData(FSM_Player_Manager.E_PlayerState key, out S_StateAnimationData animationData)
    {
        bool result = Intern_TryGetAnimationData(key, StateAnimationData, out animationData);
        return result;
    }
}