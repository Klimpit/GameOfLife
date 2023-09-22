using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Camera mainCamera;

    private Vector3 dragOrigin;

    [SerializeField] private SliderClickDetector sliderClickDetector;

    [SerializeField] private float dragSpeed = 18;

    private bool isDragging = false;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        CameraDistance();
        CameraDrag();
    }
    private void CameraDistance()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            mainCamera.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * 20;
        }
    }

    private void CameraDrag()
    {
        if (!sliderClickDetector.isSliderClicked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = Input.mousePosition;
                isDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 difference = dragOrigin - Input.mousePosition;
                Vector3 newPosition = transform.position + difference * dragSpeed * Time.deltaTime;
                transform.position = newPosition;
                dragOrigin = Input.mousePosition;
            }
        }
    }
}
