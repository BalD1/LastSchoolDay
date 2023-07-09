using UnityEditor;

[CustomEditor(typeof(ScaleScreenTween))]
[CanEditMultipleObjects]
public class ED_ScaleScreenTween : ED_ScreenTweenBase
{
    private SerializedProperty rectTransform;
    private SerializedProperty maxScale;
    private SerializedProperty minScale;
    private SerializedProperty scaleTime;

    protected override void OnEnable()
    {
        base.OnEnable();

        rectTransform = serializedObject.FindProperty(nameof(rectTransform));
        maxScale = serializedObject.FindProperty(nameof(maxScale));
        minScale = serializedObject.FindProperty(nameof(minScale));
        scaleTime = serializedObject.FindProperty(nameof(scaleTime));
    }

    public override void OnInspectorGUI()
    {
        ShowScripts(typeof(ED_ScaleScreenTween), typeof(ScaleScreenTween));
        EditorGUILayout.LabelField("Components", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(canvasGroup);
        EditorGUILayout.PropertyField(rectTransform);
        EditorGUILayout.LabelField("Tween Setup", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(maxScale);
        EditorGUILayout.PropertyField(minScale);
        EditorGUILayout.PropertyField(scaleTime);
        EditorGUILayout.PropertyField(tweenType);
        if (ShowTweenSwitch()) return;
        serializedObject.ApplyModifiedProperties();
    }
}
