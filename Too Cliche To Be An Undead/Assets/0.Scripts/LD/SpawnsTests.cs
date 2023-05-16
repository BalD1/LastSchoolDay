using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[ExecuteInEditMode]
public class SpawnsTests : MonoBehaviour
{
    [SerializeField] private Tilemap map;

    [InspectorButton(nameof(Count))]
    [SerializeField] private bool count;

    [SerializeField, Range(0,100)] private float tilesPercentageToKeep;

    [SerializeField] private Tile okTile, noTile, spawnTile;

    [InspectorButton(nameof(Keep))]
    [SerializeField] private bool keep;

    [InspectorButton(nameof(TestSize))]
    public bool testSize;

    public Vector2Int tilesRange;
    public Vector2Int innerRange;

    private Vector3Int currentPosInMap;
    private Vector3Int lastPosInMap;

    public Vector3Int[] surroundingTilesPositions;

    public Transform target;

    public int arraySize = 132;

    public bool simulate;

    private void OnValidate()
    {
        if (innerRange.x % 2 != 0) innerRange.x++;
        if (innerRange.y % 2 != 0) innerRange.y++;
    }

    private void Update()
    {
        if (!simulate) return;

        currentPosInMap = map.WorldToCell(target.position);

        if (currentPosInMap == lastPosInMap) return;

        CheckNeighbours();
    }

    private void CheckNeighbours()
    {
        if (tilesRange.x == 0 || tilesRange.y == 0) return;

        if (surroundingTilesPositions != null)
        {
            foreach (var item in surroundingTilesPositions)
            {
                map.SetTile(item, okTile);
            }
        }

        surroundingTilesPositions = new Vector3Int[(tilesRange.x * tilesRange.y) - ((innerRange.x / 2 * tilesRange.x) + (innerRange.y / 2 * tilesRange.y))];

        int arrayIdx = 0;

        for (int x = 0; x < tilesRange.x * .5f; x++)
        {
            for (int y = 0; y < tilesRange.y * .5f; y++)
            {
                if (x <= innerRange.x * .5f && y <=  innerRange.y * .5f) continue;

                Vector3Int dlNeighbour = new Vector3Int(currentPosInMap.x - x, currentPosInMap.y - y);
                Vector3Int ulNeighbour = new Vector3Int(currentPosInMap.x - x, currentPosInMap.y + y);
                Vector3Int drNeighbour = new Vector3Int(currentPosInMap.x + x, currentPosInMap.y - y);
                Vector3Int urNeighbour = new Vector3Int(currentPosInMap.x + x, currentPosInMap.y + y);

                SetupCell(dlNeighbour, ref arrayIdx);
                SetupCell(ulNeighbour, ref arrayIdx);
                SetupCell(drNeighbour, ref arrayIdx);
                SetupCell(urNeighbour, ref arrayIdx);
            }
        }

        void SetupCell(Vector3Int pos, ref int idx)
        {
            if (pos == Vector3Int.zero) return;
            if (surroundingTilesPositions.Contains(pos)) return;
            if (map.HasTile(pos) == false || map.GetTile(pos) == noTile) return;

            surroundingTilesPositions[idx] = pos;
            idx++;
            map.SetTile(pos, spawnTile);
        }
    }

    private void Reset()
    {
        map = this.GetComponent<Tilemap>();
    }

    private int Count() => Count(false);
    private int Count(bool onlyOK)
    {
        int amount = 0;

        BoundsInt bounds = map.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            Tile tile = map.GetTile<Tile>(pos);
            if (tile != null)
            {
                if (!onlyOK)
                    amount += 1;
                else if (tile == okTile)
                    amount += 1;
            }
        }

        Debug.Log("The map has " + amount + " tiles.");

        return amount;
    }

    private void Keep()
    {
        int amount = 0;
        BoundsInt bounds = map.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (!map.HasTile(pos)) continue;
            if (RandomExtensions.PercentageChance(tilesPercentageToKeep))
            {
                map.SetTile(pos, okTile);
                amount++;
            }
            else map.SetTile(pos, noTile);
        }

        Debug.Log("Kept " + amount + " tiles.");
    }

    private void TestSize()
    {
        int dictSize = Count(true);

        long lSize = 0;
        SizeTestClass sizeTestObj = new SizeTestClass(dictSize, arraySize);
        sizeTestObj.Populate();

        try
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();

            BinaryFormatter objFormatter = new BinaryFormatter();
            objFormatter.Serialize(stream, sizeTestObj);

            lSize = stream.Length;

            Debug.Log("Size of the Object: " + lSize + " bytes");
        }
        catch (Exception excp)
        {
            Debug.Log("Error: " + excp.Message);
        }
    }
}

[Serializable]
public class SizeTestClass
{
    private int dictionaryCapacity = 1000;
    private int arrayCapacity = 100;

    [Serializable]
    public class V2I
    {
        public int x;
        public int y;

        public V2I(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    Dictionary<int, V2I[]> buffer = new Dictionary<int, V2I[]>();

    public SizeTestClass(int _dictionaryCapacity, int _arrayCapacity)
    {
        this.dictionaryCapacity = _dictionaryCapacity;
        this.arrayCapacity = _arrayCapacity;
    }

    public void Populate()
    {
        buffer = new Dictionary<int, V2I[]>(dictionaryCapacity);

        for (int i = 0; i < dictionaryCapacity; i++)
        {
            V2I[] v = new V2I[arrayCapacity];
            for (int j = 0; j < arrayCapacity; j++)
            {
                v[j] = new V2I(0,0);
            }

            buffer.Add(i, v);
        }
    }
}

