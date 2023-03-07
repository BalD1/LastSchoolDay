using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomMinimap", menuName = "Scriptable/LD/RoomMinimap")]
public class SCRPT_RoomMinimap : ScriptableObject
{
    [field: SerializeField] public Color UndiscoveredColor { get; private set; }
    [field: SerializeField] public Color DiscoveredColor { get; private set; }

    [field: SerializeField] public float DiscoveryColorBlendSpeed { get; private set; }
}