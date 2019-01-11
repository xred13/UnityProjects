using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraMovement : MonoBehaviour
{

    public GameObject player;

    [Tooltip("Velocity of the player (magnitude) after which the camera starts to change perspective.")]
    public float minVelocityCameraChanges = 40;

    [Tooltip("Default camera offset in relation to the player.")]
    public float offsetY = 3, offsetZ = -10;
    [Tooltip("Distance the camera will go behind/forward and up/down.")]
    public float distanceZ = 20, distanceY = 10;
    [Tooltip("Time the camera will take to reach those distances. This will influence the speed at which the camera moves once the player reaches that speed.")]
    public float setupTimeZ = 4, setupTimeY;

    float velocityY, velocityZ;
    float distantOffSetY = 0, distantOffSetZ = 0;

    [Tooltip("Camera rotates in the X axis until it reaches this value.")]
    public float xAngleMax = 30;
    [Tooltip("Time it takes for the camera to reach the maximum rotation.")]
    public float xAngleSetupTime = 4;

    float velocityAngleX;
    float distantxAngleMax = 0;

    enum State
    {
        Increasing,
        Decreasing,
        MaintainNormal,
        MaintainIncreased,
    }

    State currentState = State.MaintainNormal;


    private void Start()
    {
        velocityZ = distanceZ / setupTimeZ;
        velocityY = distanceY / setupTimeY;

        velocityAngleX = xAngleMax / xAngleSetupTime;
    }

    private void LateUpdate()
    {
        switch (currentState)
        {
            case State.MaintainNormal:

                transform.position = player.transform.position + new Vector3(0, offsetY, offsetZ);

                if(player.GetComponent<Rigidbody>().velocity.magnitude >= minVelocityCameraChanges) // if the velocity is greater or equal to minVelocityCameraChanges, then we should increase
                {
                    currentState = State.Increasing;
                }
                break;

            case State.Increasing:

                #region Camera Position in a Line
                
                if(distantOffSetY != distanceY)
                {
                    distantOffSetY += velocityY * Time.deltaTime;

                    if (distantOffSetY >= distanceY) // if the position is already max, we should maintain
                    {
                        distantOffSetY = distanceY;
                    }
                }

                if(distantOffSetZ != distanceZ)
                {

                    distantOffSetZ += -velocityZ * Time.deltaTime;

                    if (distantOffSetZ <= -distanceZ) // if the position is already max, we should maintain
                    {
                        distantOffSetZ = -distanceZ;
                    }

                }
                transform.position = player.transform.position + new Vector3(0, offsetY, offsetZ) + new Vector3(0, distantOffSetY, distantOffSetZ); // sets the position based on the distantOffSetZ and Y and offsetZ and Y

                #endregion

                #region Camera Rotation
                if (distantxAngleMax != xAngleMax) // if the rotation still hasnt reached its peak, then we have to adjust it
                {
                    distantxAngleMax += velocityAngleX * Time.deltaTime;

                    if (distantxAngleMax >= xAngleMax) // if the rotation has already reached its peak or gone over it
                    {
                        distantxAngleMax = xAngleMax;
                    }

                    transform.rotation = Quaternion.Euler(distantxAngleMax, 0, 0);
                }
                #endregion

                #region State Change

                if (player.GetComponent<Rigidbody>().velocity.magnitude < minVelocityCameraChanges) // if velocity smaller then minVelocityCameraChanges, we should decrease
                {
                    currentState = State.Decreasing;
                }
                else if (distantOffSetY == distanceY && distantOffSetY == distanceZ && distantxAngleMax == xAngleMax)
                {
                    currentState = State.MaintainIncreased;
                }

                #endregion
                break;

            case State.Decreasing:

                #region Camera Position in a Line

                if (distantOffSetY != 0)
                {
                    distantOffSetY -= velocityY * Time.deltaTime;

                    if (distantOffSetY <= 0) // if this position is equal or smaller to 0, then we maintain the normal 
                    {
                        distantOffSetY = 0;
                    }
                }

                if(distantOffSetZ != 0)
                {
                    distantOffSetZ -= -velocityZ * Time.deltaTime;

                    if (distantOffSetZ >= 0) // if this position is equal or smaller to 0, then we maintain the normal value
                    {
                        distantOffSetZ = 0;
                    }

                }

                transform.position = player.transform.position + new Vector3(0, offsetY, offsetZ) + new Vector3(0, distantOffSetY, distantOffSetZ); // set the position of the camera

                #endregion

                #region Camera Rotation

                if (distantxAngleMax != 0) // if rotation still isn't the normal,we adjust it
                {
                    distantxAngleMax -= velocityAngleX * Time.deltaTime;

                    if (distantxAngleMax <= 0) // if the rotation is smaller or equal to the normal value
                    {
                        distantxAngleMax = 0; // adjust it to the normal value
                    }

                    transform.rotation = Quaternion.Euler(distantxAngleMax, 0, 0);
                }
                #endregion

                #region State Changes
                if (player.GetComponent<Rigidbody>().velocity.magnitude >= minVelocityCameraChanges) // if velocity is greater then minVelocityCameraChanges, then state = increasing
                {
                    currentState = State.Increasing;
                }
                else if (distantOffSetZ == 0 && distantOffSetY == 0 && distantxAngleMax == 0)
                {
                    currentState = State.MaintainNormal;
                }
                #endregion


                break;

            case State.MaintainIncreased: // maintains maximum position, doesn't have to change anything about the rotation

                transform.position = player.transform.position + new Vector3(0, offsetY, offsetZ) + new Vector3(0, distanceY, -distanceZ);
                if (player.GetComponent<Rigidbody>().velocity.magnitude < minVelocityCameraChanges) // if velocity lowers, we should decrease
                {
                    currentState = State.Decreasing;
                }
                break;

        }
    }
}
