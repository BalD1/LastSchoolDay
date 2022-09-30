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
    private bool debugOpenedMenusQueue = false;

    private void OnEnable()
    {
        targetScript = (UIManager)target;
    }

    public override void OnInspectorGUI()
    {
        showDefaultInspector = EditorGUILayout.Toggle("Show Default Inspector", showDefaultInspector);
        if (showDefaultInspector)
        {
            ReadOnlyDraws.EditorScriptDraw(typeof(ED_UIManager), this);
            base.OnInspectorGUI();
            return;
        }

        ReadOnlyDraws.EditorScriptDraw(typeof(ED_UIManager), this);
        ReadOnlyDraws.ScriptDraw(typeof(UIManager), targetScript, true);

        DrawGeneralInspector();

        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu))
            DrawMainMenuInspector();
        else
            DrawInGameInspector();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawGeneralInspector()
    {
        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

        debugOpenedMenusQueue = EditorGUILayout.Toggle("Debug Menus Queue", debugOpenedMenusQueue);

        if (debugOpenedMenusQueue)
        {
            SerializedProperty EDITOR_openMenusQueues = serializedObject.FindProperty("EDITOR_openMenusQueues");
            EditorGUILayout.PropertyField(EDITOR_openMenusQueues);

            targetScript.EDITOR_PopulateInspectorOpenedMenus();
        }

        SerializedProperty eventSystem = serializedObject.FindProperty("eventSystem");
        EditorGUILayout.PropertyField(eventSystem);
    }

    private void DrawMainMenuInspector()
    {
        EditorGUILayout.LabelField("Menus", EditorStyles.boldLabel);

        SerializedProperty mainMenu_mainPanel = serializedObject.FindProperty("mainMenu_mainPanel");
        EditorGUILayout.PropertyField(mainMenu_mainPanel);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Buttons Selection", EditorStyles.boldLabel);

        SerializedProperty firstSelectedButton_MainMenu = serializedObject.FindProperty("firstSelectedButton_MainMenu");
        EditorGUILayout.PropertyField(firstSelectedButton_MainMenu);

        SerializedProperty firstSelectedButton_Options = serializedObject.FindProperty("firstSelectedButton_Options");
        EditorGUILayout.PropertyField(firstSelectedButton_Options);
    }

    private void DrawInGameInspector()
    {
        EditorGUILayout.LabelField("Menus", EditorStyles.boldLabel);

        SerializedProperty pauseMenu = serializedObject.FindProperty("pauseMenu");
        EditorGUILayout.PropertyField(pauseMenu);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Buttons Selection", EditorStyles.boldLabel);

        SerializedProperty firstSelectedButton_Options = serializedObject.FindProperty("firstSelectedButton_Options");
        EditorGUILayout.PropertyField(firstSelectedButton_Options);

        SerializedProperty firstSelectedButton_Pause = serializedObject.FindProperty("firstSelectedButton_Pause");
        EditorGUILayout.PropertyField(firstSelectedButton_Pause);
    }
}
