using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;

[CustomEditor(typeof(SoundManager))]
public class ED_AudioTest : Editor
{
	private SoundManager targetScript;
    
    private bool showDefaultInspector = false;
    
	private void OnEnable()
    {
        targetScript = (SoundManager)target;
    }
    
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        if (!EditorApplication.isPlaying) return;
        SimpleDraws.HorizontalLine();

        ReadOnlyDraws.EditorScriptDraw(typeof(TEST_AudioTest), this);

        EditorGUILayout.LabelField("Music");
        EditorGUILayout.BeginVertical("GroupBox");

        foreach (SoundManager.E_MusicClipsTags item in Enum.GetValues(typeof(SoundManager.E_MusicClipsTags)))
        {
            if (GUILayout.Button("Play " + item.ToString(), EditorStyles.miniButton))
            {
                SoundManager.Instance.PlayMusic(item);
            }
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Pause"))
            SoundManager.Instance.PauseMusic();
        if (GUILayout.Button("Resume"))
            SoundManager.Instance.ResumeMusic();
        if (GUILayout.Button("Stop"))
            SoundManager.Instance.StopMusic();

        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("SFX");
        EditorGUILayout.BeginVertical("GroupBox");

        foreach (SoundManager.E_SFXClipsTags item in Enum.GetValues(typeof(SoundManager.E_SFXClipsTags)))
        {
            if (GUILayout.Button(item.ToString(), EditorStyles.miniButton))
            {
                SoundManager.Instance.Play2DSFX(item);
            }
        }

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}