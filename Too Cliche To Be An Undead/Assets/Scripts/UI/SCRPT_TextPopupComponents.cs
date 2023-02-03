using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PopupComponents", menuName = "Scriptable/TextPopupComps")]
public class SCRPT_TextPopupComponents : ScriptableObject
{
    [System.Serializable]
    public struct HitComponents
    {
        public Vector3 speedMovements;
        public float lifetime;
        public float disapearSpeed;
        public float fontSize;
        public float increaseScaleAmount;
        public float decreaseScaleAmount;
        public Color color;
    }

    [field: SerializeField] public HitComponents baseComponents { get; private set; }
    [field: SerializeField] public HitComponents StunComponents { get; private set; }
    [field: SerializeField] public HitComponents ItemPickupComponents { get; private set; }
}