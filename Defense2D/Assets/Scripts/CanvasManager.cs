using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{

    RectTransform canvasRect;

    void Start()
    {
        canvasRect = GetComponent<RectTransform>();
    }


    public Vector2 WorldToCanvasPoint(Vector3 position)
    {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(position);
        Vector2 worldObject_ScreenPosition = new Vector2(viewportPosition.x * canvasRect.sizeDelta.x,viewportPosition.y * canvasRect.sizeDelta.y);
        return worldObject_ScreenPosition;
    }
}
