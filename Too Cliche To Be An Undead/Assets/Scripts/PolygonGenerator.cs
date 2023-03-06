using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PolygonGenerator : MonoBehaviour
{
    #region setup
    //mesh properties
    Mesh mesh;
    public Vector3[] polygonPoints;
    public int[] polygonTriangles;

    [SerializeField] private GameObject cc;

    //polygon properties
    public bool isFilled;
    public int polygonSides;
    public float polygonRadius;
    public float centerRadius;

    public bool spawnObjs;

    public int objectsNum = 5;
    public float radius = 2;
    public float angleOffset;

    private List<GameObject> lol = new List<GameObject>();

    void Start()
    {
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        if (isFilled)
        {
            DrawFilled(polygonSides, polygonRadius);
        }
        else
        {
            DrawHollow(polygonSides, polygonRadius, centerRadius);
        }

        if (spawnObjs)
        {
            foreach (var item in lol)
            {
                DestroyImmediate(item);
            }
            lol.Clear();
            spawnObjs = false;
            for (var i = 1; i < objectsNum; i++)
            {
                var angle = i * Mathf.PI * radius * .5f / objectsNum;
                var AngleOffset = Mathf.Atan2(angle, angle) * Mathf.Deg2Rad;
                angle += AngleOffset;
                var pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle),0) * radius;

                GameObject gO = Instantiate(cc, pos, Quaternion.identity);

                lol.Add(gO);
            }
        }
    }
    #endregion

    private void DrawFilled(int sides, float radius)
    {
        polygonPoints = GetCircumferencePoints(sides, radius).ToArray();
        polygonTriangles = DrawFilledTriangles(polygonPoints);
        mesh.Clear();
        mesh.vertices = polygonPoints;
        mesh.triangles = polygonTriangles;
    }

    private void DrawHollow(int sides, float outerRadius, float innerRadius)
    {
        List<Vector3> pointsList = new List<Vector3>();
        List<Vector3> outerPoints = GetCircumferencePoints(sides, outerRadius);
        pointsList.AddRange(outerPoints);
        List<Vector3> innerPoints = GetCircumferencePoints(sides, innerRadius);
        pointsList.AddRange(innerPoints);

        polygonPoints = pointsList.ToArray();

        polygonTriangles = DrawHollowTriangles(polygonPoints);
        mesh.Clear();
        mesh.vertices = polygonPoints;
        mesh.triangles = polygonTriangles;
    }

    private int[] DrawHollowTriangles(Vector3[] points)
    {
        int sides = points.Length / 2;
        int triangleAmount = sides * 2;

        List<int> newTriangles = new List<int>();
        for (int i = 0; i < sides; i++)
        {
            int outerIndex = i;
            int innerIndex = i + sides;

            //first triangle starting at outer edge i
            newTriangles.Add(outerIndex);
            newTriangles.Add(innerIndex);
            newTriangles.Add((i + 1) % sides);

            //second triangle starting at outer edge i
            newTriangles.Add(outerIndex);
            newTriangles.Add(sides + ((sides + i - 1) % sides));
            newTriangles.Add(outerIndex + sides);
        }
        return newTriangles.ToArray();
    }

    private List<Vector3> GetCircumferencePoints(int sides, float radius)
    {
        List<Vector3> points = new List<Vector3>();
        float circumferenceProgressPerStep = (float)1 / sides;
        float TAU = 2 * Mathf.PI;
        float radianProgressPerStep = circumferenceProgressPerStep * TAU;

        for (int i = 0; i < sides; i++)
        {
            float currentRadian = radianProgressPerStep * i;
            points.Add(new Vector3(Mathf.Cos(currentRadian) * radius, Mathf.Sin(currentRadian) * radius, 0));
        }
        return points;
    }

    private int[] DrawFilledTriangles(Vector3[] points)
    {
        int triangleAmount = points.Length - 2;
        List<int> newTriangles = new List<int>();
        for (int i = 0; i < triangleAmount; i++)
        {
            newTriangles.Add(0);
            newTriangles.Add(i + 2);
            newTriangles.Add(i + 1);
        }
        return newTriangles.ToArray();
    }
}