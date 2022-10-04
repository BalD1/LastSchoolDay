using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;
using BalDUtilities.Misc;

[CustomEditor(typeof(PlayerCharacter))]
public class ED_PlayerCharacter : Editor
{
    private PlayerCharacter targetScript;

    private bool showDefaultInspector;

    private bool showStats;

    private void OnEnable()
    {
        targetScript = (PlayerCharacter)target;
    }

    public override void OnInspectorGUI()
    {
        showDefaultInspector = EditorGUILayout.Toggle("Show Default Inspector", showDefaultInspector);

        if (showDefaultInspector)
        {
            ReadOnlyDraws.EditorScriptDraw(typeof(ED_PlayerCharacter), this);
            base.OnInspectorGUI();
            return;
        }

        ReadOnlyDraws.EditorScriptDraw(typeof(ED_PlayerCharacter), this);
        ReadOnlyDraws.ScriptDraw(typeof(PlayerCharacter), targetScript, true);

        DrawComponents();
        DrawStats();
    }

    private void DrawComponents()
    {
        SerializedProperty rb = serializedObject.FindProperty("rb");
        EditorGUILayout.PropertyField(rb);

        SerializedProperty sprite = serializedObject.FindProperty("sprite");
        EditorGUILayout.PropertyField(sprite);

        SerializedProperty hitMaterial = serializedObject.FindProperty("hitMaterial");
        EditorGUILayout.PropertyField(hitMaterial);

        SerializedProperty animator = serializedObject.FindProperty("animator");
        EditorGUILayout.PropertyField(animator);
    }

    private void DrawStats()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
        SerializedProperty stats = serializedObject.FindProperty("stats");
        EditorGUILayout.PropertyField(stats);

        GUIStyle style = new GUIStyle(EditorStyles.foldout);
        style.fixedWidth = 0;
        EditorGUI.indentLevel++;
        showStats = EditorGUILayout.Foldout(showStats, "", style);
        EditorGUI.indentLevel--;

        if (showStats)
        {
            SCRPT_EntityStats playerStats = targetScript.GetStats;

            GUI.enabled = false;
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical("GroupBox");

            EditorGUILayout.TextField("Entity Type", playerStats.EntityType);
            EditorGUILayout.FloatField("Max HP", playerStats.MaxHP);
            EditorGUILayout.FloatField("Base Damages", playerStats.BaseDamages);
            EditorGUILayout.FloatField("Attack Range", playerStats.AttackRange);
            EditorGUILayout.FloatField("Attack Cooldown", playerStats.Attack_COOLDOWN);
            EditorGUILayout.FloatField("Invincibility Cooldown", playerStats.Invincibility_COOLDOWN);
            EditorGUILayout.FloatField("Speed", playerStats.Speed);
            EditorGUILayout.IntField("Crit Chances", playerStats.CritChances);
            EditorGUILayout.TextField("Team", EnumsExtension.EnumToString(playerStats.Team));

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            GUI.enabled = true;
        }
    }
}
