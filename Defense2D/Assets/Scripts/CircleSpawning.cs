using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSpawning : MonoBehaviour
{
    public LayerMask circleLayerMask;

    public Transform reference;
    public GameManager gameManager;

    public void MoveSpawnedCircleToPosition(GameObject circle, int step, out bool spawned)
    {
        Circle circleComponent = circle.GetComponent<Circle>();

        List<float> degrees = new List<float>();
        if (CanSpawn(circle.GetComponent<CircleCollider2D>().radius * circle.transform.localScale.x, step, gameManager.rotateAroundMinimumDegrees, out degrees))
        {
            float randomDegrees = degrees[Random.Range(0, degrees.Count)];
            circle.transform.position = new Vector3(gameManager.minimumStepDistance + step * gameManager.stepSize, 0, 0);
            circle.transform.RotateAround(Vector3.zero, new Vector3(0, 0, 1), randomDegrees);
            spawned = true;
        }
        else
        {
            spawned = false;
        }

    }

    bool CanSpawn(float size, int step, float rotateAroundMinimumDegrees, out List<float> degrees) // returns true if there are values in the degrees float list, the degree floats list has all the degrees where it can be spawned
    {
        reference.position = new Vector3(gameManager.minimumStepDistance + step * gameManager.stepSize, 0, 0);
        float currentDegrees = 0;

        degrees = new List<float>();

        while (currentDegrees < 360)
        {
            bool unableToSpawn = false;
            foreach (GameObject circle in gameManager.circles)
            {
                float biggestSize = size;
                if ((circle.GetComponent<CircleCollider2D>().radius * circle.transform.localScale.x) > size)
                {
                    biggestSize = circle.GetComponent<CircleCollider2D>().radius * circle.transform.localScale.x;
                }

                biggestSize += gameManager.additionalCircleSpawnDistance;

                if ((Vector3.Distance(reference.position, circle.transform.position) - size - (circle.GetComponent<CircleCollider2D>().radius * circle.transform.localScale.x)) <= gameManager.additionalCircleSpawnDistance)
                {
                    unableToSpawn = true;
                    break;
                }
            }

            if (!unableToSpawn)
            {
                degrees.Add(currentDegrees);
            }
            reference.RotateAround(Vector3.zero, new Vector3(0, 0, 1), rotateAroundMinimumDegrees);
            currentDegrees += rotateAroundMinimumDegrees;

        }



        if (degrees.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
