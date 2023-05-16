using System.Collections;
using System.Collections.Generic;
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

    private void Reset()
    {
        map = this.GetComponent<Tilemap>();
    }

    private void Count()
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

        Debug.Log(amount);
    }

    private void Keep()
    {
        int amount = 0;
        BoundsInt bounds = map.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (RandomExtensions.OneOutOfTwo())
            {
                map.SetTile(pos, okTile);
                amount++;
            }
            else map.SetTile(pos, noTile);
        }

        Debug.Log(amount);
    }
}
