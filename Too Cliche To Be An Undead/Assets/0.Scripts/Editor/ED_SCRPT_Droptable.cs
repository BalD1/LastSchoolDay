using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SO_DropTable))]
public class ED_SCRPT_Droptable : Editor
{
    private SO_DropTable targetScript;
    private bool showElementsPercentage;

    private void OnEnable()
    {
        targetScript = (SO_DropTable)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (targetScript.totalDrops < targetScript.DropTable.Length || GUILayout.Button("Calculate weight"))
        {
            CalculateTotalWeight();
        }
        EditorGUILayout.BeginVertical("GroupBox");

        GUI.enabled = false;
        EditorGUI.indentLevel++;
        showElementsPercentage = EditorGUILayout.Foldout(showElementsPercentage, "Elements");
        if (showElementsPercentage)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < targetScript.DropTable.Length; i++)
            {
                EditorGUILayout.TextField($"{targetScript.DropTable[i].editorName} : ", $"{targetScript.DropTable[i].weight / targetScript.totalWeight * 100:F0} % ");
            }
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.TextField("Total weight : ", $"{ targetScript.totalWeight}");
        EditorGUILayout.TextField("Total drops : ", $"{targetScript.totalDrops}");

        GUI.enabled = true;

        EditorGUILayout.EndVertical();
    }

    private void CalculateTotalWeight()
    {
        targetScript.totalDrops = targetScript.DropTable.Length;
        targetScript.totalWeight = 0;
        foreach (SO_DropTable.DropWithWeight drop in targetScript.DropTable)
        {
            targetScript.totalWeight += drop.weight;
        }
    }
}

