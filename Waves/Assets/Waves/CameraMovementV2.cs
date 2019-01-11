using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementV2 : MonoBehaviour
{

    public Transform player;
    public float offsetX = 0, offsetY = 3, offsetZ = -10;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(player.position.x + offsetX, player.position.y + offsetY, player.position.z + offsetZ);
    }
}
