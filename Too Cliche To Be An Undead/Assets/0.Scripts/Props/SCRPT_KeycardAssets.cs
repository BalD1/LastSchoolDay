using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Keycard Assets", menuName = "Scriptable/Keycard Assets")]
public class SCRPT_KeycardAssets : ScriptableObject
{
    [field: SerializeField] public Sprite[] KeycardSprites { get; private set; }
}