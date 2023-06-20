using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;

[CustomEditor(typeof(SO_Cinematic))]
public class ED_SO_Cinematic : Editor
{
	private SO_Cinematic targetScript;
    
    private bool showDefaultInspector = false;
    private SerializedProperty cinematicArray;
    
	private void OnEnable()
    {
        targetScript = (SO_Cinematic)target;
        cinematicArray = serializedObject.FindProperty("actions");
    }
    
    public override void OnInspectorGUI()
    {
        showDefaultInspector = EditorGUILayout.Toggle("Show Default Inspector", showDefaultInspector);
        ReadOnlyDraws.EditorScriptDraw(typeof(SO_Cinematic), this);
        if (showDefaultInspector)
        {
            base.DrawDefaultInspector();
            return;
        }
        
        ReadOnlyDraws.ScriptDraw(typeof(SO_Cinematic), targetScript);

        foreach (var item in targetScript.cinematicActions)
        {
            if (item is CinematicDialoguePlayer)
                EditorGUILayout.LabelField("Cinematic");
            else if (item is CinematicPlayersMove)
                EditorGUILayout.LabelField("Move");
        }
        Debug.Log(cinematicArray.arraySize);
        
        serializedObject.ApplyModifiedProperties();
    }
}