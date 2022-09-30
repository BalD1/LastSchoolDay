using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using BalDUtilities.EditorUtils;
using BalDUtilities.Misc;

public class WINDOW_Utils : EditorWindow
{
    private Vector2 windowScroll = Vector2.zero;
    private Vector2 scenesScroll = Vector2.zero;

    private int scenesScrollView = 0;

    private bool showScenes;

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
            EditorGUILayout.BeginHorizontal();

            sceneSelect = (GameManager.E_ScenesNames)EditorGUILayout.EnumPopup(sceneSelect);
            if (GUILayout.Button("Select"))
            {
                if (GameManager.CompareCurrentScene(sceneSelect))
                    Debug.LogError("Already in scene.");
                else
                {
                    if (Application.isPlaying)
                        SceneManager.LoadScene(EnumsExtension.EnumToString(sceneSelect));
                    else
                        EditorSceneManager.OpenScene("Assets/Scenes/" + EnumsExtension.EnumToString(sceneSelect) + ".unity");
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndScrollView();


        EditorGUILayout.EndVertical();
    }
}
