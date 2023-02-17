using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;
using UnityEditor.TerrainTools;
using System.Text;

[CustomEditor(typeof(Cinematic))]
public class ED_Cinematic : Editor
{
    private Cinematic targetScript;

    private bool showDefaultInspector;

    private bool showActionsQueue;

    private bool showCreateSection;

    private Cinematic.S_CinematicAction.E_Command commandToAdd;
    private float duration;

    private void OnEnable()
    {
        targetScript = (Cinematic)target;
    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        EditorGUILayout.Foldout(showActionsQueue, "Show Action Queue");
        if (showActionsQueue) DisplayActionQueue();

        SimpleDraws.HorizontalLine();

        EditorGUILayout.Foldout(showCreateSection, ("Show Creates"));
        if (showCreateSection) DisplayCreateSection();
    }

    private void DisplayActionQueue()
    {
        EditorGUILayout.BeginVertical("GroupBox");

        Cinematic.S_CinematicAction[] actionQueue = new Cinematic.S_CinematicAction[0];
        targetScript.ActionsQueue.CopyTo(actionQueue, 0);

        for (int i = 0; i < actionQueue.Length; i++)
        {

            EditorGUILayout.Foldout(actionQueue[i].showInEditor, "Element " + i);
            if (actionQueue[i].showInEditor)
            {
                GUI.enabled = false;

                EditorGUILayout.LabelField(actionQueue[i].command.ToString());
                EditorGUILayout.IntField("Duration", actionQueue[i].duration);

                GUI.enabled = true ;
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DisplayCreateSection()
    {
        EditorGUILayout.BeginHorizontal();

        commandToAdd = (Cinematic.S_CinematicAction.E_Command)EditorGUILayout.EnumPopup("Command to add",
                                                                                        commandToAdd);
        duration = EditorGUILayout.FloatField("Duration", duration);

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Add to queue"))
        {
            Action<Vector2> act = null;

            switch (commandToAdd)
            {
                case Cinematic.S_CinematicAction.E_Command.MoveCamera:
                    act = CameraManager.Instance.MoveCamera;
                    break;

                case Cinematic.S_CinematicAction.E_Command.StartDialogue:
                    break;

                case Cinematic.S_CinematicAction.E_Command.Custom:
                    break;

                default:
                    break;
            }

            Cinematic.S_CinematicAction newAction = new Cinematic.S_CinematicAction(duration, act);
            targetScript.ActionsQueue.Enqueue();
        }
    }
}
