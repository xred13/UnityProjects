using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    public TerrainGenerator terrainGenerator;


    public float lerpSpeed = 5;
    [Tooltip("Maximum units per second the transform can move in the X axis.")]
    public float lerpMaxSpeed;

    float clampedInput;



    void Update()
    {
        float mouseX = (Input.mousePosition.x - (Screen.width / 2)) / Screen.width * 2;

        clampedInput = Mathf.Clamp(mouseX, -1, 1);

    }

    private void FixedUpdate()
    {
        float targetX = terrainGenerator.width * clampedInput;

        float lerpedX = Mathf.Lerp(transform.position.x, targetX, lerpSpeed * Time.deltaTime);
        lerpedX = Mathf.Clamp(lerpedX, transform.position.x - (lerpMaxSpeed * Time.deltaTime), transform.position.x + (lerpMaxSpeed * Time.deltaTime)); // clamps it so we dont exceed the maximum speed

        float clampedX = Mathf.Clamp(lerpedX, 0, terrainGenerator.width); // Clamping the X position for not falling of the sides

        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}
