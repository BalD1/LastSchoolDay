using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SCRPT_DropTable))]
public class ED_SCRPT_Droptable : Editor
{
    private SCRPT_DropTable targetScript;
    private float totalWeight;

    private void OnEnable()
    {
        targetScript = (SCRPT_DropTable)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginVertical("GroupBox");

        foreach (var item in targetScript.DropTable)
        {
            totalWeight += item.weight;
        }

        EditorGUILayout.EndVertical();
    }
}
