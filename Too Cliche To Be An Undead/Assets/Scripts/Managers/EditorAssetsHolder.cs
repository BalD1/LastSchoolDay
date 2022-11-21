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
        Table
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
