using BalDUtilities.EditorUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralRoomsManager))]
public class ED_ProceduralRoomsManager : Editor
{
    private ProceduralRoomsManager targetScript;

    private void OnEnable()
    {
        targetScript = (ProceduralRoomsManager)target;
    }

    public override void OnInspectorGUI()
    {
        ReadOnlyDraws.EditorScriptDraw(typeof(ED_GameManager), this);
        DrawDefaultInspector();

        if (GUILayout.Button("Setup Anchors data"))
        {
            targetScript.RoomsData = new ProceduralRoomsManager.RoomData[targetScript.AnchorsHolder.childCount];

            for (int i = 0; i < targetScript.AnchorsHolder.childCount; i++)
            {
                Transform child = targetScript.AnchorsHolder.GetChild(i);
                ProceduralRoomsManager.E_RoomOrientation or = AnchorNameToOrientationEnum(child.name);

                targetScript.RoomsData[i] = new ProceduralRoomsManager.RoomData(or, child);
            }

        }

        EditorGUILayout.Space();
        SimpleDraws.HorizontalLine();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Spawn Rooms"))
        {
            targetScript.PopulateLists();
            targetScript.CleanAndSpawn();
        }
        if (GUILayout.Button("Clean Rooms")) targetScript.CleanRooms();
        EditorGUILayout.EndHorizontal();
    }

    private ProceduralRoomsManager.E_RoomOrientation AnchorNameToOrientationEnum(string name)
    {
        switch (name)
        {
            case var s when name.Contains("Left"):
                return ProceduralRoomsManager.E_RoomOrientation.Left;

            case var s when name.Contains("Right"):
                return ProceduralRoomsManager.E_RoomOrientation.Right;
        }

        return ProceduralRoomsManager.E_RoomOrientation.None;
    }
}
