using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    [Header("Mouse Look Settings")]
    public float lookSensitivity = 2f;
    public bool invertY = false;
    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;

    private CharacterController controller;
    private Camera cam;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation = 0f; // Vertical rotation (pitch)
    private float yRotation = 0f; // Horizontal rotation (yaw)

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Initialize rotation to current camera rotation
        if (cam != null)
        {
            xRotation = -cam.transform.localEulerAngles.x;
            if (xRotation > 180) xRotation -= 360;
        }
        yRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleCursorToggle();
    }

    void HandleMouseLook()
    {
        // Get mouse delta from input
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;
        
        if (invertY) mouseY = -mouseY;

        // Horizontal rotation (Y-axis) - rotate the entire transform
        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        // Vertical rotation (X-axis) - rotate only the camera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        // Apply vertical rotation to camera
        if (cam != null)
        {
            cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
        else
        {
            // If no camera component, apply to this transform
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
    }

    void HandleMovement()
    {
        if (controller == null) return;

        // Move based on current rotation
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleCursorToggle()
    {
        // Toggle cursor lock with Escape key
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void OnDisable()
    {
        // Unlock cursor when script is disabled
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Input System callbacks
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
}
