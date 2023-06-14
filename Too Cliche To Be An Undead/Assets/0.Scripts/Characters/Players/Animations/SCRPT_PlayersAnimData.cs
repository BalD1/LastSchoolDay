using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayersAnimationData", menuName = "Scriptable/Entity/AnimationData/Player")]

public class SCRPT_PlayersAnimData : SCRPT_AnimationsData
{
    [field: Header("Player general Animations")]

    [field: SerializeField] public AnimationReferenceAsset dashAnim { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset skillTransitionAnim { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset skillIdleAnimSide { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset skillWalkAnimSide { get; private set; }

    [field: SerializeField] public AnimationReferenceAsset skillIdleAnimUp { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset skillIdleAnimDown { get; private set; }

    [field: SerializeField] public S_StateAnimationData[] StateAnimation { get; private set; }

    [System.Serializable]
    public struct S_StateAnimationData
    {
#if UNITY_EDITOR
        public string EDITOR_InspectorName;
#endif
        [field: SerializeField] public FSM_Player_Manager.E_PlayerState Key { get; private set; }
        [field: SerializeField] public AnimationReferenceAsset Asset { get; private set; }
        [field: SerializeField] public bool Loop { get; private set; }
    }

    [field: SerializeField] public Sprite[] arms { get; private set; }
}