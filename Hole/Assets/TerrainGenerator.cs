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

    Vector3[] centerPoints = new Vector3[0];
    float[,] widthPoints;

    Vector3[] vertices;
    int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        randomPerlinNoiseStart = Random.Range(0, 500);

        GenerateCenterPoints();
        GenerateWidthPoints();

        GenerateMesh();


    }

    private void Update()
    {
        GenerateCenterPoints();

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
            vertices[i] = new Vector3( widthPoints[i/2, 0], 0, centerPoints[i/2].z);
            vertices[i + 1] = new Vector3( widthPoints[i/2, 1], 0, centerPoints[i/2].z);
        }
    }

    void GenerateWidthPoints() // for each index of the centerpoints array, we have an array with 2 values, one for the left side width x position, the other for the right side
    {
        widthPoints = new float[centerPoints.Length,2];

        float halfWidth = width / 2;
        for(int i = 0; i < widthPoints.GetLength(0); i++)
        {
            if(centerPoints[i].z + (tightWidthPointsHalfLength*2 + tightWidthPointsMaintain)*distanceEachZPos <= endZPos && Random.Range(0, centerPoints.Length) < tightWidthChance) // if there's space left for tightening the terrain and there's chance
            {
                float smallestWidth = Random.Range(minTightWidth, maxTightWidth);
                float difference = (width - smallestWidth) / 2 / tightWidthPointsHalfLength;

                float currentHalfWidth = halfWidth;

                int cont = 0;

                for (int k = 0; k < tightWidthPointsHalfLength; k++) // decrease width
                {
                    widthPoints[i + k, 0] = centerPoints[i + k].x - currentHalfWidth + difference;
                    widthPoints[i + k, 1] = centerPoints[i + k].x + currentHalfWidth - difference;

                    currentHalfWidth -= difference;

                    cont++;
                }

                int reference = cont;
                for (int k = reference; k < reference + tightWidthPointsMaintain; k++) // maintain smallest width
                {
                    widthPoints[i + k, 0] = centerPoints[i + k].x - currentHalfWidth;
                    widthPoints[i + k, 1] = centerPoints[i + k].x + currentHalfWidth;

                    cont++;
                }

                reference = cont;
                for (int k = reference; k < reference + tightWidthPointsHalfLength; k++) // increase width to normal
                {
                    widthPoints[i + k, 0] = centerPoints[i + k].x - currentHalfWidth - difference;
                    widthPoints[i + k, 1] = centerPoints[i + k].x + currentHalfWidth + difference;

                    currentHalfWidth += difference;

                    cont++;
                }


                i += cont - 1;
            }
            else // we go with normal terrain
            {
                widthPoints[i, 0] = centerPoints[i].x - halfWidth;
                widthPoints[i, 1] = centerPoints[i].x + halfWidth;
            }

        }
    }

    // generate center points of the terrain path (returns a float array)
    void GenerateCenterPoints() // returns a vector3 array with the center points of the terrain at different z values
    {
        centerPoints = new Vector3[(int)Mathf.Ceil((endZPos-startZPos)/distanceEachZPos) + 1];


        float currentZPos = startZPos;
        for(int i = 0; i < centerPoints.Length; i++)
        {
            centerPoints[i] = new Vector3( GeneratePerlinNoise(currentZPos + randomPerlinNoiseStart), 0, currentZPos);
            currentZPos += distanceEachZPos;
        }

    }

    float GeneratePerlinNoise(float z)
    {
        return Mathf.PerlinNoise(0, z / perlinLength * perlinScale) / perlinHeight * perlinScale;
    }

    private void OnDrawGizmos()
    {
        if(centerPoints.Length == 0)
        {
            return;
        }
        for(int i = 0; i < centerPoints.Length; i++)
        {
            Gizmos.DrawSphere(centerPoints[i], 0.1f);
        }
    }

}
