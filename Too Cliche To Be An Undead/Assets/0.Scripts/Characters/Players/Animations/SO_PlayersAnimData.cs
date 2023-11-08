using AYellowpaper.SerializedCollections;
using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayersAnimationData", menuName = "Scriptable/Entity/AnimationData/Players/Base")]
public class SO_PlayersAnimData : SO_AnimationsData<FSM_PlayerCharacter.E_PlayerStates>
{
    [field: SerializeField] public SerializedDictionary<FSM_PlayerCharacter.E_PlayerStates, S_StateAnimationData> StateAnimationData { get; private set;}
    [field: Header("Player general Animations")]

    [field: SerializeField] public Sprite[] arms { get; private set; }

    public override bool TryGetAnimationData(FSM_PlayerCharacter.E_PlayerStates key, out S_StateAnimationData animationData)
    {
        bool result = Intern_TryGetAnimationData(key, StateAnimationData, out animationData);
        return result;
    }
}