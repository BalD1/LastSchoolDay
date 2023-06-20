using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCinematic", menuName = "Scriptable/Cinematic")]
public class SO_Cinematic : ScriptableObject
{
    [SerializeField] private CinematicAction[] actions = new CinematicAction[0];
    public CinematicAction[] cinematicActions { get => actions; }

}