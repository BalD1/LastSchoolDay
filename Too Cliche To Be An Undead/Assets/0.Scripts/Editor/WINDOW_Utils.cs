using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using BalDUtilities.EditorUtils;
using BalDUtilities.Misc;
using System;

public class WINDOW_Utils : EditorWindow
{
    private Vector2 windowScroll = Vector2.zero;
    private Vector2 scenesScroll = Vector2.zero;

    private int scenesScrollView = 0;

    private bool showScenes;

    private bool showUIUtils;

    private GameManager.E_ScenesNames sceneSelect;

    [MenuItem("Window/Utils")]
    public static void ShowWindow()
    {
        GetWindow<WINDOW_Utils>("Utils Window");
    }

    private void OnGUI()
    {
        ReadOnlyDraws.EditorScriptDraw(typeof(WINDOW_Utils), this);
        windowScroll = EditorGUILayout.BeginScrollView(windowScroll);

        if (GUILayout.Button("Force scripts recompile")) AssetDatabase.Refresh();

        ScenesManagement();

        SimpleDraws.HorizontalLine();

        showUIUtils = EditorGUILayout.Foldout(showUIUtils, "UI Utils");
        if (showUIUtils) UIUtils();

        EditorGUILayout.EndScrollView();
    }

    private void ScenesManagement()
    {
        EditorGUILayout.BeginVertical("GroupBox");

        scenesScrollView = EditorGUILayout.IntField("View Size", scenesScrollView);
        scenesScrollView = Mathf.Clamp(scenesScrollView, 0, int.MaxValue);

        scenesScroll = EditorGUILayout.BeginScrollView(scenesScroll, GUILayout.Height(scenesScrollView));

        showScenes = EditorGUILayout.Foldout(showScenes, "Scenes management");
        if (showScenes)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical("GroupBox");

            foreach (var item in Enum.GetNames(typeof(GameManager.E_ScenesNames)))
            {
                if (item == SceneManager.GetActiveScene().name) continue;

                if (GUILayout.Button("Go to " + item))
                {
                    if (Application.isPlaying)
                        SceneManager.LoadScene(EnumsExtension.EnumToString(sceneSelect));
                    else
                        EditorSceneManager.OpenScene("Assets/2.Scenes/" + item + ".unity");
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndScrollView();


        EditorGUILayout.EndVertical();
    }

    private void UIUtils()
    {
        EditorGUILayout.BeginVertical("GroupBox");

        if (GUILayout.Button("Setup every UI elements anchors"))
        {
            RectTransform[] elements = GameObject.FindObjectsOfType<RectTransform>();

            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].parent == null) continue;
                if (elements[i].rotation.eulerAngles != Vector3.zero) return;

                SetupAnchors(ref elements[i]);
            }

            void SetupAnchors(ref RectTransform itemTransform)
            {
                RectTransform parentTransform = itemTransform.parent as RectTransform;

                if (parentTransform == null) return;

                Vector2 newAnchorsMin = new Vector2(itemTransform.anchorMin.x + itemTransform.offsetMin.x / parentTransform.rect.width,
                                                    itemTransform.anchorMin.y + itemTransform.offsetMin.y / parentTransform.rect.height);
                Vector2 newAnchorsMax = new Vector2(itemTransform.anchorMax.x + itemTransform.offsetMax.x / parentTransform.rect.width,
                                                    itemTransform.anchorMax.y + itemTransform.offsetMax.y / parentTransform.rect.height);

                itemTransform.anchorMin = newAnchorsMin;
                itemTransform.anchorMax = newAnchorsMax;
                itemTransform.offsetMin = itemTransform.offsetMax = new Vector2(0, 0);
            }
        }

        EditorGUILayout.EndVertical();
    }
}
