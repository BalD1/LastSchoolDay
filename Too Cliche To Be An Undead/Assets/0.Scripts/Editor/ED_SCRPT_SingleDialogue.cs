using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using BalDUtilities.EditorUtils;

[CustomEditor(typeof(SCRPT_SingleDialogue))]
public class ED_SCRPT_SingleDialogue : Editor
{
    private SCRPT_SingleDialogue targetScript;

    GUILayoutOption[] opts;

    private string searchIndexOf = "";
    private int dialogueLine;
    private int indexResult;

    private void OnEnable()
    {
        targetScript = (SCRPT_SingleDialogue)target;
        opts = new GUILayoutOption[2];
        opts[0] = GUILayout.ExpandWidth(false);
        opts[1] = GUILayout.MaxWidth(200);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SimpleDraws.HorizontalLine();

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Find Index Of"))
        {
            string textLine = targetScript.dialogueLines[dialogueLine].textLine;
            indexResult = textLine.IndexOf(searchIndexOf);
        }

        searchIndexOf = EditorGUILayout.TextField(searchIndexOf, opts);
        GUILayout.FlexibleSpace();
        dialogueLine = EditorGUILayout.IntField("In line", dialogueLine, opts);
        GUILayout.EndHorizontal();
        EditorGUILayout.IntField("Result", indexResult);
    }
}
