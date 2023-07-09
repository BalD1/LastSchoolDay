using BalDUtilities.EditorUtils;
using UnityEditor;

[CustomEditor(typeof(SlideScreenTween))]
[CanEditMultipleObjects]
public class ED_SlideScreenTween : ED_ScreenTweenBase
{
    private SerializedProperty rectTransform;
    private SerializedProperty slideTime;

    private SerializedProperty screenSizeMultiplier;

    private SerializedProperty inHorizontalDir;
    private SerializedProperty inVerticalDir;

    private SerializedProperty outHorizontalDir;
    private SerializedProperty outVerticalDir;

    protected override void OnEnable()
    {
        base.OnEnable();

        rectTransform = serializedObject.FindProperty(nameof(rectTransform));
        slideTime = serializedObject.FindProperty(nameof(slideTime));
        screenSizeMultiplier = serializedObject.FindProperty(nameof(screenSizeMultiplier));
        inHorizontalDir = serializedObject.FindProperty(nameof(inHorizontalDir));
        inVerticalDir = serializedObject.FindProperty(nameof(inVerticalDir));
        outHorizontalDir = serializedObject.FindProperty(nameof(outHorizontalDir));
        outVerticalDir = serializedObject.FindProperty(nameof(outVerticalDir));
    }

    public override void OnInspectorGUI()
    {
        ShowScripts(typeof(ED_ScreenTweenBase), typeof(SlideScreenTween));
        EditorGUILayout.LabelField("Components", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(rectTransform);
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.LabelField("Tween Setup", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(canvasGroup);
        EditorGUILayout.PropertyField(slideTime);
        EditorGUILayout.PropertyField(tweenType);
        EditorGUILayout.PropertyField(screenSizeMultiplier);

        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.LabelField("In Slide");
        SimpleDraws.HorizontalLine();
        EditorGUILayout.PropertyField(inHorizontalDir);
        EditorGUILayout.PropertyField(inVerticalDir);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.LabelField("Out Slide");
        SimpleDraws.HorizontalLine();
        EditorGUILayout.PropertyField(outHorizontalDir);
        EditorGUILayout.PropertyField(outVerticalDir);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
        if (ShowTweenSwitch()) return;
        serializedObject.ApplyModifiedProperties();
    }
}
