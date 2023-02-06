using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PopupComponents", menuName = "Scriptable/TextPopupComps")]
public class SCRPT_TextPopupComponents : ScriptableObject
{
    [System.Serializable]
    public struct HitComponents
    {
        [field: SerializeField] public Vector3 speedMovements { get; private set; }
        [field: SerializeField] public float lifetime { get; private set; }
        [field: SerializeField] public float disapearSpeed { get; private set; }
        [field: SerializeField] public float fontSize { get; private set; }
        [field: SerializeField] public float increaseScaleAmount { get; private set; }
        [field: SerializeField] public float decreaseScaleAmount { get; private set; }
        [field: SerializeField] public Color color { get; private set; }
    }

    [field: SerializeField] public HitComponents baseComponents { get; private set; }
    [field: SerializeField] public HitComponents StunComponents { get; private set; }
    [field: SerializeField] public HitComponents ItemPickupComponents { get; private set; }
}