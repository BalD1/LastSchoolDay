using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Props", menuName = "Scriptable/Editor/Props")]
public class SCRPT_Props : ScriptableObject
{
    [SerializeField] private PropsByName[] propsByNames;
    public PropsByName[] PropsByNames { get => propsByNames; }

    [SerializeField] private DecorationByName[] decorationsByNames;
    public DecorationByName[] DecorationsByNames { get => decorationsByNames; }

    [System.Serializable]
    public struct PropsByName
    {
        public E_PropName pName;
        public EditorAssetsHolder.E_IconNames icon;
        public GameObject[] propPFs;

        public bool HasMultiplePFs() => propPFs.Length > 1;
    }

    [System.Serializable]
    public struct DecorationByName
    {
        public E_PropName pName;
        public EditorAssetsHolder.E_IconNames icon;
        public Sprite[] sprites;

        public bool HasMultipleSprites() => sprites.Length > 1;
    }

    public enum E_PropName
    {
        Default,

        Locker,
        Desk,
        Table,
        Bag,
        Bin,
        Chair,
        Plant,

        Blackboard,
        Whiteboard,
        Flag,
        Paper,
        Poster,
        Window,
        Aquarium,
        Tree,
        Camera,
        VendingMachine,

        GymnasiumBallsHolder,
        GymnasiumBall,
        BloodyBottles,
        EagleFlag,
        EagleStrips,
        BasketHoop,

    }   
}
