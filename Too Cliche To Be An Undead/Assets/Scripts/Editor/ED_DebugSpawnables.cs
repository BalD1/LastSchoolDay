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

        // foreach trucs
        // switch type
        // case healthpopup
        // draw tout
        // args = 
        // 1 : amount
        // 2 : isheal
        // ...
        // case dummy
        // ...


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
            current.showInEditor = EditorGUILayout.Foldout(current.showInEditor, $"Element {i}");

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
            }

            current.key = (KeyCode)EditorGUILayout.EnumPopup("Key", current.key);
            current.spawnPos = (DebugSpawnables.E_SpawnPos)EditorGUILayout.EnumPopup("Spawn Position", current.spawnPos);


            EditorGUILayout.BeginVertical("GroupBox");

            MixedDraws.ListFoldoutWithEditableSize<string>(ref current.showArgsInEditor, "VarArgs", current.varsArgs);
            EditorGUILayout.Space(5);

            if (current.showArgsInEditor)
            {
                DrawArgsByType(ref current);
            }

            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel--;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("+", GUILayout.MaxWidth(20)))
            {
                DebugSpawnables.SpawnableByKey s = new DebugSpawnables.SpawnableByKey();
                s.varsArgs = new List<string>() { "" };
                targetScript.spawnableByKey.Add(s);
            }
            if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
            {
                targetScript.spawnableByKey.RemoveAt(i);
            }

            EditorGUILayout.EndHorizontal();

            if (i > targetScript.spawnableByKey.Count) continue;
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
                break;

            default:
                break;
        }
        EditorGUI.indentLevel--;
    }
}
