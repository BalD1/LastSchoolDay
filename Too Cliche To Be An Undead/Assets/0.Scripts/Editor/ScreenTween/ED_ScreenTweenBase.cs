using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;
using System;

[CustomEditor(typeof(BaseScreenTween))]
[CanEditMultipleObjects]
public class ED_ScreenTweenBase : Editor
{
	protected BaseScreenTween targetScript;
    protected SerializedProperty canvasGroup;
    protected SerializedProperty tweenType;
    private bool scriptsToggle;
    private bool switchTweenToggle = true;
    private bool showDefaultInspector = false;


    protected virtual void OnEnable()
    {
        targetScript = (BaseScreenTween)target;
        canvasGroup = serializedObject.FindProperty(nameof(canvasGroup));
        tweenType = serializedObject.FindProperty(nameof(tweenType));
        switchTweenToggle = true;
    }
    
    public override void OnInspectorGUI()
    {
        ShowScripts(typeof(ED_ScreenTweenBase), typeof(BaseScreenTween));

        EditorGUILayout.LabelField("Components", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(canvasGroup);
        EditorGUILayout.LabelField("Tween Setup", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(tweenType);

        if (ShowTweenSwitch()) return;

        serializedObject.ApplyModifiedProperties();
    }

    protected void ShowScripts(Type editorScriptType, Type scriptType)
    {
        scriptsToggle = EditorGUILayout.Foldout(scriptsToggle, "Scripts");
        if (scriptsToggle)
        {
            EditorGUI.indentLevel++;
            showDefaultInspector = EditorGUILayout.Toggle("Show Default Inspector", showDefaultInspector);
            ReadOnlyDraws.EditorScriptDraw(editorScriptType, this);
            if (showDefaultInspector)
            {
                base.DrawDefaultInspector();
                return;
            }

            ReadOnlyDraws.ScriptDraw(scriptType, targetScript);
            EditorGUI.indentLevel--;
        }
    }

    protected bool ShowTweenSwitch()
    {
        switchTweenToggle = EditorGUILayout.Foldout(switchTweenToggle, "Tweens");
        if (switchTweenToggle)
        {
            EditorGUI.indentLevel++;
            if (CheckForNewTween<FadeScreenTween>("Fade")) return true;
            if (CheckForNewTween<SlideScreenTween>("Slide")) return true;
            if (CheckForNewTween<ScaleScreenTween>("Scale")) return true;
            if (GUILayout.Button("Remove"))
            {
                UIScreenBase screen = targetScript.gameObject.GetComponent<UIScreenBase>();
                screen.RemoveScreenTween(targetScript);
                Undo.DestroyObjectImmediate(targetScript);
                EditorUtility.SetDirty(screen);
                return true;
            }
            EditorGUI.indentLevel--;
        }

        return false;
    }

    private bool CheckForNewTween<T>(string typeName) where T : BaseScreenTween
    {
        if (targetScript as T != null) return false;
        if (!GUILayout.Button("Switch to " +  typeName)) return false;

        UIScreenBase screen = targetScript.gameObject.GetComponent<UIScreenBase>();
        screen.RemoveScreenTween(targetScript);
        T fst = Undo.AddComponent<T>(targetScript.gameObject);
        fst.Reset();
        fst.Awake();
        Undo.DestroyObjectImmediate(targetScript);
        EditorUtility.SetDirty(screen);
        EditorUtility.SetDirty(fst);

        return true;
    }
}