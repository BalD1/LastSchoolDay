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
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

        if (Application.isPlaying && GameManager.Instance)
            currentState = GameManager.Instance.GameState;

        GUI.enabled = false;
        EditorGUILayout.TextField("Current GameState", EnumsExtension.EnumToString(currentState));
        GUI.enabled = true;

        EditorGUILayout.BeginHorizontal();                                          // S Hori 1

        GameManager.gameTimeSpeed = EditorGUILayout.FloatField("Gametime Speed", GameManager.gameTimeSpeed);
        if (EditorAssetsHolder.exist)
        {
            EditorAssetsHolder.IconWithSize icon = EditorAssetsHolder.Instance.GetIconData(EditorAssetsHolder.E_IconNames.Back);
            if (GUILayout.Button(icon.image, GUILayout.MaxWidth(icon.maxWidth), GUILayout.MaxHeight(icon.maxHeight)))
            {
                GameManager.gameTimeSpeed = 1;
            }
        }

        EditorGUILayout.EndHorizontal();                                            // E Hori 1
    }

    private void DrawMainMenuInspector()
    {

    }

    private void DrawInGameInspector()
    {
        DrawIGReferences();
    }

    private void DrawIGReferences()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("References", EditorStyles.boldLabel);

        SerializedProperty playerRef = serializedObject.FindProperty("playerRef");
        EditorGUILayout.PropertyField(playerRef);
    }
}
