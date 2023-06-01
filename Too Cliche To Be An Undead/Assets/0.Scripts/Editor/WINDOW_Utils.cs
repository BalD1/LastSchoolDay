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

    private bool showReplace;

    private SerializedObject serializedSelf;

    private GameManager.E_ScenesNames sceneSelect;

    [System.Serializable]
    private struct S_ReplacePrefabs
    {
        [field: SerializeField] public GameObject ObjectToReplace { get; private set; }
        [field: SerializeField] public GameObject ReplaceObjectPrefab { get; private set; }
    }

    [SerializeField] private S_ReplacePrefabs[] replacePrefabs;

    [MenuItem("Window/Utils")]
    public static void ShowWindow()
    {
        GetWindow<WINDOW_Utils>("Utils Window");
    }

    private void OnEnable()
    {
        ScriptableObject target = this;
        serializedSelf = new SerializedObject(target);
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

        SimpleDraws.HorizontalLine();

        showReplace = EditorGUILayout.Foldout(showReplace, "Replace");
        if (showReplace) Replace();

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

    private void Replace()
    {
        serializedSelf.Update();
        SerializedProperty stringsProperty = serializedSelf.FindProperty("replacePrefabs");

        EditorGUILayout.PropertyField(stringsProperty, true);
        serializedSelf.ApplyModifiedProperties();

        if (GUILayout.Button("Replace"))
        {
            int passes = 0;
            int totalInstances = 0;
            Debug.Log("<color=green><b>" + this + "</b></color> - REPLACE");
            float allStartTime = Time.time;
            Debug.Log("<color=green><b>" + this + "</b></color> " + "Start Replacing at " + allStartTime);
            foreach (var item in replacePrefabs)
            {
                passes++;
                float startTime = Time.time;
                Debug.Log("<color=green><b>" + this + "</b></color> " + "<color=red><b>" + passes + "</b></color> : -----------------------------");
                Debug.Log("<color=green><b>" + this + "</b></color> " + "<color=red><b>" + passes + "</b></color> : Start Replacing of <color=blue>" + item.ObjectToReplace.name + "</color> by <color=blue>" + item.ReplaceObjectPrefab.name + "</color>");
                GameObject[] instances = PrefabUtility.FindAllInstancesOfPrefab(item.ObjectToReplace);
                totalInstances += instances.Length;
                Debug.Log("<color=green><b>" + this + "</b></color> " + "<color=red><b>" + passes + "</b></color> : Found <color=blue>" + instances.Length + "</color> instances of <color=blue>" + item.ObjectToReplace.name+ "</color>");
                foreach (GameObject obj in instances)
                {
                    GameObject newObj = PrefabUtility.InstantiatePrefab(item.ReplaceObjectPrefab) as GameObject;
                    newObj.name = obj.name;
                    newObj.transform.parent = obj.transform.parent;
                    newObj.transform.localPosition = obj.transform.localPosition;
                    newObj.transform.localRotation = obj.transform.localRotation;
                    newObj.transform.localScale = obj.transform.localScale;
                    Undo.RegisterCreatedObjectUndo(newObj, "Created Replace Obj");
                    newObj.transform.parent = obj.gameObject.transform.parent;
                    SpriteRenderer sr = newObj.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        SpriteRenderer objSR = obj.GetComponent<SpriteRenderer>();
                        if (objSR != null)
                        {
                            sr.color = objSR.color;
                            sr.sortingLayerID = objSR.sortingLayerID;
                            sr.flipX = objSR.flipX;
                            sr.flipY = objSR.flipY;
                        }
                    }
                    Undo.DestroyObjectImmediate(obj.gameObject);
                }
                float endTime = Time.time;
                Debug.Log("<color=green><b>" + this + "</b></color> " + "<color=red><b>" + passes + "</b></color> : Ended Replacement, took : " + (endTime - startTime));
            }
            float allEndTime = Time.time;
            Debug.Log("<color=green><b>" + this + "</b></color> " + "-----------------------------");
            Debug.Log("<color=green><b>" + this + "</b></color> " + "Total replaces : <color=blue>" + totalInstances + "</color>");
            Debug.Log("<color=green><b>" + this + "</b></color> " + "Ended Replacements at " + allEndTime);
            Debug.Log("<color=green><b>" + this + "</b></color> " + "Took : " + (allEndTime - allStartTime));
        }
    }

    public string Colorize(string text, string color, bool bold)
    {
        return
        "<color=" + color + ">" +
        (bold ? "<b>" : "") +
        text +
        (bold ? "</b>" : "") +
        "</color>";
    }
}
