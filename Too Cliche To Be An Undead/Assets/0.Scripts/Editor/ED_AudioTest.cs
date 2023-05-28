using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;

[CustomEditor(typeof(TEST_AudioTest))]
public class ED_AudioTest : Editor
{
	private TEST_AudioTest targetScript;
    
    private bool showDefaultInspector = false;
    
	private void OnEnable()
    {
        targetScript = (TEST_AudioTest)target;
    }
    
    public override void OnInspectorGUI()
    {
        showDefaultInspector = EditorGUILayout.Toggle("Show Default Inspector", showDefaultInspector);
        ReadOnlyDraws.EditorScriptDraw(typeof(TEST_AudioTest), this);
        if (showDefaultInspector)
        {
            base.DrawDefaultInspector();
            return;
        }
        
        ReadOnlyDraws.ScriptDraw(typeof(DebugSpawnables), targetScript);

        SimpleDraws.HorizontalLine();

        SerializedProperty musicClips = serializedObject.FindProperty("musicClips");
        EditorGUILayout.PropertyField(musicClips);

        SerializedProperty musicData = serializedObject.FindProperty("musicData");
        EditorGUILayout.PropertyField(musicData);

        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.BeginVertical("GroupBox");

            foreach (SoundManager.E_MusicClipsTags item in targetScript.MusicClips)
            {
                if (GUILayout.Button(item.ToString(), EditorStyles.miniButton))
                {
                    SoundManager.Instance.PlayMusic(item);
                }
            }

            foreach (SCRPT_MusicData item in targetScript.MusicData)
            {
                if (GUILayout.Button(item.name, EditorStyles.miniButton))
                {
                    SoundManager.Instance.PlayMusic(item);
                }
            }

            EditorGUILayout.EndVertical();
        }

        SimpleDraws.HorizontalLine();

        SerializedProperty sfxClips = serializedObject.FindProperty("sfxClips");
        EditorGUILayout.PropertyField(sfxClips);

        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.BeginVertical("GroupBox");

            foreach (SoundManager.E_SFXClipsTags item in targetScript.SFXClips)
            {
                if (GUILayout.Button(item.ToString(), EditorStyles.miniButton))
                {
                    SoundManager.Instance.Play2DSFX(item);
                }
            }

            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }
}