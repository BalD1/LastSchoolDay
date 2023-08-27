using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;

[CustomEditor(typeof(CameraManager))]
public class ED_CameraManager : Editor
{
	private CameraManager targetScript;
    
    private bool showDefaultInspector = false;

    private float duration;
    private float intensity;
    
	private void OnEnable()
    {
        targetScript = (CameraManager)target;
    }
    
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        SimpleDraws.HorizontalLine();

        ReadOnlyDraws.EditorScriptDraw(typeof(CameraManager), this);
        if (!Application.isPlaying) return;
        if (GUILayout.Button("Play Shake"))
        {
            CameraManager.Instance.ShakeCamera(intensity, duration);
        }

        duration = EditorGUILayout.FloatField("Duration", duration);
        intensity = EditorGUILayout.FloatField("Intensity", intensity);

        serializedObject.ApplyModifiedProperties();
    }
}