using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerManagerV2 : MonoBehaviour
{
    Rigidbody rb;
    SphereCollider cl;

    [Tooltip("Time it takes after the player goes into the air to enable clicking and adding a downwards force.")]
    public float downForceClickEnableTime = 0.2f; // time after the player goes in the air after which the player can add downwards force
    bool canClickDown = true;
    bool canClickDownCoroutineRunning = false;

    [Tooltip("Minimum time the player has to be in the air before falling for us to calculate the score and streak fail/succession/increments/resets.")]
    public float minimumSecondsOnAirToCheck;
    [HideInInspector]
    public bool weCanCheck = true; 
    bool checkCoroutineRunning;

    public int downForce, forwardForce;
    public float maxAngularVelocity;
    public LayerMask terrain;
    bool onAir;
    [HideInInspector]
    public bool grounded = false;

    public delegate void Landing(float z);
    public event Landing OnLanding;

    private void Start()
    {
        cl = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
    }


    private void FixedUpdate()
    {
        //check if the sphere is touching the ground
        if (Physics.CheckSphere(cl.bounds.center, cl.radius+1.02f, terrain, QueryTriggerInteraction.UseGlobal)) // if player is grounded
        {
            grounded = true;

            if ((Input.GetMouseButton(0) || Input.touchCount > 0)) // on touch when sphere is grounded, we add a force
            {
                rb.AddForce(Vector3.forward * forwardForce * Time.fixedDeltaTime, ForceMode.Acceleration);
            }

            if (onAir) // if object was on air and is now on ground
            {
                onAir = false;
                OnLanding(Mathf.Round(transform.position.z));
            }

        }
        else // if player isn't grounded
        {
            grounded = false;

            if (!onAir) // if player was previously on the ground, if it is falling, bool is true
            {
                if (!canClickDownCoroutineRunning) // starts a timer, after a cetain time, canClickDown turns to true
                {
                    StartCoroutine(CanClickDown());
                }
                else
                {
                    StopCoroutine(CanClickDown());
                    StartCoroutine(CanClickDown());
                }

                if (!checkCoroutineRunning)
                {
                    StartCoroutine(CheckCoroutine()); // starts a timer, after a certain time, weCanCheck turns to true
                }
                else
                {
                    StopCoroutine(CheckCoroutine());
                    StartCoroutine(CheckCoroutine());
                }
            }

            if ((Input.GetMouseButton(0) || Input.touchCount > 0) && canClickDown)
            {
                rb.AddForce(Vector3.down * downForce * Time.fixedDeltaTime, ForceMode.Acceleration);
            }
            onAir = true;
        }

    }

    IEnumerator CheckCoroutine()
    {
        checkCoroutineRunning = true;
        weCanCheck = false;
        yield return new WaitForSecondsRealtime(minimumSecondsOnAirToCheck);
        weCanCheck = true;
        checkCoroutineRunning = false;
    }

    IEnumerator CanClickDown()
    {
        canClickDownCoroutineRunning = true;
        canClickDown = false;
        yield return new WaitForSecondsRealtime(downForceClickEnableTime);
        canClickDown = true;
        canClickDownCoroutineRunning = false;
    }

}
