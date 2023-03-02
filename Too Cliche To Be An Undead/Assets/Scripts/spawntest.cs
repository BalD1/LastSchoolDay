using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class spawntest : MonoBehaviour
{
    public Tilemap tileMap;
    public List<Vector3> availablePlaces;

    public GameObject objectPrefab;
    public int objectsToSpawn = 50;
    public GameObject[] objects;

    [InspectorButton(nameof(SpawnEnemy), ButtonWidth = 150)]
    public bool spawn;

    [InspectorButton(nameof(SetupBounds), ButtonWidth = 150)]
    public bool setupbounds;

    [InspectorButton(nameof(DestroyAllObjects), ButtonWidth = 150)]
    public bool destroyall;

    private void SetupBounds()
    {
        availablePlaces = new List<Vector3>();

        for (int n = tileMap.cellBounds.xMin; n < tileMap.cellBounds.xMax; n++)
        {
            for (int p = tileMap.cellBounds.yMin; p < tileMap.cellBounds.yMax; p++)
            {
                Vector3Int localPlace = (new Vector3Int(n, p, (int)tileMap.transform.position.y));
                Vector3 place = tileMap.CellToWorld(localPlace);
                if (tileMap.HasTile(localPlace))
                {
                    availablePlaces.Add(place);
                }
            }
        }

        for (int i = 0; i < availablePlaces.Count / 2; i++) availablePlaces.RemoveAt(Random.Range(0, availablePlaces.Count));
    }

    private void DestroyAllObjects()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            DestroyImmediate(objects[i]);
        }
        objects = new GameObject[0];
    }

    private void SpawnEnemy()
    {
        DestroyAllObjects();

        objects = new GameObject[objectsToSpawn];
        for (int i = 0; i < objectsToSpawn; i++)
        {
            objects[i] =  Instantiate(objectPrefab,
                 availablePlaces[Random.Range(0, availablePlaces.Count)],
                 Quaternion.identity);
        }
    }
}
