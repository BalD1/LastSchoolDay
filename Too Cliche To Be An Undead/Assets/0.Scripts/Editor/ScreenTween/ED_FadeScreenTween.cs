using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FadeScreenTween))]
[CanEditMultipleObjects]
public class ED_FadeScreenTween : ED_ScreenTweenBase
{
    private SerializedProperty useCustomAlphaLeanTime;
    private SerializedProperty customAlphaLeanTime;

    protected override void OnEnable()
    {
        base.OnEnable();

        useCustomAlphaLeanTime = serializedObject.FindProperty(nameof(useCustomAlphaLeanTime));
        customAlphaLeanTime = serializedObject.FindProperty(nameof(customAlphaLeanTime));
    }

    public override void OnInspectorGUI()
    {
        ShowScripts(typeof(ED_FadeScreenTween), typeof(FadeScreenTween));
        EditorGUILayout.PropertyField(canvasGroup);
        EditorGUILayout.PropertyField(useCustomAlphaLeanTime);
        EditorGUILayout.PropertyField(customAlphaLeanTime);
        EditorGUILayout.PropertyField(tweenType);
        if (ShowTweenSwitch()) return;
        serializedObject.ApplyModifiedProperties();
    }
}
