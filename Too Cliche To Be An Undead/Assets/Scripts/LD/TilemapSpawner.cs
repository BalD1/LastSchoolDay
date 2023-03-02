using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class TilemapSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    [ReadOnly][SerializeField] private Vector3[] spawnPoints;

    [SerializeField] private S_MinMaxVector[] boundsToExclude;

    [System.Serializable]
    private struct S_MinMaxVector
    {
        public int xMin;
        public int xMax;

        public int yMin;
        public int yMax;
    }

    [SerializeField][Range(0, 100)] private int pointsToRemovePercentage;

    [SerializeField] private GameObject[] objectsToSpawn_PF;
    [SerializeField] private int spawnCount;

    [SerializeField] private GameObject[] spawnedObjects;

    [InspectorButton(nameof(SpawnObjects), ButtonWidth = 150)]
    [SerializeField] private bool spawn;

    [InspectorButton(nameof(SetupBounds), ButtonWidth = 150)]
    [SerializeField] private bool setupbounds;

    [InspectorButton(nameof(DestroyAllObjects), ButtonWidth = 150)]
    [SerializeField] private bool destroyall;

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(tilemap.WorldToCell(mousePos));
    }

    private void SetupBounds()
    {
        List<Vector3> points = new List<Vector3>();

        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                Vector3Int localPlace = (new Vector3Int(x, y, (int)tilemap.transform.position.y));
                Vector3 place = tilemap.CellToWorld(localPlace);

                bool exclude = false;
                foreach (var bound in boundsToExclude)
                {
                    if (place.x >= bound.xMin && place.x <= bound.xMax &&
                        place.y >= bound.yMin && place.y <= bound.yMax)
                    {
                        exclude = true;
                        continue;
                    }
                }

                if (exclude) continue;

                if (tilemap.HasTile(localPlace))
                {
                    points.Add(place);
                }
            }
        }

        if (pointsToRemovePercentage > 0)
        {
            float percentageToValue = pointsToRemovePercentage * 0.01f;
            int iterations = (int)(points.Count * percentageToValue);

            RemovePointsFromList(ref points, iterations);
        }

        spawnPoints = new Vector3[points.Count];

        for (int i = 0; i < points.Count; i++)
        {
            spawnPoints[i] = points[i];
        }
    }

    private void RemovePointsFromList(ref List<Vector3> pointsList, int iterationsLeft)
    {
        if (pointsList.Count <= 0) return;

        pointsList.RemoveAt(Random.Range(0, pointsList.Count));

        if (iterationsLeft > 0)
        {
            RemovePointsFromList(ref pointsList, iterationsLeft - 1);
        }
    }

    private void DestroyAllObjects()
    {
        for (int i = 0; i < spawnedObjects.Length; i++)
        {
            DestroyImmediate(spawnedObjects[i]);
        }
        spawnedObjects = new GameObject[0];
    }

    private void SpawnObjects()
    {
        DestroyAllObjects();

        spawnedObjects = new GameObject[spawnCount];
        for (int i = 0; i < spawnCount; i++)
        {
            spawnedObjects[i] = Instantiate(objectsToSpawn_PF.RandomElement(),
                                            spawnPoints.RandomElement(),
                                            Quaternion.identity);
        }
    }
}
