using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;
using System.Text;
using static UnityEditor.Progress;

[CustomEditor(typeof(ZombiesScalingManager))]
public class ED_ZombiesScalingManager : Editor
{
	private ZombiesScalingManager targetScript;
    
    private bool showDefaultInspector = false;
    
	private void OnEnable()
    {
        targetScript = (ZombiesScalingManager)target;
    }
    
    public override void OnInspectorGUI()
    {
        showDefaultInspector = EditorGUILayout.Toggle("Show Default Inspector", showDefaultInspector);
        ReadOnlyDraws.EditorScriptDraw(typeof(ZombiesScalingManager), this);
        if (showDefaultInspector)
        {
            base.DrawDefaultInspector();

            SetIDs();

            return;
        }
        
        ReadOnlyDraws.ScriptDraw(typeof(DebugSpawnables), targetScript);

        serializedObject.ApplyModifiedProperties();
    }

    private void SetIDs()
    {
        for (int i = 0; i < targetScript.ModifiersByStamps.Length; i++)
        {
            foreach (var item in targetScript.ModifiersByStamps[i].Modifiers)
            {
                StringBuilder sb = new StringBuilder("SCALE_");

                sb.Append(i);
                sb.Append("_");
                sb.Append(item.StatType);

                item.SetID(sb.ToString());
            }
        }
    }
}