using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    public ObstacleSpawner obstacleSpawner;
    public SidewaysLightSpawning sidewaysLightSpawning;

    public GameObject terrainParent;
    public GameObject plane;

    public float width, length;
    public int widthPlaneNumber, lengthPlaneNumber;

    public float inclinationAngle;
    public Vector2 inclinationVector;

    public float perlinZoom, perlinHeight;


    GameObject[][] planes;
    float m; // the inclination of the terrain (y = x*m)

    public void Initialization() // initialize variables and terrain, obstacles, lights
    {
        // setting plane's rotation and the vector that'll be used to calculate the inclination of the terrain
        plane.transform.rotation = Quaternion.Euler(inclinationAngle, 0, 0);
        inclinationVector = new Vector2(Mathf.Cos(-inclinationAngle * Mathf.Deg2Rad), Mathf.Sin(-inclinationAngle * Mathf.Deg2Rad));
        m = inclinationVector.y / inclinationVector.x;


        // setting the plane's scale
        float planeScaleX = (width / widthPlaneNumber) / 10;
        float planeScaleZ = (length / lengthPlaneNumber) / 10;

        plane.transform.localScale = new Vector3(planeScaleX,1,planeScaleZ);

        // setting planes array [columns][rows]
        planes = new GameObject[lengthPlaneNumber][];
        for(int i = 0; i < planes.Length; i++)
        {
            planes[i] = new GameObject[widthPlaneNumber];
        }

        GenerateTerrain(); // we create the terrain

        WeldPlanes(); // join all the planes into one big mesh and remove close vertices

        obstacleSpawner.SpawnObstacles(); // spawn the obstacles

        sidewaysLightSpawning.SpawnLights();
    }

    void Awake () {
        Initialization(); // initialize some variables
    }


    void GenerateTerrain()
    {
        for(int i = 0; i < planes.Length; i++) // goes through each row of planes
        {
            for(int k = 0; k < planes[i].Length; k++) // goes through each plane in a row
            {
                GameObject spawnedPlane = Instantiate(plane);

                float zPos = ((inclinationVector.normalized * length).x /lengthPlaneNumber) * i + (((inclinationVector.normalized * length).x / lengthPlaneNumber)/2); 
                float xPos = k * (width/widthPlaneNumber) + ((width/widthPlaneNumber)/2);

                spawnedPlane.transform.position = new Vector3(xPos,YFunction(zPos),zPos);
                spawnedPlane = SetPlanePerlinNoise(spawnedPlane);
                spawnedPlane.transform.parent = terrainParent.transform;

                planes[i][k] = spawnedPlane; 
            }
        }
    }

    GameObject SetPlanePerlinNoise(GameObject planeSpawned) // receives a plane, sets the perlin noise and returns the same plane
    {
        Mesh mesh = planeSpawned.GetComponent<MeshFilter>().mesh;
        Vector3[] tileVertices = mesh.vertices;

        for(int i = tileVertices.Length-1; i >= 0; i--)
        {
            Vector3 worldPos = planeSpawned.transform.TransformPoint(tileVertices[i]);
            Vector2 pos = new Vector2(worldPos.x, worldPos.z);

            tileVertices[i].y = PerlinNoiseFunction(pos);
        }

        mesh.vertices = tileVertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        // we dont need to destroy and remove collider because we're joining all the planes and doing it on only one gameobject

        return planeSpawned;
    }



    float YFunction(float x) // used to calculate the plane's position, because we have inclinated terrain
    {
        return x * m;
    }

    float PerlinNoiseFunction(Vector2 pos) 
    {
        float height = Mathf.PerlinNoise((pos.x) * perlinZoom, (pos.y) * perlinZoom) * perlinHeight;
        return height;

    }

    void WeldPlanes() // makes only one mesh with all the planes
    {
        MeshFilter[] meshFilters = new MeshFilter[planes.Length * planes[0].Length];

        int cont = 0;
        for (int i = 0; i < planes.Length; i++)
        {
            for(int g = 0; g < planes[0].Length; g++)
            {
                meshFilters[cont++] = planes[i][g].GetComponent<MeshFilter>();
            }
        }

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int k = 0;
        while (k < meshFilters.Length)
        {
            combine[k].mesh = meshFilters[k].sharedMesh;
            combine[k].transform = meshFilters[k].transform.localToWorldMatrix;
            meshFilters[k].gameObject.SetActive(false);
            k++;
        }

        Mesh tempMesh = new Mesh();
        tempMesh.CombineMeshes(combine);

        AutoWeld(tempMesh, 0.1f, 1);

        terrainParent.GetComponent<MeshFilter>().mesh = tempMesh;
        terrainParent.gameObject.SetActive(true);

        Destroy(terrainParent.transform.GetComponent<MeshCollider>());
        terrainParent.gameObject.AddComponent<MeshCollider>();
    }

    public static void AutoWeld(Mesh mesh, float threshold, float bucketStep) // removes close vertices
    {
        Vector3[] oldVertices = mesh.vertices;
        Vector3[] newVertices = new Vector3[oldVertices.Length];
        int[] old2new = new int[oldVertices.Length];
        int newSize = 0;

        // Find AABB
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        for (int i = 0; i < oldVertices.Length; i++)
        {
            if (oldVertices[i].x < min.x) min.x = oldVertices[i].x;
            if (oldVertices[i].y < min.y) min.y = oldVertices[i].y;
            if (oldVertices[i].z < min.z) min.z = oldVertices[i].z;
            if (oldVertices[i].x > max.x) max.x = oldVertices[i].x;
            if (oldVertices[i].y > max.y) max.y = oldVertices[i].y;
            if (oldVertices[i].z > max.z) max.z = oldVertices[i].z;
        }

        // Make cubic buckets, each with dimensions "bucketStep"
        int bucketSizeX = Mathf.FloorToInt((max.x - min.x) / bucketStep) + 1;
        int bucketSizeY = Mathf.FloorToInt((max.y - min.y) / bucketStep) + 1;
        int bucketSizeZ = Mathf.FloorToInt((max.z - min.z) / bucketStep) + 1;
        List<int>[,,] buckets = new List<int>[bucketSizeX, bucketSizeY, bucketSizeZ];

        // Make new vertices
        for (int i = 0; i < oldVertices.Length; i++)
        {
            // Determine which bucket it belongs to
            int x = Mathf.FloorToInt((oldVertices[i].x - min.x) / bucketStep);
            int y = Mathf.FloorToInt((oldVertices[i].y - min.y) / bucketStep);
            int z = Mathf.FloorToInt((oldVertices[i].z - min.z) / bucketStep);

            // Check to see if it's already been added
            if (buckets[x, y, z] == null)
                buckets[x, y, z] = new List<int>(); // Make buckets lazily

            for (int j = 0; j < buckets[x, y, z].Count; j++)
            {
                Vector3 to = newVertices[buckets[x, y, z][j]] - oldVertices[i];
                if (Vector3.SqrMagnitude(to) < threshold)
                {
                    old2new[i] = buckets[x, y, z][j];
                    goto skip; // Skip to next old vertex if this one is already there
                }
            }

            // Add new vertex
            newVertices[newSize] = oldVertices[i];
            buckets[x, y, z].Add(newSize);
            old2new[i] = newSize;
            newSize++;

            skip:;
        }

        // Make new triangles
        int[] oldTris = mesh.triangles;
        int[] newTris = new int[oldTris.Length];
        for (int i = 0; i < oldTris.Length; i++)
        {
            newTris[i] = old2new[oldTris[i]];
        }

        Vector3[] finalVertices = new Vector3[newSize];
        for (int i = 0; i < newSize; i++)
            finalVertices[i] = newVertices[i];

        mesh.Clear();
        mesh.vertices = finalVertices;
        mesh.triangles = newTris;
        mesh.RecalculateNormals();
    }
}
