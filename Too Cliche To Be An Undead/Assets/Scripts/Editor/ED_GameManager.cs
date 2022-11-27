using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;
using BalDUtilities.Misc;
using System.Linq;

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
        
        if (!Application.isPlaying) GUI.enabled = false;
        GameManager.MaxAttackers = EditorGUILayout.IntSlider("Max Attackers", GameManager.MaxAttackers, 0, 20);
        if (!Application.isPlaying) GUI.enabled = true;
        EditorGUILayout.HelpBox("Max Attackers étant static, il ne peut être modifié que dans le GameManager, les changements IG sont remis à 0 à chaque fois", MessageType.Warning);

        SerializedProperty spawnPoints = serializedObject.FindProperty("spawnPoints");
        EditorGUILayout.PropertyField(spawnPoints);
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

        SerializedProperty player1Ref = serializedObject.FindProperty("player1Ref");
        EditorGUILayout.PropertyField(player1Ref);

        GUI.enabled = false;
        SerializedProperty playersCount = serializedObject.FindProperty("playersCount");
        EditorGUILayout.PropertyField(playersCount);

        SerializedProperty playersByName = serializedObject.FindProperty("playersByName");
        EditorGUILayout.PropertyField(playersByName);
        GUI.enabled = true;

        GameManager.AcquiredCards = EditorGUILayout.DelayedIntField("Acquired Cards ", GameManager.AcquiredCards);

        SerializedProperty shop = serializedObject.FindProperty("shop");
        EditorGUILayout.PropertyField(shop);

        SerializedProperty fightArena = serializedObject.FindProperty("fightArena");
        EditorGUILayout.PropertyField(fightArena);

        bool checkAllow = EditorGUILayout.Toggle("Allow Enemies", targetScript.AllowEnemies);
        if (checkAllow != targetScript.AllowEnemies) targetScript.AllowEnemies = checkAllow;
    }
}
