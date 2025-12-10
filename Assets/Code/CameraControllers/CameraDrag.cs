using UnityEngine;
using UnityEngine.InputSystem;
using TestProject;

public class CameraDrag : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private float dragSpeed = 2.0f;
    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = false;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 2.0f;
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private bool invertZoom = false;

    [Header("Bounds (Optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds = new Vector2(-10, -10);
    [SerializeField] private Vector2 maxBounds = new Vector2(10, 10);

    private Vector3 dragOrigin;
    private bool isDragging = false;
    private Mouse mouse;
    private Camera cam;

    void Start()
    {
        mouse = Mouse.current;
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Update()
    {
        if (mouse == null) return;

        // Check for mouse button down (left mouse button)
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = mouse.position.ReadValue();
            dragOrigin = Utils.GetWorldPoint(mousePos);
            isDragging = true;
        }

        // Check if mouse button is held down and dragging
        if (mouse.leftButton.isPressed && isDragging)
        {
            Vector2 mousePos = mouse.position.ReadValue();
            Vector3 currentPos = Utils.GetWorldPoint(mousePos);
            Vector3 difference = dragOrigin - currentPos;

            // Apply invert settings
            if (invertX) difference.x = -difference.x;
            if (invertY) difference.y = -difference.y;

            // Move camera
            Vector3 newPosition = transform.position + difference * dragSpeed;

            // Apply bounds if enabled
            if (useBounds)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
                newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);
            }

            transform.position = newPosition;
        }



        // Check for mouse button up
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        // Handle zoom with scroll wheel
        if (cam != null)
        {
            Vector2 scrollDelta = mouse.scroll.ReadValue();
            if (scrollDelta.y != 0)
            {
                float zoomDelta = scrollDelta.y * zoomSpeed * Time.deltaTime;
                if (invertZoom) zoomDelta = -zoomDelta;

                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomDelta, minZoom, maxZoom);
            }
        }
    }
}

