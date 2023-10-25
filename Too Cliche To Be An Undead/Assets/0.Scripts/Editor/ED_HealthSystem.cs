using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;

[CustomEditor(typeof(HealthSystem), true)]
public class ED_HealthSystem : Editor
{
	private HealthSystem targetScript;
    
	private void OnEnable()
    {
        targetScript = (HealthSystem)target;
    }
    
    public override void OnInspectorGUI()
    {
        ReadOnlyDraws.EditorScriptDraw(typeof(ED_HealthSystem), this);
        base.DrawDefaultInspector();

        SimpleDraws.HorizontalLine();

        EditorGUILayout.LabelField("Tick Damages", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical("GrouPBox");
        EditorGUILayout.LabelField("Uniques", EditorStyles.boldLabel);
        foreach (var item in targetScript.UniqueTickDamages)
        {
            DisplayTickDamages(item.Key, item.Value, true);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("GrouPBox");
        EditorGUILayout.LabelField("Stackables", EditorStyles.boldLabel);
        foreach (var item in targetScript.StackableTickDamages)
        {
            EditorGUILayout.LabelField(item.Key, EditorStyles.boldLabel);
            foreach (var tick in item.Value)
            {
                DisplayTickDamages(item.Key, tick, false);
            }
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayTickDamages(string id, NewTickDamages tick, bool displayID)
    {
        GUI.enabled = false;
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginVertical("GroupBox");

        if (displayID) EditorGUILayout.LabelField(id, EditorStyles.boldLabel);

        EditorGUILayout.Toggle("Stackable", tick.Data.Stackable);

        EditorGUILayout.IntField("TicksLifetime", tick.Data.TicksLifetime);
        EditorGUILayout.IntField("Required Ticks To Trigger", tick.Data.RequiredTicksToTrigger);

        EditorGUILayout.FloatField("Damages", tick.Data.Damages);
        EditorGUILayout.IntField("Crit Chances", tick.Data.CritChances);

        EditorGUILayout.IntField("Remaining Ticks", tick.RemainingTicks());
        EditorGUILayout.FloatField("Remaining Time In Seconds", tick.RemainingTimeInSeconds());

        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel--;
        GUI.enabled = true;
    }
}