using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;
using BalDUtilities.Misc;

[CustomEditor(typeof(GameManager))]
public class ED_GameManager : Editor
{
    private GameManager targetScript;
    private GameManager.E_GameState currentState;

    private bool showDefaultInspector = false;

    private void OnEnable()
    {
        targetScript = (GameManager)target;
    }

    public override void OnInspectorGUI()
    {
        showDefaultInspector = EditorGUILayout.Toggle("Show Default Inspector", showDefaultInspector);

        if (showDefaultInspector)
        {
            ReadOnlyDraws.EditorScriptDraw(typeof(ED_GameManager), this);
            DrawDefaultInspector();
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
        if (Application.isPlaying && GameManager.Instance)
            currentState = GameManager.Instance.GameState;

        GUI.enabled = false;
        EditorGUILayout.TextField("Current GameState", EnumsExtension.EnumToString(currentState));
        GUI.enabled = true;
    }

    private void DrawMainMenuInspector()
    {

    }

    private void DrawInGameInspector()
    {

    }
}
