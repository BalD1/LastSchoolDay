using UnityEditor;

[CustomEditor(typeof(SlideUpScreenTween))]
[CanEditMultipleObjects]
public class ED_SlideUpScreenTween : ED_ScreenTweenBase
{
    private SerializedProperty parentCanvas;
    private SerializedProperty rectTransform;
    private SerializedProperty slideTime;
    private SerializedProperty pingPong;
    private SerializedProperty inDirection;
    private SerializedProperty outDirection;

    protected override void OnEnable()
    {
        base.OnEnable();

        parentCanvas = serializedObject.FindProperty(nameof(parentCanvas));
        rectTransform = serializedObject.FindProperty(nameof(rectTransform));
        slideTime = serializedObject.FindProperty(nameof(slideTime));
        pingPong = serializedObject.FindProperty(nameof(pingPong));
        inDirection = serializedObject.FindProperty(nameof(inDirection));
        outDirection = serializedObject.FindProperty(nameof(outDirection));
    }

    public override void OnInspectorGUI()
    {
        ShowScripts(typeof(ED_ScreenTweenBase), typeof(SlideScreenTween));
        EditorGUILayout.LabelField("Components", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(canvasGroup);
        EditorGUILayout.PropertyField(parentCanvas);
        EditorGUILayout.PropertyField(rectTransform);
        EditorGUILayout.LabelField("Tween Setup", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(slideTime);
        EditorGUILayout.PropertyField(pingPong);
        EditorGUILayout.PropertyField(tweenType);
        EditorGUILayout.PropertyField(inDirection);
        EditorGUILayout.PropertyField(outDirection);
        if (ShowTweenSwitch()) return;
        serializedObject.ApplyModifiedProperties();
    }
}