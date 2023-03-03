using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapReplacer : MonoBehaviour
{
    [SerializeField] private Tilemap targetMap;

    [SerializeField] private Tile[] tiles;

    [InspectorButton(nameof(ReplaceEveryTiles), ButtonWidth = 150)]
    [SerializeField] private bool replaceEveryTiles;

    private void ReplaceEveryTiles()
    {
        for (int x = targetMap.cellBounds.xMin; x < targetMap.cellBounds.xMax; x++)
        {
            for (int y = targetMap.cellBounds.yMin; y < targetMap.cellBounds.yMax; y++)
            {
                Vector3Int localPlace = (new Vector3Int(x, y, (int)targetMap.transform.position.y));

                if (targetMap.HasTile(localPlace))
                {
                    targetMap.SetTile(new Vector3Int(x, y), tiles.RandomElement());
                }
            }
        }
    }
}
