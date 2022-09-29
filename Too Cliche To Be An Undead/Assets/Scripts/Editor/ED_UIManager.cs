using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;

[CustomEditor(typeof(UIManager))]
public class ED_UIManager : Editor
{
    private UIManager targetScript;

    private bool showDefaultInspector = false;

    private void OnEnable()
    {
        targetScript = (UIManager)target;
    }

    public override void OnInspectorGUI()
    {
        showDefaultInspector = EditorGUILayout.Toggle("Show Default Inspector", showDefaultInspector);
        if (showDefaultInspector)
        {
            base.OnInspectorGUI();
            return;
        }

        ReadOnlyDraws.ScriptDraw(typeof(UIManager), targetScript, true);

        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu))
            DrawMainMenuInspector();
        else
            DrawInGameInspector();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawMainMenuInspector()
    {
        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

        SerializedProperty eventSystem = serializedObject.FindProperty("eventSystem");
        EditorGUILayout.PropertyField(eventSystem);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Buttons Selection", EditorStyles.boldLabel);

        SerializedProperty firstSelectedButton_MainMenu = serializedObject.FindProperty("firstSelectedButton_MainMenu");
        EditorGUILayout.PropertyField(firstSelectedButton_MainMenu);

        SerializedProperty firstSelectedButton_Options = serializedObject.FindProperty("firstSelectedButton_Options");
        EditorGUILayout.PropertyField(firstSelectedButton_Options);
    }

    private void DrawInGameInspector()
    {

    }
}
