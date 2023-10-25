using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;

[CustomEditor(typeof(SO_TickDamagesData))]
public class ED_SO_TickDamagesData : Editor
{
	private SO_TickDamagesData targetScript;
    
    private bool showDefaultInspector = false;
    
	private void OnEnable()
    {
        targetScript = (SO_TickDamagesData)target;
    }
    
    public override void OnInspectorGUI()
    {
        ReadOnlyDraws.EditorScriptDraw(typeof(ED_SO_TickDamagesData), this);
        base.DrawDefaultInspector();

        SimpleDraws.HorizontalLine();

        GUI.enabled = false;
        EditorGUILayout.FloatField("Duration in seconds", targetScript.TicksLifetime * TickManager.TICK_TIMER_MAX);
        if (targetScript.RequiredTicksToTrigger > 0)
        {
            float triggersInLifetime = targetScript.TicksLifetime / targetScript.RequiredTicksToTrigger;
            EditorGUILayout.FloatField("Triggers in lifetime", triggersInLifetime);
            EditorGUILayout.LabelField("Damages");
            EditorGUI.indentLevel++;

            EditorGUILayout.FloatField("Min", triggersInLifetime * targetScript.Damages);
            EditorGUILayout.FloatField("Max", targetScript.CritChances > 0 ? triggersInLifetime * (targetScript.Damages * GameManager.CRIT_MULTIPLIER)
                                                                           : triggersInLifetime * targetScript.Damages);

            EditorGUI.indentLevel--;
        }
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }
}