using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{

    private Rigidbody rb;

    public bool isRamp = false;
    public bool isBooster = false;



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("PlayerOrAI")) // both the AI and Player have tag PlayerOrAI
        {
            if (isRamp)
            {
                other.GetComponent<BallMovement>().OnRamp();
            }
            else if (isBooster)
            {
                other.GetComponent<BallMovement>().OnBooster();
            }
        }

    }




}
