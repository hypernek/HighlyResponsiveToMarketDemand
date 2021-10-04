using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera gameCamera;

    [Header("Zoom")]
    public float zoomSpeed = 60.0f; 
    public float zoomMin = 3.0f;
    public float zoomMax = 25.0f;

    private float zoom;
    private Vector3 dragOrigin;

    private void Awake()
    {
        zoom = gameCamera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        PanCamera();
        ZoomCamera();
    }

    private void PanCamera()
    {
        if(Input.GetMouseButtonDown(1))
        {
            dragOrigin = gameCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        if(Input.GetMouseButton(1))
        {
            Vector3 difference = dragOrigin - gameCamera.ScreenToWorldPoint(Input.mousePosition);

            gameCamera.transform.position += difference;
        }
    }

    private void ZoomCamera()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            zoom -= zoomSpeed * Time.deltaTime * 10f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoom += zoomSpeed * Time.deltaTime * 10f;
        }
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
        gameCamera.orthographicSize = zoom;
    }
}
