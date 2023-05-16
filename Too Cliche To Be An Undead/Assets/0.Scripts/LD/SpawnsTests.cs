using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnsTests : MonoBehaviour
{
    [SerializeField] private Tilemap map;

    [InspectorButton(nameof(Count))]
    [SerializeField] private bool count;

    [SerializeField, Range(0,100)] private float tilesPercentageToKeep;

    [SerializeField] private Tile okTile, noTile;

    [InspectorButton(nameof(Keep))]
    [SerializeField] private bool keep;

    [InspectorButton(nameof(TestSize))]
    public bool testSize;

    public int arraySize = 132;

    private void Reset()
    {
        map = this.GetComponent<Tilemap>();
    }

    private int Count()
    {
        int amount = 0;

        BoundsInt bounds = map.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            Tile tile = map.GetTile<Tile>(pos);
            if (tile != null)
            {
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
        int dictSize = Count();

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
            buffer.Add(i, new V2I[arrayCapacity]);
        }
    }
}

