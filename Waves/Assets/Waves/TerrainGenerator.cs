using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour {

    public GameObject barrier; //stops player from going backwards
    public Vector3 barrierSpawnPos;

    public GameObject player;
    [Tooltip("A plane used to generate the waves.")]
    public GameObject tile;
    [Tooltip("Number of tiles (planes) one each wave.")]
    public int dedicatedTiles = 5;
    [Tooltip("Number of waves alive at each time.")]
    public int dedicatedWaves = 10;
    [Tooltip("After the player's z position surpasses this value, the float is incremented by 50 and we add a wave and remove the oldest one.")]
    public float playerPosReference = 50;

    [Tooltip("Values for the wave's generation. We give a min and max and we get random values between those. The values are a bit weird because of the function implemented (normal distribution).")]
    public float minWidth, maxWidth, minHeight, maxHeight; // o and g min and max values on the normaldistributionfunction
    [Tooltip("Z position in the world where the first plane is spawned.")]
    public float planeSpawnPosition = 0; // incremented each time we set a plane by 10 * localscale.z (tileSize)

    float? planeLastVertexHeight = null;
    float planeLastVertexDifference = 0;
    int cont = 0; // resets to 0 every time we set a wave
    [HideInInspector]
    public float tileSize; // calculated based on dedicatedTiles * scale.z
    [HideInInspector]
    public float waveSize; // tileSize * dedicated tiles

    Queue<GameObject[]> waveQueue = new Queue<GameObject[]>(); // the queue that holds all the instantiated waves that will be modified and swapped during runtime


    void Initialization() 
    {
        barrier = Instantiate(barrier);
        barrier.transform.position = barrierSpawnPos;

        // initializing the tilesize
        tileSize = tile.transform.localScale.z * 10;
        waveSize = tileSize * dedicatedTiles;

        // initializing the queue
        for(int k = 0; k < dedicatedWaves; k++) 
        {
            GameObject[] wave; // array of tiles that forms a wave

            // initializing a wave
            wave = new GameObject[dedicatedTiles];
            for (int i = 0; i < wave.Length; i++)
            {
                wave[i] = Instantiate(tile);
            }

            waveQueue.Enqueue(wave);
        }

        // going through the queue and setting the waves
        for (int l = 0; l < waveQueue.Count; l++)
        {
            SetFirstWaveInQueue();
        }

    }

    void Awake () {
        Initialization();
    }

    private void Update()
    {
        if (player.transform.position.z > playerPosReference)
        {
            SetFirstWaveInQueue();
            playerPosReference += tileSize*dedicatedTiles;

            barrierSpawnPos.z += tileSize * dedicatedTiles; // update barrier spawn position vector and change the barrier position 
            barrier.transform.position = barrierSpawnPos;
        }
    }

    void SetFirstWaveInQueue() // peeks at the queue, sets the wave, enqueue's it again
    {
        GameObject[] tempWave = waveQueue.Peek();
        waveQueue.Dequeue();

        SetWave(tempWave);

        waveQueue.Enqueue(tempWave);
    }

    void SetWave(GameObject[] tempWave) 
    {
        // initialize width and height values for the wave
        float width = RandomWidth(), height = RandomHeight();

        cont = 0;
        for (int k = 0; k < dedicatedTiles; k++)
        {
            GameObject tempPlane = tempWave[k];
            PlaneSetHeight(tempPlane, width, height); // sets a plane's vertices height
            
        }
    }

    float RandomWidth()
    {
        return Random.Range(minWidth,maxWidth);
    }

    float RandomHeight() 
    {
        return Random.Range(minHeight, maxHeight);
    }

    void PlaneSetHeight(GameObject plane, float o, float g) // cont starts at 0 and is incremented by 10 each time we call this function
    {
        // setting the plane's position
        plane.transform.position = new Vector3(0,0,planeSpawnPosition);
        planeSpawnPosition += tileSize;

        float length = NormalDistributionFunctionLength(0, 0, o, g);
        float zIncrement = length / (10 * dedicatedTiles);

        float verticesPosZ = cont * zIncrement - length/2;

        Mesh mesh = plane.GetComponent<MeshFilter>().mesh;
        Vector3[] tileVertices = mesh.vertices;

        if(planeLastVertexHeight!=null && cont == 0 && planeLastVertexDifference == 0) // sets the difference (difference between last wave's vertex height and this wave's first vertex)
        {
            planeLastVertexDifference = (float)planeLastVertexHeight-NormalDistributionFunction(length/2, 0, o, g);
        }

        int counting = 0;
        for (int k = tileVertices.Length - 1; k >= 0; k--) // we go through all the vertices in the tile
        {
            if((k >= tileVertices.Length-11) && planeLastVertexHeight != null && planeSpawnPosition > dedicatedTiles*tileSize) // if first line of plane and not first plane of game and not first wave
            {
                tileVertices[k] = new Vector3(tileVertices[k].x, (float)planeLastVertexHeight, tileVertices[k].z); // first line of vertexes of the wave's first plane is on the position of last wave's last plane's line of vertices
            }
            else
            {
                // setting the vertices height
                tileVertices[k] = new Vector3(tileVertices[k].x, NormalDistributionFunction(verticesPosZ, 0, o, g) + planeLastVertexDifference, tileVertices[k].z); // we add the difference, which it will be 0 on the first wave

                if (counting == 10) // after each line of vertices(11) we increment the posZ that'll be used to calculate the height based on the normal distribution function
                {
                    verticesPosZ += zIncrement;
                    counting = 0;
                }
                else
                {
                    counting++;
                }
            }


            if (k == 0) // last vertex of the plane
            {
                planeLastVertexHeight = tileVertices[k].y;
            }

        }

        // applying changes to vertices and rebuilding the mesh collider to the new mesh
        mesh.vertices = tileVertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        Destroy(plane.GetComponent<MeshCollider>());
        plane.AddComponent<MeshCollider>();

        if(cont == dedicatedTiles*10 - 10) // last plane of a wave
        {
            planeLastVertexDifference = 0;
            cont = 0;
        }
        else
        {
            // incrementing cont (10 for each plane of a wave, resets to 0 in the last plane of a wave
            cont += 10;
        }

    }

    float NormalDistributionFunction(float x, float j, float o, float g) // x usually starts at 0, j is always 0
    {
        return Mathf.Pow((float)Math.E, -0.5f * Mathf.Pow((x - j) / o, 2)) / (g * Mathf.Sqrt(2 * Mathf.PI));
    }

    float NormalDistributionFunctionLength(float x, float j, float o, float g) // x usually starts at 0, j is always 0
    {
        bool foundIt = false;
        float threshHold = 0.0001f; // difference between y values
        float increment = 0.05f; // each time x increments
        float tempY = NormalDistributionFunction(x,j,o,g);

        while (!foundIt)
        {
            x += increment;
            float tempNumber = NormalDistributionFunction(x,j,o,g);

            if (tempNumber < tempY - threshHold)
            {
                tempY = tempNumber;
            }
            else
            {
                foundIt = true;
            }
        }
        return x;
    }
}


