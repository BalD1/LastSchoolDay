using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PortraitRect", menuName = "Scriptable/UI/PortraitRect")]
public class SCRPT_PortraitsWithRect : ScriptableObject
{
    [field: SerializeField] public Sprite portrait { get; private set; }
    [field: SerializeField] public Vector2 offsetMin {get; private set;}
    [field: SerializeField] public Vector2 offsetMax {get; private set;}
    [field: SerializeField] public Vector2 anchorMin {get; private set;}
    [field: SerializeField] public Vector2 anchorMax { get; private set; }
}