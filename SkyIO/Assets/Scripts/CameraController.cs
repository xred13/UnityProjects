using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 offSet;

    private void Start()
    {
        transform.position = player.transform.position + offSet;
    }

    private void Update()
    {
        transform.position = player.transform.position + offSet;
    }

}
