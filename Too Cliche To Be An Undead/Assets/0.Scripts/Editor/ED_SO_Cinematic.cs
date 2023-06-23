using UnityEditor;
using BalDUtilities.EditorUtils;
using static UnityEditor.Progress;
using static SO_Cinematic;
using UnityEngine;

[CustomEditor(typeof(SO_Cinematic))]
public class ED_SO_Cinematic : Editor
{
	private SO_Cinematic targetScript;
    
    private bool showDefaultInspector = false;

    private SerializedProperty actions;
    private SerializedProperty dialoguePlayers;
    private SerializedProperty movePlayers;

    private void OnEnable()
    {
        targetScript = (SO_Cinematic)target;
        actions = serializedObject.FindProperty(nameof(actions));
        dialoguePlayers = serializedObject.FindProperty(nameof(dialoguePlayers));
        movePlayers = serializedObject.FindProperty(nameof(movePlayers));
    }
    
    public override void OnInspectorGUI()
    {
        showDefaultInspector = EditorGUILayout.Toggle("Show Default Inspector", showDefaultInspector);
        ReadOnlyDraws.EditorScriptDraw(typeof(SO_Cinematic), this);
        if (showDefaultInspector)
        {
            base.DrawDefaultInspector();
            return;
        }
        ReadOnlyDraws.ScriptDraw(typeof(SO_Cinematic), targetScript);

        EditorGUILayout.PropertyField(actions);
        EditorGUILayout.PropertyField(dialoguePlayers);
        EditorGUILayout.PropertyField(movePlayers);

        targetScript.CinematicActions = new S_CinematicActionWithIndex<CA_CinematicAction>[targetScript.CinematicDialogueActions.Length + targetScript.CinematicMovePlayers.Length];

        for (int i = 0; i < targetScript.CinematicDialogueActions.Length; i++)
        {
            targetScript.CinematicActions[targetScript.CinematicDialogueActions[i].IndexInCinematicArray] =
                new S_CinematicActionWithIndex<CA_CinematicAction>(targetScript.CinematicDialogueActions[i].Action, targetScript.CinematicDialogueActions[i].IndexInCinematicArray, i);
        }
        for (int i = 0; i < targetScript.CinematicMovePlayers.Length; i++)
        {
            targetScript.CinematicActions[targetScript.CinematicMovePlayers[i].IndexInCinematicArray]
                = new S_CinematicActionWithIndex<CA_CinematicAction>(targetScript.CinematicMovePlayers[i].Action, targetScript.CinematicMovePlayers[i].IndexInCinematicArray, i);
        }

        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUI.indentLevel++;
        for (int i = 0; i < targetScript.CinematicActions.Length; i++)
        {
            if (targetScript.CinematicActions[i].Action is CA_CinematicDialoguePlayer)
                EditorGUILayout.PropertyField(dialoguePlayers.GetArrayElementAtIndex(targetScript.CinematicActions[i].IndexInSelfArray));
            else if (targetScript.CinematicActions[i].Action is CA_CinematicPlayersMove)
                EditorGUILayout.PropertyField(movePlayers.GetArrayElementAtIndex(targetScript.CinematicActions[i].IndexInSelfArray));
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(targetScript);
    }
}