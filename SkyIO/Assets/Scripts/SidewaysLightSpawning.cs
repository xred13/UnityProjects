using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidewaysLightSpawning : MonoBehaviour {

    public Transform lightsParent;

    public LayerMask groundLayer;
    public GameObject[] lights;

    public float[] xPositionSpawning;
    public float zPositionStartSpawning;
    public float zPositionStopSpawning;

    public float zIncrement;

    float currentZPosition;

    private void Awake()
    {
        currentZPosition = zPositionStartSpawning;
    }

    public void SpawnLights()
    {
        int cont = 0;
        while(currentZPosition <= zPositionStopSpawning)
        {
            for(int i = 0; i < xPositionSpawning.Length; i++)
            {
                GameObject spawned = Instantiate(lights[cont++]);
                spawned.transform.SetParent(lightsParent);
                spawned.transform.position = new Vector3(xPositionSpawning[i], FindHeight(new Vector3(xPositionSpawning[i],20,currentZPosition)), currentZPosition);
            }
            if(cont >= lights.Length)
            {
                cont = 0;
            }

            currentZPosition += zIncrement;
        }
    }

    public float FindHeight(Vector3 pos) // finds height in terrain
    {
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(pos, Vector3.down, out hit, 1000, groundLayer))
        {
            return hit.point.y;
        }

        return 0;
    }
}
