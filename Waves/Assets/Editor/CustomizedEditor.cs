using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameObject))]
public class CostumizedEditor : Editor { // this is just to check the vertices of the planes we click on the editor, checks for diferent values in the x axis and shows the height values along the z axis of  the plane

    GameObject selectedPlane;

    private void OnEnable()
    {
        selectedPlane = (GameObject)target;
    }

    public override void OnInspectorGUI()
    {
        if (selectedPlane.name.Equals("Plane(Clone)"))
        {
            Mesh mesh = selectedPlane.GetComponent<MeshFilter>().mesh;
            Vector3[] tileVertices = mesh.vertices;

            Debug.Log("Debugging errors on this plane!");
            Debug.Log("Calculating diferences in the vertices on the X axis of this single plane.");

            bool errorFound = false;
            int cont = 0;
            for (int i = tileVertices.Length - 1; i >= 0; i--)
            {
                if (cont < 10)
                {
                    cont++;
                    if (!tileVertices[i].y.Equals(tileVertices[i - 1].y))
                    {
                        Debug.Log("Diference detected on vertices (X AXIS): " + tileVertices[i] + " and " + tileVertices[i - 1]);
                        errorFound = true;
                    }
                }
                else
                {
                    if (cont == 10)
                    {
                        cont = 0;
                    }
                }

            }

            if (!errorFound)
            {
                Debug.Log("No error found!");
            }

            Debug.Log("Showing all vertices on the Z axis of this plane.");
            for (int i = tileVertices.Length - 1; i >= 0; i-=11)
            {
                Debug.Log("Vertice: " + tileVertices[i]);
            }
        }
    }

}
