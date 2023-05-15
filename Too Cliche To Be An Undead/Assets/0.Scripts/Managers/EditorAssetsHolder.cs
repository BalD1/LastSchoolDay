using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditorAssetsHolder : MonoBehaviour
{
#if UNITY_EDITOR
    private static EditorAssetsHolder instance;
    public static EditorAssetsHolder Instance
    {
        get
        {
            if (instance == null) Debug.LogError("EditorAssetsHolder could not be found.");

            return instance;
        }
    }
    public static bool exist => instance != null;

    [field: SerializeField] public CanvasGroup WaitForPlayersPanel { get; private set; }
    [field: SerializeField] public GameObject MainButtons { get; private set; }

    private void OnEnable()
    {
        instance = this;
    }

    [System.Serializable]
    public struct IconWithSize
    {
        public E_IconNames iconName;
        public Texture2D image;
        public float maxWidth;
        public float maxHeight;
        public bool showInInspector;
        public bool showExemples;
    }

    public List<IconWithSize> iconsWithSize = new List<IconWithSize>();

    public enum E_IconNames
    {
        Default,
        Back,
        
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

        ReversedLocker,
        ExitDoor,
        Bench,
        Extinguisher,
        Clock,
        Blood,
        Tray,
        OutdoorTable,
    }

    public IconWithSize GetIconData(E_IconNames name)
    {
        foreach (var item in iconsWithSize)
        {
            if (item.iconName.Equals(name)) return item;
        }

        return iconsWithSize[0];
    }
#endif
}
