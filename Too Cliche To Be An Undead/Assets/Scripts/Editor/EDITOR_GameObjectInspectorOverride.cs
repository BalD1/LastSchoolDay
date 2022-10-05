using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using System.Reflection;
using BalDUtilities.EditorUtils;

[CustomEditor(typeof(GameObject))]
public class EDITOR_GameObjectInspectorOverride : Editor
{

    private System.Type inspectorType;
    private Editor editorInstance;
    private _MethodInfo defaultHeaderGUI;

    private bool newWindowLock = true;

    public EDITOR_GameObjectInspectorOverride()
    {
        inspectorType = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.GameObjectInspector");
        defaultHeaderGUI = inspectorType.GetMethod("OnHeaderGUI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    protected override void OnHeaderGUI()
    {
        editorInstance = Editor.CreateEditor(target, inspectorType);
        defaultHeaderGUI.Invoke(editorInstance, null);
        EditorGUILayout.BeginHorizontal();          // S Hori 1

        if (GUILayout.Button("Open in new window"))
        {
            OpenWindow.OpenCurrentInspectorInNewWindow(newWindowLock);
        }
        newWindowLock = EditorGUILayout.Toggle("is locked", newWindowLock);

        EditorGUILayout.EndHorizontal();            // E Hori 1
    }

    public override void OnInspectorGUI() { }

}
