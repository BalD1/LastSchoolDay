using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BalDUtilities.EditorUtils;
using BalDUtilities.Misc;
using static UnityEditor.Progress;
using UnityEngine.UIElements;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;

public class WINDOW_LDTools : EditorWindow
{
    private Vector2 windowScroll = Vector2.zero;

    private Vector3 mousePosition;

    private Vector2 snapGrid = new Vector2(.25f, .25f);

    private bool isDragging;
    private bool snapKeyIsPressed;
    private bool roundedSnapKeyIsPressed;

    private bool showTutorial;

    private GameObject draggedGO;
    private string currentDraggedName;

    private SCRPT_Props.PropsByName currentProp;
    private SCRPT_Props.DecorationByName currentDeco;
    private int currentGOPFIdx;

    private int propsByRow = 5;
    private int decorationsByRow = 5;

    private static SCRPT_Props props;

    private void OnEnable()
    {
        props = (SCRPT_Props)AssetDatabase.LoadAssetAtPath("Assets/Scripts/Editor/Props.asset", typeof(SCRPT_Props));
    }

    private void OnFocus()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (isDragging)
        {
            Event e = Event.current;
            mousePosition = e.mousePosition;
            mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
            mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
            mousePosition.z = 0;

            if (draggedGO == null)
            {
                isDragging = false;
                return;
            }

            switch(e.type)
            {
                case EventType.KeyDown:

                    if (e.keyCode == KeyCode.LeftShift) snapKeyIsPressed = true;
                    if (e.keyCode == KeyCode.LeftControl) roundedSnapKeyIsPressed = true;
                    if (e.keyCode == KeyCode.LeftAlt && HasMultiples()) InstantiateNewCurrentProp(true);

                    if (e.keyCode == KeyCode.A && HasMultiples())
                    {
                        currentGOPFIdx -= 1;

                        InstantiateNewCurrentProp();
                    }
                    if (e.keyCode == KeyCode.E && HasMultiples())
                    {
                        currentGOPFIdx += 1;

                        InstantiateNewCurrentProp();
                    }

                    // 256 is Alpha0 and 265 is Alpha9
                    if (e.keyCode > (KeyCode)256 && e.keyCode < (KeyCode)265)
                    {
                        for (int i = 256; i < 265; i++)
                        {
                            if (e.keyCode == (KeyCode)i)
                            {
                                currentGOPFIdx = i - 256;
                                InstantiateNewCurrentProp();
                                break;
                            }
                        }
                    }

                    break;

                case EventType.KeyUp:

                    if (e.keyCode == KeyCode.LeftShift) snapKeyIsPressed = false;
                    if (e.keyCode == KeyCode.LeftControl) roundedSnapKeyIsPressed = false;

                    break;

                case EventType.MouseDown:

                    if (e.button == 0)
                    {
                        draggedGO = null;
                        InstantiateNewCurrentProp(true);
                        Selection.activeGameObject = null;
                    }

                    if (e.button == 1)
                    {
                        GameObject ghost = draggedGO;
                        draggedGO = null;
                        DestroyImmediate(ghost);

                        isDragging = false;
                        return;
                    }

                    break;
            }

            if (draggedGO == null)
            {
                isDragging = false;
                return;
            }

            if (snapKeyIsPressed)
            {
                Vector2 dgoPos = draggedGO.transform.position;
                Vector2 targetPos = dgoPos;

                if (mousePosition.x > (dgoPos.x + snapGrid.x)) targetPos.x += snapGrid.x;
                if (mousePosition.x < (dgoPos.x - snapGrid.x)) targetPos.x -= snapGrid.x;

                if (mousePosition.y > (dgoPos.y + snapGrid.y)) targetPos.y += snapGrid.y;
                if (mousePosition.y < (dgoPos.y - snapGrid.y)) targetPos.y -= snapGrid.y;

                draggedGO.transform.position = targetPos;
            }
            else if (roundedSnapKeyIsPressed)
            {
                Vector2 dgoPos = draggedGO.transform.position;
                Vector2 targetPos = dgoPos;

                RoundToGrid(ref targetPos.x);
                RoundToGrid(ref targetPos.y);

                if (mousePosition.x > (dgoPos.x + snapGrid.x)) targetPos.x += snapGrid.x;
                if (mousePosition.x < (dgoPos.x - snapGrid.x)) targetPos.x -= snapGrid.x;

                if (mousePosition.y > (dgoPos.y + snapGrid.y)) targetPos.y += snapGrid.y;
                if (mousePosition.y < (dgoPos.y - snapGrid.y)) targetPos.y -= snapGrid.y;

                draggedGO.transform.position = targetPos;
            }
            else 
            {
                draggedGO.transform.position = mousePosition;
            }
        }
    }

    public bool HasMultiples()
    {
        if (currentProp.pName != SCRPT_Props.E_PropName.Default) return currentProp.HasMultiplePFs();
        if (currentDeco.pName != SCRPT_Props.E_PropName.Default) return currentDeco.HasMultipleSprites();

        return false;
    }

    private float RoundToGrid(float f)
    {
        if (f % snapGrid.x != 0)
            f = f + ((snapGrid.x - f) + Mathf.Round(f));

        return f;
    }
    private void RoundToGrid(ref float f)
    {
        if (f % snapGrid.x != 0)
            f = f + ((snapGrid.x - f) + Mathf.Round(f));
    }

    [MenuItem("Window/LDTools")]
    public static void ShowWindow()
    {
        GetWindow<WINDOW_LDTools>("LD Tools Window");
    }

    private void OnGUI()
    {
        ReadOnlyDraws.EditorScriptDraw(typeof(WINDOW_LDTools), this);
        windowScroll = EditorGUILayout.BeginScrollView(windowScroll);

        showTutorial = EditorGUILayout.Foldout(showTutorial, "Tutorial");
        if (showTutorial)
        {
            EditorGUI.indentLevel++;
            GUILayout.TextArea("\nCette fenêtre vous permettra de placer facilement des props sans avoir à vous inquiéter " +
                               "de poser leur hitbox, scripts, anims, ect... Il suffit de choisir le props et de le déplacer " +
                               "dans la scène.\n" +
                               "\n" +
                               "Contrôles : \n" +
                               "Clic Gauche : Place le props sélectionné \n" +
                               "Clic Droit : Stop la sélection d'un props \n" +
                               "Shift : Permet de déplacer le props d'un montant fixe \n" +
                               "Contrôle : Permet de déplacer le props d'un montant fixe et arrondis\n" +
                               "Alt Gauche : Choisit un nouveau props random \n" +
                               "Flèches gauche & droite : Choisit le prochain/précedent props \n");
            EditorGUI.indentLevel--;
        }

        snapGrid = EditorGUILayout.Vector2Field("Snap Amount", snapGrid);
        SimpleDraws.HorizontalLine();

        DrawPropsWindow();

        SimpleDraws.HorizontalLine();

        DrawDecorationWindow();

        SimpleDraws.HorizontalLine();

        EditorGUILayout.EndScrollView();

        EditorUtility.UnloadUnusedAssetsImmediate();
        System.GC.Collect();
    }

    private void DrawPropsWindow()
    {
        propsByRow = EditorGUILayout.IntSlider("Props by rows", propsByRow, 2, 10);

        for (int i = 1; i < props.PropsByNames.Length + 1; i++)
        {
            if (i == 1) EditorGUILayout.BeginHorizontal();

            EditorAssetsHolder.IconWithSize ic = EditorAssetsHolder.Instance.GetIconData(props.PropsByNames[i - 1].icon);

            if (GUILayout.Button(ic.image, GUILayout.Width(ic.maxWidth), GUILayout.Height(ic.maxHeight)))
            {
                isDragging = true;

                currentDeco = default;
                currentProp = props.PropsByNames[i - 1];

                InstantiateNewCurrentProp(true);

                if (SceneView.sceneViews.Count > 0)
                {
                    SceneView sceneView = (SceneView)SceneView.sceneViews[0];
                    sceneView.Focus();
                }
            }

            if (i == 1) continue;

            if (i % propsByRow == 0)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
        }

        if (props.PropsByNames.Length > 0)
            EditorGUILayout.EndHorizontal();
    }

    private void DrawDecorationWindow()
    {
        decorationsByRow = EditorGUILayout.IntSlider("Decorations by rows", decorationsByRow, 2, 10);

        for (int i = 1; i < props.DecorationsByNames.Length + 1; i++)
        {
            if (i == 1) EditorGUILayout.BeginHorizontal();

            EditorAssetsHolder.IconWithSize ic = EditorAssetsHolder.Instance.GetIconData(props.DecorationsByNames[i - 1].icon);

            if (GUILayout.Button(ic.image, GUILayout.Width(ic.maxWidth), GUILayout.Height(ic.maxHeight)))
            {
                isDragging = true;

                currentProp = default;
                currentDeco = props.DecorationsByNames[i - 1];

                InstantiateNewCurrentProp(true);

                if (SceneView.sceneViews.Count > 0)
                {
                    SceneView sceneView = (SceneView)SceneView.sceneViews[0];
                    sceneView.Focus();
                }
            }

            if (i == 1) continue;

            if (i % decorationsByRow == 0)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
        }

        if (props.DecorationsByNames.Length > 0)
            EditorGUILayout.EndHorizontal();
    }

    private void InstantiateNewCurrentProp(bool random = false)
    {
        if (draggedGO != null)
        {
            GameObject ghost = draggedGO;
            draggedGO = null;
            DestroyImmediate(ghost);
        }

        if (currentProp.pName != SCRPT_Props.E_PropName.Default)
        {
            if (random && HasMultiples()) SelectRandom(currentProp.propPFs.Length);

            if (currentGOPFIdx < 0) currentGOPFIdx = currentProp.propPFs.Length - 1;
            else if (currentGOPFIdx >= currentProp.propPFs.Length) currentGOPFIdx = 0;

            draggedGO = (GameObject)PrefabUtility.InstantiatePrefab(currentProp.propPFs[currentGOPFIdx]);
            draggedGO.transform.position = mousePosition;

            Undo.RegisterCreatedObjectUndo(draggedGO, "Create my GameObject");

            currentDraggedName = currentProp.pName.ToString();
            draggedGO.name = currentDraggedName;
        }
        else
        {
            if (random && HasMultiples()) SelectRandom(currentDeco.sprites.Length);

            if (currentGOPFIdx < 0) currentGOPFIdx = currentDeco.sprites.Length - 1;
            else if (currentGOPFIdx >= currentDeco.sprites.Length) currentGOPFIdx = 0;

            draggedGO = new GameObject();
            SpriteRenderer sp = draggedGO.AddComponent<SpriteRenderer>();
            sp.sprite = currentDeco.sprites[currentGOPFIdx];
            sp.sortingLayerName = "Front-Background";
            sp.sortingOrder = 1;
            sp.spriteSortPoint = SpriteSortPoint.Pivot;
            draggedGO.transform.position = mousePosition;

            Undo.RegisterCreatedObjectUndo(draggedGO, "Create my GameObject");

            currentDraggedName = currentDeco.pName.ToString();
            draggedGO.name = currentDraggedName;
        }
    }

    private void SelectRandom(int length)
    {
        int r = Random.Range(0, length);
        if (r == currentGOPFIdx) r = (r + 1) % length;
        currentGOPFIdx = r;
    }
}
