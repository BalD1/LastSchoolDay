using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;

[CustomEditor(typeof(StatsHandler))]
public class ED_StatsHandler : Editor
{
	private StatsHandler targetScript;
    
    private bool showDefaultInspector = false;
    
	private void OnEnable()
    {
        targetScript = (StatsHandler)target;
    }
    
    public override void OnInspectorGUI()
    {
        ReadOnlyDraws.EditorScriptDraw(typeof(ED_StatsHandler), this);
        base.DrawDefaultInspector();

        DrawStats();
        DrawModifiersLists();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawStats()
    {
        GUI.enabled = false;
        SimpleDraws.HorizontalLine();
        if (targetScript.BaseStats != null)
        {
            EditorGUILayout.BeginVertical("GroupBox");
            EditorGUILayout.LabelField("Base Stats", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            foreach (var item in targetScript.BaseStats.Stats)
            {
                EditorGUILayout.FloatField(item.Key.ToString(), item.Value.Value);
            }
            EditorGUILayout.EndVertical();
        }

        DrawDictionary(targetScript.BrutFinalStats, "Brut Stats", true, false);
        DrawDictionary(targetScript.PermanentBonusStats, "Permanent Stats", true, true);
        DrawDictionary(targetScript.TemporaryBonusStats, "Temporary Stats", false, false);
        GUI.enabled = true;
    }

    private void DrawModifiersLists()
    {
        SimpleDraws.HorizontalLine();

        EditorGUILayout.LabelField("Modifiers", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.LabelField("Uniques", EditorStyles.boldLabel);
        foreach (var item in targetScript.UniqueStatsModifiers)
        {
            DrawSingleModifier(item.Key, item.Value, true);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.LabelField("Stackables", EditorStyles.boldLabel);
        foreach (var item in targetScript.StackableStatsModifiers)
        {
            EditorGUILayout.LabelField(item.Key, EditorStyles.boldLabel);
            foreach (var modifier in item.Value)
            {
                DrawSingleModifier(name, modifier, false);
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawSingleModifier(string id, NewStatsModifier modifier, bool displayID)
    {
        GUI.enabled = false;
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginVertical("GroupBox");

        if (displayID) EditorGUILayout.LabelField(id, EditorStyles.boldLabel);

        EditorGUILayout.Toggle("Stackable", modifier.Data.Stackable);

        EditorGUILayout.FloatField(modifier.Data.StatType.ToString(), modifier.Data.Amount);

        EditorGUILayout.Toggle("Temporary", modifier.Data.Temporary);
        if (modifier.Data.Temporary)
        {
            EditorGUILayout.FloatField("Lifetime", modifier.Data.TicksLifetime);
        }

        EditorGUILayout.IntField("Remaning Ticks", modifier.RemainingTicks());
        EditorGUILayout.FloatField("Remaining time in seconds", modifier.RemainingTimeInSeconds());

        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel--;
        GUI.enabled = true;
    }

    private void DrawDictionary(Dictionary<IStatContainer.E_StatType, float> dic, string label, bool withMax, bool maxMinusBase)
    {
        if (dic == null) return;
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        EditorGUILayout.Space();

        foreach (var item in dic)
        {
            if (targetScript.BaseStats != null)
            {
                if (withMax && targetScript.BaseStats.TryGetHigherAllowedValue(item.Key, out float higher))
                {
                    if (maxMinusBase) higher -= targetScript.BaseStats.Stats[item.Key].Value;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.FloatField(item.Key.ToString(), item.Value);
                    EditorGUILayout.FloatField(higher);
                    EditorGUILayout.EndHorizontal();
                }
                else
                    EditorGUILayout.FloatField(item.Key.ToString(), item.Value);
            }
            else
                EditorGUILayout.FloatField(item.Key.ToString(), item.Value);

        }
        EditorGUILayout.EndVertical();
    }
}