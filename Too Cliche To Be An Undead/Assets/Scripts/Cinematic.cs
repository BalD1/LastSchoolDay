using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cinematic", menuName = "Scriptable/Cinematic")]
public class Cinematic : ScriptableObject
{
    [field: SerializeField] public Queue<S_CinematicAction> ActionsQueue { get; private set; }

    [System.Serializable] 
    public class S_CinematicAction
    {
        [field: SerializeField] public float duration { get; private set; }
        [field: SerializeField] public Action actionToPlay { get; private set; }

#if UNITY_EDITOR
        public bool showInEditor;

        public E_Command command;
#endif

        public S_CinematicAction(float _duration, Action _actionToPlay)
        {
            duration = _duration;
            actionToPlay = _actionToPlay;
        }

        public enum E_Command
        {
            MoveCamera,
            StartDialogue,
            Custom,
        }
    }


}