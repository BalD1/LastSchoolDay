using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;
using System;
using System.Reflection;

public class WINDOW_EntitiesList : EditorWindow
{
    /*
    private Vector2 windowScroll = Vector2.zero;
    private Vector2 enemyListScroll = Vector2.zero;
    private Vector2 playerListScroll = Vector2.zero;
    private int enemyListViewSize = 0;
    private int playerListViewSize = 0;

    private List<Entity> enemiesList;
    private List<bool> showEnemy;
    private List<bool> showEnemyStats;

    private List<Entity> playersList;
    private List<bool> showPlayer;
    private List<bool> showPlayerStats;

    private bool showEnemiesList;
    private bool showPlayersList;

    private GameObject spawnTarget;

    private GameAssets.ENUM_EnemyType enemyToSpawn;

    [MenuItem("Window/Entities")]
    public static void ShowWindow()
    {
        GetWindow<WINDOW_EntitiesList>("Entities Displayer");
    }

    private void OnGUI()
    {
        windowScroll = EditorGUILayout.BeginScrollView(windowScroll);
        DisplayUpperButtons();

        SimpleDraws.HorizontalLine(Color.white);

        DisplayEnemiesList();

        SimpleDraws.HorizontalLine();

        DisplayPlayersList();

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Displays the buttons to initialize the lists
    /// </summary>
    private void DisplayUpperButtons()
    {
        // List Initialization
        EditorGUILayout.Space(50);
        if (GUILayout.Button("InitializeLists") || enemiesList == null)
            InitializeLists();
    }

    /// <summary>
    /// Draws the list of every enemies in the scene
    /// </summary>
    private void DisplayEnemiesList()
    {
        // Should we display the enemies list ?
        enemyListViewSize = EditorGUILayout.IntField("View Size", enemyListViewSize);
        enemyListViewSize = Mathf.Clamp(enemyListViewSize, 0, int.MaxValue);
        MixedDraws.ListFoldoutWithSize<Entity>(ref showEnemiesList, "Enemies List", enemiesList);

        if (showEnemiesList && enemiesList.Count > 0)
        {
            EditorGUILayout.BeginVertical("GroupBox");                                    // Begin Vert 1
            EditorGUI.indentLevel++;

            enemyListScroll = EditorGUILayout.BeginScrollView(enemyListScroll, GUILayout.Height(enemyListViewSize));
            for (int i = 0; i < enemiesList.Count; i++)
            {
                DisplaySingleEntity(i, true);
            }
            EditorGUILayout.EndScrollView();

            EditorGUI.indentLevel--;

            EditorGUILayout.Space(25);
            SimpleDraws.HorizontalLine();

            DisplaySpawnEnemyLayout();
            EditorGUILayout.EndVertical();                                               // End Vert 1 
        }

    }

    /// <summary>
    /// Draws the list of every players in the scene
    /// </summary>
    private void DisplayPlayersList()
    {
        // Should we display the enemies list ?
        playerListViewSize = EditorGUILayout.IntField("View Size", playerListViewSize);
        playerListViewSize = Mathf.Clamp(playerListViewSize, 0, int.MaxValue);
        MixedDraws.ListFoldoutWithSize<Entity>(ref showPlayersList, "Players List", enemiesList);

        if (showPlayersList && playersList.Count > 0)
        {
            EditorGUILayout.BeginVertical("GroupBox");                                    // Begin Vert 1
            EditorGUI.indentLevel++;

            playerListScroll = EditorGUILayout.BeginScrollView(playerListScroll, GUILayout.Height(playerListViewSize));
            for (int i = 0; i < playersList.Count; i++)
            {
                DisplaySingleEntity(i, false);
            }
            EditorGUILayout.EndScrollView();

            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();                                               // End Vert 1 
        }

    }

    /// <summary>
    /// Draws a given enemy of the list[idx], and it's different components
    /// </summary>
    /// <param name="idx"></param>
    private void DisplaySingleEntity(int idx, bool isEnemy)
    {
        List<Entity> list = new List<Entity>();
        List<bool> showList = new List<bool>();
        List<bool> showStatsList = new List<bool>();

        if (isEnemy)
        {
            list = enemiesList;
            showList = showEnemy;
            showStatsList = showEnemyStats;
        }
        else
        {
            list = playersList;
            showList = showPlayer;
            showStatsList = showPlayerStats;
        }

        Entity current;
        GameObject currentGO;

        if (idx > list.Count)
            InitializeLists();

        if (list[idx] == null)
            return;

        current = list[idx];
        currentGO = current.gameObject;

        EditorGUILayout.BeginHorizontal();                                              // Begin Hor 1

        // Should we display the enemy[i] ?
        showList[idx] = EditorGUILayout.Foldout(showList[idx], current.gameObject.name);
        if (showList[idx])
        {
            DrawDuplicateAndKillButtons(currentGO);
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("Entity " + idx + " : " + current.gameObject.name + ", type of " + current.GetStats.name);
            ReadOnlyDraws.GameObjectDraw(currentGO);

            currentGO.transform.position = EditorGUILayout.Vector3Field("Position", currentGO.transform.position);

            // Shoud we display his stats ?
            showStatsList[idx] = EditorGUILayout.Foldout(showStatsList[idx], "Entity Stats");
            if (showStatsList[idx])
            {
                EditorGUI.indentLevel++;

                GUI.enabled = false;
                EditorGUILayout.TextField("HP : ", current.CurrentHP + " / " + current.GetStats.MaxHP);

                EditorGUILayout.FloatField("Damages : ", current.GetStats.Damages);
                EditorGUILayout.FloatField("Speed : ", current.GetStats.Speed);
                EditorGUILayout.FloatField("Attack Range : ", current.GetStats.AttackRange);
                EditorGUILayout.FloatField("Attack Cooldown : ", current.GetStats.Attack_COOLDOWN);
                EditorGUILayout.FloatField("Invincibility Cooldown : ", current.GetStats.Invincibility_COOLDOWN);
                EditorGUILayout.IntField("Crit Chances : ", current.GetStats.CritChances);

                Color color = current.GetStats.Team == SCRPT_EntityStats.TeamType.Enemy ? Color.red : Color.blue;
                GUIStyle style = new GUIStyle(EditorStyles.textField);
                style.normal.textColor = color;
                EditorGUILayout.TextField("Team : ", "" + current.GetStats.Team, style);

                GUI.enabled = true;

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }
        else
            DrawDuplicateAndKillButtons(currentGO);

    }

    private void DrawDuplicateAndKillButtons(GameObject currentGO)
    {
        if (GUILayout.Button("Duplicate", GUILayout.MaxWidth(70)))
        {
            Instantiate(currentGO, currentGO.transform.position, Quaternion.identity);
            InitializeLists();
        }

        if (GUILayout.Button("Kill", GUILayout.MaxWidth(30)))
        {
            DestroyImmediate(currentGO);
            InitializeLists();
        }

        EditorGUILayout.EndHorizontal();                                              // End Hor 1
    }

    /// <summary>
    /// Draws the fields utilities to spawn an enemy
    /// </summary>
    private void DisplaySpawnEnemyLayout()
    {
        // Spawn position by target
        EditorGUILayout.BeginHorizontal();
        if (spawnTarget)
        {
            if (spawnTarget.name != "Target")
            {
                spawnTarget = (GameObject)EditorGUILayout.ObjectField("Target", spawnTarget, typeof(GameObject), true);
                GUI.enabled = false;
                EditorGUILayout.Vector2Field("Position", spawnTarget.transform.position);
                GUI.enabled = true;
                if (GUILayout.Button("Create Target"))
                {
                    spawnTarget = null;
                    spawnTarget = new GameObject("Target");
                }
            }
            else
            {
                GUI.enabled = false;
                spawnTarget = (GameObject)EditorGUILayout.ObjectField("Target", spawnTarget, typeof(GameObject), true);
                GUI.enabled = true;
                spawnTarget.transform.position = EditorGUILayout.Vector2Field("", spawnTarget.transform.position);
                if (GUILayout.Button("Delete Target"))
                {
                    DestroyImmediate(spawnTarget);
                }
            }

            EditorGUILayout.EndHorizontal();
            // Spawn Enemy button

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(50);
            if (GUILayout.Button("Spawn Enemy"))
            {
                foreach (var item in GameAssets.Instance.EnemiesByType)
                {
                    if (item.EnemyType.Equals(enemyToSpawn))
                    {
                        Instantiate(item.Enemy_PF, spawnTarget.transform.position, Quaternion.identity);
                        InitializeLists();
                    }
                }
            }
            enemyToSpawn = (GameAssets.ENUM_EnemyType)EditorGUILayout.EnumPopup(enemyToSpawn);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            spawnTarget = (GameObject)EditorGUILayout.ObjectField("Target", spawnTarget, typeof(GameObject), true);
            if (GUILayout.Button("Create Target"))
            {
                spawnTarget = null;
                spawnTarget = new GameObject("Target");
                AssignLabel(spawnTarget);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Searches for every enemies in the scene, then adds them in the list
    /// </summary>
    private void InitializeLists()
    {
        InitializeSingleList(false);
        InitializeSingleList(true);
    }

    private void InitializeSingleList(bool isEnemy)
    {
        List<Entity> list = new List<Entity>();
        List<bool> showList = new List<bool>();
        List<bool> showStatsList = new List<bool>();
        string tag = "";

        if (isEnemy)
        {
            enemiesList = new List<Entity>();
            showEnemy = new List<bool>();
            showEnemyStats = new List<bool>();

            list = enemiesList;
            showList = showEnemy;
            showStatsList = showEnemyStats;
            tag = "Enemy";
        }
        else
        {
            playersList = new List<Entity>();
            showPlayer = new List<bool>();
            showPlayerStats = new List<bool>();

            list = playersList;
            showList = showPlayer;
            showStatsList = showPlayerStats;
            tag = "Player";
        }

        GameObject[] foundEntities = GameObject.FindGameObjectsWithTag(tag);
        Debug.Log("Found " + foundEntities.Length + " " + tag);

        foreach (GameObject go in foundEntities)
        {
            if (go == null)
                continue;

            list.Add(go.GetComponent<Entity>());
            showList.Add(false);
            showStatsList.Add(false);
        }
    }

    public static void AssignLabel(GameObject g)
    {
        Texture2D tex = EditorGUIUtility.IconContent("sv_label_0").image as Texture2D;
        Type editorGUIUtilityType = typeof(EditorGUIUtility);
        BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
        object[] args = new object[] { g, tex };
        editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
    }
    */
}
