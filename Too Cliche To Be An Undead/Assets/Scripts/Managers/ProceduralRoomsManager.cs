using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralRoomsManager : MonoBehaviour
{
    private static ProceduralRoomsManager instance;
    public static ProceduralRoomsManager Instance
    {
        get => instance;
    }

    [SerializeField] private Transform anchorsHolder;
    public Transform AnchorsHolder { get => anchorsHolder; }

    [SerializeField] private RoomData[] roomsData = new RoomData[0];
    public RoomData[] RoomsData { get => roomsData; set => roomsData = value; }

    [ReadOnly] [SerializeField] private List<GameObject> instantiatedRooms = new List<GameObject>();

    [SerializeField] private GameObject[] leftClassroomsPF = new GameObject[0];
    [SerializeField] private GameObject[] rightClassroomsPF = new GameObject[0];
    //[SerializeField] private GameObject[] upClassroomsPF = new GameObject[0];
    //[SerializeField] private GameObject[] downClassroomsPF = new GameObject[0];

    private List<GameObject> availableLeftRooms = new List<GameObject>();
    private List<GameObject> availableRightRooms = new List<GameObject>();

    [System.Serializable]
    public struct RoomData
    {
        public E_RoomOrientation roomOrientation;
        public Transform roomAnchor;

        public RoomData(E_RoomOrientation _roomOrientation, Transform _roomAnchor)
        {
            roomOrientation = _roomOrientation;
            roomAnchor = _roomAnchor;
        }
    }

    public enum E_RoomOrientation
    {
        Left,
        Right,
        //Up,
        //Down,

        None,
    }

    private void Awake()
    {
        instance = this;

        PopulateLists();

        CleanAndSpawn();
    }

    public void PopulateLists()
    {
        availableLeftRooms.Clear();
        availableRightRooms.Clear();

        foreach (var item in leftClassroomsPF) availableLeftRooms.Add(item);
        foreach (var item in rightClassroomsPF) availableRightRooms.Add(item);
    }

    public void CleanAndSpawn()
    {
        CleanRooms();
        SpawnEveryRooms();
    }

    public void CleanRooms()
    {
        if (instantiatedRooms != null && instantiatedRooms.Count > 0)
        {
#if UNITY_EDITOR
            foreach (var item in instantiatedRooms) DestroyImmediate(item);
#else
            foreach (var item in instantiatedRooms) Destroy(item);
#endif
        }

        instantiatedRooms.Clear();
    }

    public void SpawnEveryRooms()
    {
        foreach (var item in roomsData) AddNewRoom(item);
    }

    private void AddNewRoom(RoomData data)
    {
        switch (data.roomOrientation)
        {
            case E_RoomOrientation.Left:

                GetRandomRoomAtList(ref availableLeftRooms, leftClassroomsPF, data.roomAnchor);

                break;

            case E_RoomOrientation.Right:

                GetRandomRoomAtList(ref availableRightRooms, rightClassroomsPF, data.roomAnchor);
                break;

            //case E_RoomOrientation.Up:
            //    break;

            //case E_RoomOrientation.Down:
            //    break;

            case E_RoomOrientation.None:
                Debug.LogError(data + " orientation was set at None.");
                break;
        }
    }

    private void GetRandomRoomAtList(ref List<GameObject> list, GameObject[] pool, Transform anchorPoint)
    {
        PopulateListIfEmpty(ref list, pool);

        int randRoom = Random.Range(0, list.Count);
        GameObject newRoom = Instantiate(list[randRoom], anchorPoint.position, Quaternion.identity);

        list.RemoveAt(randRoom);

        if (newRoom != null)
            instantiatedRooms.Add(newRoom);
    }

    private void PopulateListIfEmpty(ref List<GameObject> list, GameObject[] pool)
    {
        if (list.Count <= 0)
            foreach (var item in pool) list.Add(item);
    }

}
