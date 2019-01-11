using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabTransformRandomizing : MonoBehaviour {

    public Vector3 minRotation, maxRotation, minScale, maxScale;

    public void Start()
    {
        transform.localScale = new Vector3(Random.Range(minScale.x, maxScale.x), Random.Range(minScale.y,maxScale.y), Random.Range(minScale.z,maxScale.z));

        transform.rotation = Quaternion.Euler(new Vector3(Random.Range(minRotation.x,maxRotation.x), Random.Range(minRotation.y,maxRotation.y), Random.Range(minRotation.z,maxRotation.z)));
    }

}
