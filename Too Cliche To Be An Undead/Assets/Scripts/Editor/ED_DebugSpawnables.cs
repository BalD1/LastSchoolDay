using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;

[CustomEditor(typeof(DebugSpawnables))]
public class ED_DebugSpawnables : Editor
{
    private DebugSpawnables targetScript;

    private bool showDefaultInspector = false;
    private bool showSpawnableByKeyList = true;

    private void OnEnable()
    {
        targetScript = (DebugSpawnables)target;
    }

    public override void OnInspectorGUI()
    {
        showDefaultInspector = EditorGUILayout.Toggle("Show Default Inspector", showDefaultInspector);
        ReadOnlyDraws.EditorScriptDraw(typeof(ED_DebugSpawnables), this);
        if (showDefaultInspector)
        {
            base.DrawDefaultInspector();
            //return;
        }

        ReadOnlyDraws.ScriptDraw(typeof(DebugSpawnables), targetScript);

        DrawSpawnableByKeyList();

    }

    private void DrawSpawnableByKeyList()
    {
        MixedDraws.ListFoldoutWithEditableSize<DebugSpawnables.SpawnableByKey>(ref showSpawnableByKeyList, "SpawnableByKey", targetScript.spawnableByKey);
        if (!showSpawnableByKeyList) return;

        EditorGUILayout.BeginVertical("GroupBox");

        DebugSpawnables.SpawnableByKey current;

        EditorGUI.indentLevel++;
        for (int i = 0; i < targetScript.spawnableByKey.Count; i++)
        {
            current = targetScript.spawnableByKey[i];
            string logScriptType = current.scriptType.ToString();
            current.showInEditor = EditorGUILayout.Foldout(current.showInEditor, $"Element {i} ({logScriptType})");

            if (!current.showInEditor)
            {
                targetScript.spawnableByKey[i] = current;
                continue;
            }

            EditorGUI.indentLevel++;

            DebugSpawnables.E_ScriptType pastType = current.scriptType;

            current.scriptType = (DebugSpawnables.E_ScriptType)EditorGUILayout.EnumPopup("Type", current.scriptType);
            if (current.scriptType != pastType)
            {
                ModifyArgsByType(ref current);
                current = DebugSpawnables.CreateActionBasedOnType(current);
            }

            current.key = (KeyCode)EditorGUILayout.EnumPopup("Key", current.key);
            current.spawnPos = (DebugSpawnables.E_SpawnPos)EditorGUILayout.EnumPopup("Spawn Position", current.spawnPos);

            if (current.scriptType != DebugSpawnables.E_ScriptType.Custom)
            {
                current.showArgsInEditor = EditorGUILayout.Foldout(current.showArgsInEditor, "Var Args");
                EditorGUILayout.Space(5);

                if (current.showArgsInEditor)
                {
                    DrawArgsByType(ref current);
                }
            }
            else
            {
                current.customPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", current.customPrefab, typeof(GameObject), false);
            }

            if (current.spawnPos == DebugSpawnables.E_SpawnPos.CustomPosition)
            {
                current.customPosition = EditorGUILayout.Vector2Field("Custom Position", current.customPosition);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("+", GUILayout.MaxWidth(20)))
            {
                DebugSpawnables.SpawnableByKey s = new DebugSpawnables.SpawnableByKey();
                ModifyArgsByType(ref s);
                targetScript.spawnableByKey.Add(s);
            }

            bool deleted = false;
            if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
            {
                targetScript.spawnableByKey.RemoveAt(i);
                deleted = true;
            }

            EditorGUILayout.EndHorizontal();

            if (!deleted)
                targetScript.spawnableByKey[i] = current;
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.EndVertical();
    }

    private void ModifyArgsByType(ref DebugSpawnables.SpawnableByKey current)
    {
        switch (current.scriptType)
        {
            case DebugSpawnables.E_ScriptType.HealthPopup:
                current.varsArgs = new List<string>(3) { "0.0", "False", "False" };
                break;

            case DebugSpawnables.E_ScriptType.TrainingDummy:
                current.varsArgs = new List<string>(1) { "5.0" };
                break;

            case DebugSpawnables.E_ScriptType.Coin:
                current.varsArgs = new List<string>(1) { "1" };
                break;
        }
    }

    private void DrawArgsByType(ref DebugSpawnables.SpawnableByKey current)
    {
        EditorGUI.indentLevel++;

        switch (current.scriptType)
        {
            case DebugSpawnables.E_ScriptType.HealthPopup:

                if (current.varsArgs[0] == "") current.varsArgs[0] = "0.0";
                float f = Convert.ToSingle(current.varsArgs[0], CultureInfo.InvariantCulture);
                current.varsArgs[0] = Convert.ToString(EditorGUILayout.FloatField("Value", f));

                if (current.varsArgs[1] != "True" && current.varsArgs[1] != "False") current.varsArgs[1] = "False";
                bool bHeal = Convert.ToBoolean(current.varsArgs[1]);
                current.varsArgs[1] = Convert.ToString(EditorGUILayout.Toggle("Is Heal", bHeal));

                if (current.varsArgs[2] != "True" && current.varsArgs[2] != "False") current.varsArgs[2] = "False";
                bool bCrit = Convert.ToBoolean(current.varsArgs[2]);
                current.varsArgs[2] = Convert.ToString(EditorGUILayout.Toggle("Is Crit", bCrit));

                break;

            case DebugSpawnables.E_ScriptType.TrainingDummy:
                if (current.varsArgs[0] == "") current.varsArgs[0] = "0.0";
                float regenTime = Convert.ToSingle(current.varsArgs[0], CultureInfo.InvariantCulture);
                current.varsArgs[0] = Convert.ToString(EditorGUILayout.FloatField("Regen Time", regenTime));
                break;

            case DebugSpawnables.E_ScriptType.Coin:
                if (current.varsArgs[0] == "") current.varsArgs[0] = "1";
                int coinValue = Convert.ToInt32(current.varsArgs[0], CultureInfo.InvariantCulture);
                current.varsArgs[0] = Convert.ToString(EditorGUILayout.IntField("Coin Value", coinValue));
                break;

            default:
                break;
        }
        EditorGUI.indentLevel--;
    }
}
