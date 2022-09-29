using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoundManager))]
public class ED_SoundManager : Editor
{
    private SoundManager targetScript;

    private void OnEnable() => targetScript = (SoundManager)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(10);

        DrawMixerParams();
    }

    private void DrawMixerParams()
    {
        EditorGUILayout.LabelField("Param groups names", EditorStyles.boldLabel);

        GUI.enabled = false;
        EditorGUILayout.TextField("Master param", SoundManager.masterVolParam);
        EditorGUILayout.TextField("Music param", SoundManager.musicVolParam);
        EditorGUILayout.TextField("SFX param", SoundManager.sfxVolparam);
        GUI.enabled = true;
    }
}
