using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;
using BalDUtilities.Misc;
using UnityEditor.Sprites;

[CustomEditor(typeof(EditorAssetsHolder))]
public class EDITOR_Assets : Editor
{
    private EditorAssetsHolder targetScript;
    private bool drawExemples = false;

    private bool showIconsWithSize = false;

    private void OnEnable()
    {
        targetScript = (EditorAssetsHolder)target;
    }

    public override void OnInspectorGUI()
    {
        ReadOnlyDraws.EditorScriptDraw(typeof(EDITOR_Assets), this);
        base.OnInspectorGUI();

        drawExemples = EditorGUILayout.Toggle("Show exemples", drawExemples);
        if (drawExemples) DrawIconsWithSize();
    }

    private void DrawIconsWithSize()
    {
        MixedDraws.ListFoldoutWithEditableSize<EditorAssetsHolder.IconWithSize>(ref showIconsWithSize, "Icons with size", EditorAssetsHolder.Instance.iconsWithSize);
        if (!showIconsWithSize || EditorAssetsHolder.Instance.iconsWithSize.Count <= 0) return;

        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUI.indentLevel++;

        var item = EditorAssetsHolder.Instance.iconsWithSize[0];
        for (int i = 0; i < EditorAssetsHolder.Instance.iconsWithSize.Count; i++)
        {
            item = EditorAssetsHolder.Instance.iconsWithSize[i];

            item.showInInspector = EditorGUILayout.Foldout(item.showInInspector, EnumsExtension.EnumToString(item.iconName));

            if (!item.showInInspector) continue;

            EditorGUI.indentLevel++;

            item.iconName = (EditorAssetsHolder.E_IconNames)EditorGUILayout.EnumPopup("EditorName", item.iconName);
            item.image = (Texture2D)EditorGUILayout.ObjectField("Icon", item.image, typeof(Texture2D), false);
            item.maxWidth = EditorGUILayout.FloatField("Max Width", item.maxWidth);
            item.maxHeight = EditorGUILayout.FloatField("Max Height", item.maxHeight);

            GUILayout.Button(item.image, GUILayout.MaxWidth(item.maxWidth), GUILayout.MaxHeight(item.maxHeight));

            item.showExemples = EditorGUILayout.Foldout(item.showExemples, "Show Exemples");

            if (item.showExemples)
            {

                EditorGUI.indentLevel++;

                EditorGUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();                                  // S Hori 1
                EditorGUILayout.TextField("Exemple string", "Lorem ipsum dolor sit amet");
                GUILayout.Button(item.image, GUILayout.MaxWidth(item.maxWidth), GUILayout.MaxHeight(item.maxHeight));
                EditorGUILayout.EndHorizontal();                                    // E Hori 1

                EditorGUILayout.BeginHorizontal();                                  // S Hori 2
                EditorGUILayout.IntField("Exemple int", 10);
                GUILayout.Button(item.image, GUILayout.MaxWidth(item.maxWidth), GUILayout.MaxHeight(item.maxHeight));
                EditorGUILayout.EndHorizontal();                                    // E Hori 2

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;

            EditorAssetsHolder.Instance.iconsWithSize[i] = item;
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    private void AddItem()
    {

    }
}
