using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;

[CustomEditor(typeof(AreaSpawner))]
public class ED_AreaSpawner : Editor
{
	private AreaSpawner targetScript;
    
    private bool showDefaultInspector = false;
    
	private void OnEnable()
    {
        targetScript = (AreaSpawner)target;
    }
    
    public override void OnInspectorGUI()
    {
        showDefaultInspector = EditorGUILayout.Toggle("Show Default Inspector", showDefaultInspector);
        ReadOnlyDraws.EditorScriptDraw(typeof(AreaSpawner), this);
        if (showDefaultInspector)
        {
            base.DrawDefaultInspector();
            return;
        }
    
        ReadOnlyDraws.ScriptDraw(typeof(AreaSpawner), targetScript);

        SerializedProperty objectsPoolToSpawn = serializedObject.FindProperty("objectsPoolToSpawn");
        EditorGUILayout.PropertyField(objectsPoolToSpawn);

        SerializedProperty symetrical = serializedObject.FindProperty("symetrical");
        EditorGUILayout.PropertyField(symetrical);

        SerializedProperty isValid = serializedObject.FindProperty("isValid");
        EditorGUILayout.PropertyField(isValid);

        if (targetScript.Symetrical == false)
        {
            EditorGUILayout.BeginVertical("GroupBox");

            EditorGUILayout.LabelField("Bounds");

            SerializedProperty minBounds = serializedObject.FindProperty("minBounds");
            EditorGUILayout.PropertyField(minBounds);

            SerializedProperty maxBounds = serializedObject.FindProperty("maxBounds");
            EditorGUILayout.PropertyField(maxBounds);

            SerializedProperty centerOffset = serializedObject.FindProperty("centerOffset");
            EditorGUILayout.PropertyField(centerOffset);

            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.BeginVertical("GroupBox");

            EditorGUILayout.LabelField("Bounds");

            SerializedProperty maxBounds = serializedObject.FindProperty("maxBounds");
            EditorGUILayout.PropertyField(maxBounds);

            SerializedProperty centerOffset = serializedObject.FindProperty("centerOffset");
            EditorGUILayout.PropertyField(centerOffset);

            EditorGUILayout.EndVertical();
        }

        SerializedProperty debugMode = serializedObject.FindProperty("debugMode");
        EditorGUILayout.PropertyField(debugMode);

        serializedObject.ApplyModifiedProperties();
    }
}