using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Props", menuName = "Scriptable/Editor/Props")]
public class SCRPT_Props : ScriptableObject
{
    [SerializeField] private PropsByName[] propsByNames;
    public PropsByName[] PropsByNames { get => propsByNames; }

    [System.Serializable]
    public struct PropsByName
    {
        public E_PropName pName;
        public GameObject[] propPFs;
        public EditorAssetsHolder.E_IconNames icon;

        public bool HasMultiplePFs() => propPFs.Length > 1;
    }

    public enum E_PropName
    {
        Locker,
        Desk,
        Table,
    }
}
