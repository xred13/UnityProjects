using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public TerrainGenerator terrainGenerator;

    public float forwardSpeedForce = 40;
    public float forwardMaxSpeed = 20;

    public float boosterIncreasePercent = 100;
    public float boosterMaxVelocityPercentageIncrease = 50;

    public float rampIncreasePercent = 100;
    public float rampMaxVelocityPercentageIncrease = 50;



    enum BallState
    {
        NormalMovement,
        OnBooster,
        AfterBooster,
        OnRamp,
        AfterRamp
    }

    BallState ballState = BallState.NormalMovement;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        switch (ballState)
        {
            case BallState.NormalMovement:
                Vector2 force = terrainGenerator.inclinationVector * forwardSpeedForce; // inclination vector is the inclination vector of the terrain (x,y)

                rb.AddForce(new Vector3(0, force.y, force.x));

                rb.velocity = new Vector3(0, rb.velocity.y, Mathf.Clamp(rb.velocity.z, -forwardMaxSpeed, forwardMaxSpeed)); // we will not use physics to change the player horizontally
                break;

            case BallState.OnBooster:

                rb.velocity += rb.velocity * boosterIncreasePercent * 0.01f;

                float tempForwardMaxSpeed = forwardMaxSpeed + (forwardMaxSpeed * boosterMaxVelocityPercentageIncrease * 0.01f);
                rb.velocity = new Vector3(0, rb.velocity.y, Mathf.Clamp(rb.velocity.z, -tempForwardMaxSpeed, tempForwardMaxSpeed));

                ballState = BallState.AfterBooster;
                break;

            case BallState.AfterBooster:
                if(rb.velocity.z <= forwardMaxSpeed)
                {
                    ballState = BallState.NormalMovement;
                    break;
                }

                tempForwardMaxSpeed = forwardMaxSpeed + (forwardMaxSpeed * boosterMaxVelocityPercentageIncrease * 0.01f);
                rb.velocity = new Vector3(0, rb.velocity.y, Mathf.Clamp(rb.velocity.z, -tempForwardMaxSpeed, tempForwardMaxSpeed));

                break;

            case BallState.OnRamp:

                rb.velocity += rb.velocity * rampIncreasePercent * 0.01f;

                tempForwardMaxSpeed = forwardMaxSpeed + (forwardMaxSpeed * rampMaxVelocityPercentageIncrease * 0.01f);
                rb.velocity = new Vector3(0, rb.velocity.y, Mathf.Clamp(rb.velocity.z, -tempForwardMaxSpeed, tempForwardMaxSpeed));

                ballState = BallState.AfterRamp;
                break;

            case BallState.AfterRamp:
                if (rb.velocity.z <= forwardMaxSpeed)
                {
                    ballState = BallState.NormalMovement;
                    break;
                }

                tempForwardMaxSpeed = forwardMaxSpeed + (forwardMaxSpeed * rampMaxVelocityPercentageIncrease * 0.01f);
                rb.velocity = new Vector3(0, rb.velocity.y, Mathf.Clamp(rb.velocity.z, -tempForwardMaxSpeed, tempForwardMaxSpeed));

                break;

        }

    }

    public void OnBooster()
    {
        ballState = BallState.OnBooster;
    }

    public void OnRamp()
    {
        ballState = BallState.OnRamp;
    }
}
