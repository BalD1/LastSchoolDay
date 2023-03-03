using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class SurroundingTiles : MonoBehaviour
{
    [SerializeField] private Tilemap targetMap;

    [SerializeField] private Vector2Int tilesRange;

    [SerializeField] private TileBase surroundingTile;
    [SerializeField] private TileBase baseTile;

    [SerializeField][ReadOnly] private Vector3Int[] surroundingTilesPositions;

    [SerializeField] [ReadOnly] private Vector2Int currentPositionInMap;
    [SerializeField] [ReadOnly] private Vector2Int lastPositionInMap;

    [SerializeField] private bool updateSurroundingTiles;

    private void Start()
    {
    }

    private void Update()
    {
        if (updateSurroundingTiles == false) return;
        currentPositionInMap = (Vector2Int)targetMap.WorldToCell(this.transform.position);

        if (currentPositionInMap != lastPositionInMap) CheckSurroundingTiles();
    }

    private void CheckSurroundingTiles()
    {
        foreach (var item in surroundingTilesPositions)
        {
            targetMap.SetTile(item, baseTile);
        }

        int arrayMaxSize = tilesRange.x * tilesRange.y + 1;
        surroundingTilesPositions = new Vector3Int[arrayMaxSize];

        int arrayIdx = 0;

        Vector2Int minPos = new Vector2Int(
            currentPositionInMap.x - Mathf.CeilToInt(tilesRange.x * .5f) + 1,
            currentPositionInMap.y - Mathf.CeilToInt(tilesRange.y * .5f) + 1
            );

        Vector2Int maxPos = new Vector2Int(
            currentPositionInMap.x + Mathf.FloorToInt(tilesRange.x * .5f) + 1,
            currentPositionInMap.y + Mathf.FloorToInt(tilesRange.y * .5f) + 1
            );

        for (int x = minPos.x; x < maxPos.x; x++)
        {
            for (int y = minPos.y; y < maxPos.y; y++)
            {
                arrayIdx++;

                int xCellPos = x;
                int yCellPos = y;

                Vector3Int cellpos = new Vector3Int(xCellPos, yCellPos);

                if (targetMap.HasTile(cellpos) == false) continue;

                surroundingTilesPositions[arrayIdx] = cellpos;
                targetMap.SetTile(cellpos, surroundingTile);
            }
        }

        lastPositionInMap = currentPositionInMap;
    }
}
