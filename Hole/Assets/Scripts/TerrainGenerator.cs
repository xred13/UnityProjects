using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Generate Mesh")]

    public GameObject terrain;

    [Header("Generate Width Points")]

    public float width;

    public float minTightWidth;
    public float maxTightWidth;
    public int tightWidthPointsHalfLength;
    public int tightWidthPointsMaintain;
    public float tightWidthChance;


    [Header("Generate Center Points")]
    public float startZPos = 0;
    public float endZPos;
    public float distanceEachZPos;

    public float perlinHeight;
    public float perlinLength;
    public float perlinScale = 1;


    float randomPerlinNoiseStart;

    [HideInInspector]
    public Vector3[] centerPoints = new Vector3[0];
    Vector3[,] widthPoints;

    Vector3[] vertices;
    int[] triangles;

    public static TerrainGenerator instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            Destroy(gameObject);
        }

        randomPerlinNoiseStart = Random.Range(0, 500);

        GenerateCenterPoints();
        GenerateWidthPoints();

        GenerateMesh();
    }



    void GenerateMesh()
    {
        GenerateVertices();
        GenerateTriangles();

        Mesh mesh = new Mesh()
        {
            vertices = vertices,
            triangles = triangles
        };

        terrain.AddComponent<MeshFilter>();
        terrain.GetComponent<MeshFilter>().mesh = mesh;

        terrain.AddComponent<MeshCollider>();

    }

    void GenerateTriangles()
    {
        triangles = new int[(centerPoints.Length - 1) * 2 * 3];


        int currentVertice = 0;
        for(int i = 0; i < triangles.Length; i += 6)
        {
            triangles[i] = currentVertice;
            triangles[i + 1] = currentVertice + 2;
            triangles[i + 2] = currentVertice + 1;

            triangles[i + 3] = currentVertice + 1;
            triangles[i + 4] = currentVertice + 2;
            triangles[i + 5] = currentVertice + 3;

            currentVertice += 2;
        }
    }

    void GenerateVertices()
    {
        vertices = new Vector3[widthPoints.GetLength(0) *2];

        for(int i = 0; i < vertices.Length; i += 2)
        {
            vertices[i] = widthPoints[i/2, 0];
            vertices[i + 1] = widthPoints[i/2, 1];
        }
    }

    void GenerateWidthPoints() // for each index of the centerpoints array, we have an array with 2 values, one for the left side width x position, the other for the right side
    {
        widthPoints = new Vector3[centerPoints.Length,2];

        float halfWidth = width / 2;

        // setting the first point's width values
        Vector3 direction;
        Vector3 perpendicular;

        widthPoints[0, 0] = centerPoints[0] - halfWidth * Vector3.right;
        widthPoints[0, 1] = centerPoints[0] + halfWidth * Vector3.right;


        // setting the rest except the last
        for (int i = 1; i < widthPoints.GetLength(0) - 1; i++)
        {
            if(centerPoints[i].z + (tightWidthPointsHalfLength*2 + tightWidthPointsMaintain)*distanceEachZPos <= endZPos && Random.Range(0, centerPoints.Length) < tightWidthChance) // if there's space left for tightening the terrain and there's chance
            {
                float smallestWidth = Random.Range(minTightWidth, maxTightWidth);
                float difference = (width - smallestWidth) / 2 / tightWidthPointsHalfLength;

                float currentHalfWidth = halfWidth;

                int cont = 0;

                for (int k = 0; k < tightWidthPointsHalfLength; k++) // decrease width
                {
                    direction = centerPoints[i + k + 1] - centerPoints[i + k - 1];
                    perpendicular = new Vector3(direction.z, direction.y, -direction.x).normalized;

                    float distance = currentHalfWidth - difference;

                    widthPoints[i + k, 0] = centerPoints[i + k] - distance * perpendicular;
                    widthPoints[i + k, 1] = centerPoints[i + k] + distance * perpendicular;

                    currentHalfWidth -= difference;

                    cont++;
                }

                int reference = cont;
                for (int k = reference; k < reference + tightWidthPointsMaintain; k++) // maintain smallest width
                {
                    direction = centerPoints[i + k + 1] - centerPoints[i + k - 1];
                    perpendicular = new Vector3(direction.z, direction.y, -direction.x).normalized;

                    widthPoints[i + k, 0] = centerPoints[i + k] - currentHalfWidth * perpendicular;
                    widthPoints[i + k, 1] = centerPoints[i + k] + currentHalfWidth * perpendicular;

                    cont++;
                }

                reference = cont;
                for (int k = reference; k < reference + tightWidthPointsHalfLength; k++) // increase width to normal
                {
                    direction = centerPoints[i + k + 1] - centerPoints[i + k - 1];
                    perpendicular = new Vector3(direction.z, direction.y, -direction.x).normalized;

                    float distance = currentHalfWidth - difference;

                    widthPoints[i + k, 0] = centerPoints[i + k] - distance * perpendicular;
                    widthPoints[i + k, 1] = centerPoints[i + k] + distance * perpendicular;

                    currentHalfWidth += difference;

                    cont++;
                }


                i += cont - 1;
            }
            else // we go with normal terrain
            {
                direction = centerPoints[i + 1] - centerPoints[i - 1];
                perpendicular = new Vector3(direction.z, direction.y, -direction.x).normalized;

                widthPoints[i, 0] = centerPoints[i] - halfWidth * perpendicular;
                widthPoints[i, 1] = centerPoints[i] + halfWidth * perpendicular;
            }
        }

        widthPoints[centerPoints.Length-1, 0] = centerPoints[centerPoints.Length-1] - halfWidth * Vector3.right;
        widthPoints[centerPoints.Length-1, 1] = centerPoints[centerPoints.Length-1] + halfWidth * Vector3.right;
    }

    // generate center points of the terrain path (returns a float array)
    void GenerateCenterPoints() // returns a vector3 array with the center points of the terrain at different z values
    {
        centerPoints = new Vector3[(int)Mathf.Ceil((endZPos-startZPos)/distanceEachZPos) + 1];

        float distanceToZero = GeneratePerlinNoise(randomPerlinNoiseStart);

        float currentZPos = startZPos;
        for(int i = 0; i < centerPoints.Length; i++)
        {
            centerPoints[i] = new Vector3( GeneratePerlinNoise(currentZPos + randomPerlinNoiseStart) - distanceToZero, 0, currentZPos);
            currentZPos += distanceEachZPos;
        }

    }

    float GeneratePerlinNoise(float z)
    {
        return Mathf.PerlinNoise(0, z / perlinLength * perlinScale) / perlinHeight * perlinScale;
    }

    //private void OnDrawGizmos()
    //{
    //    if(centerPoints.Length == 0)
    //    {
    //        return;
    //    }
    //    for(int i = 0; i < centerPoints.Length; i++)
    //    {
    //        Gizmos.DrawSphere(centerPoints[i], 0.1f);
    //    }
    //}

}
