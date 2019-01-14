using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public Transform reference;
    public float radius;
    public float lerpSpeed;
    public float horizontalMultiplier;
    public float forwardVelocity;

    Vector3[] centerPoints;

    int currCenterPointsIndex = 1;
    [HideInInspector]
    public Vector3 currDirection;

    public static PlayerManager instance = null;
    Rigidbody rigidbody;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }

        rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        centerPoints = TerrainGenerator.instance.centerPoints;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentCenterPointsIndex();
        reference.position = centerPoints[currCenterPointsIndex];

        // we update the direction of the path based on our current position
        currDirection = centerPoints[currCenterPointsIndex+1] - centerPoints[currCenterPointsIndex-1];
        currDirection = currDirection.normalized;

        HorizontalMovement();
    }

    private void FixedUpdate()
    {
        ForwardMovement();
    }

    void UpdateCurrentCenterPointsIndex()
    {
        while (currCenterPointsIndex < centerPoints.Length && transform.position.z > centerPoints[currCenterPointsIndex].z) // update our reference in the center points array
        {
            currCenterPointsIndex++;
        }
    }

    void HorizontalMovement()
    {
        Vector3 horizontalDirection = new Vector3(currDirection.z,currDirection.y,-currDirection.x);
        float mouseX = transform.position.x + horizontalDirection.x * (Input.mousePosition.x - (Screen.width / 2)) / (Screen.width / 2) * horizontalMultiplier;
        Vector3 currentPos = transform.position;
        currentPos.x = Mathf.Lerp(currentPos.x, mouseX, Time.deltaTime * lerpSpeed);

        transform.position = currentPos;
    }

    void ForwardMovement()
    {
        Vector3 vel = forwardVelocity * currDirection;
        rigidbody.velocity = new Vector3(vel.x, rigidbody.velocity.y, vel.z);
    }
}
