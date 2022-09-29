using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;

[CustomEditor(typeof(SaveManager))]
public class ED_SaveManager : Editor
{
    private SaveManager targetScript;

    private bool showConfirm;
    private bool showResult;
    private bool positiveResult;

    public enum E_KeysToDelete
    {
        All,
        Volume,
    }
    private E_KeysToDelete keyToDelete;

    private void OnEnable() => targetScript = (SaveManager)target;

    public override void OnInspectorGUI()
    {
        ReadOnlyDraws.EditorScriptDraw(typeof(ED_SaveManager), this);
        base.OnInspectorGUI();

        DrawDeleteKey();
        DrawResult();
    }

    private void DrawDeleteKey()
    {
        EditorGUILayout.BeginHorizontal();                  // S HORI 1

        if (!showConfirm && !showResult)
        {
            if (GUILayout.Button("Delete Keys")) showConfirm = true;

            keyToDelete = (E_KeysToDelete)EditorGUILayout.EnumPopup(keyToDelete);
        }

        EditorGUILayout.EndHorizontal();                    // E HORI 1

        if (showConfirm && !showResult)
        {
            EditorGUILayout.LabelField("Are you sure ?");

            EditorGUILayout.BeginHorizontal();                  // S HORI 2

            if (GUILayout.Button("No")) showConfirm = false;
            if (GUILayout.Button("Yes"))
            {
                showResult = true;
                switch (keyToDelete)
                {
                    case E_KeysToDelete.All:
                        DeleteVolumeKeys();
                        positiveResult = true;
                        break;

                    case E_KeysToDelete.Volume:
                        DeleteVolumeKeys();
                        positiveResult = true;
                        break;

                    default:
                        positiveResult = false;
                        break;
                }
            }

            EditorGUILayout.EndHorizontal();                    // E HORI 2
        }
    }

    private void DrawResult()
    {
        if (!showResult) return;

        EditorGUILayout.LabelField(positiveResult ? "Successfuly deleted keys" : "Could not delete keys");
        if (GUILayout.Button("Confirm"))
        {
            showResult = false;
            showConfirm = false;
        }
    }

    private void EndResult() => showResult = false;

    private void DeleteVolumeKeys()
    {
        showConfirm = false;

        SaveManager.DeleteKey(SaveManager.E_SaveKeys.F_MasterVolume);
        SaveManager.DeleteKey(SaveManager.E_SaveKeys.F_MusicVolume);
        SaveManager.DeleteKey(SaveManager.E_SaveKeys.F_SFXVolume);
    }
}
