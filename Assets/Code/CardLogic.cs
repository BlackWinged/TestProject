using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using JetBrains.Annotations;

public class CardLogic : MonoBehaviour
{
    [Header("Flip Animation Settings")]
    [SerializeField] private float flipDuration = 0.5f;
    [SerializeField] private float upwardMovement = 0.5f;
    [SerializeField] private AnimationCurve flipCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isFlipping = false;
    public bool isFlipped = false;
    private Vector3 originalPosition;
    //private Quaternion originalRotation;
    private Renderer cardRenderer;
    private Collider cardCollider;
    private Mouse mouse;
    private MemoryGameLogic memoryManager;

    void Start()
    {
        originalPosition = transform.position;
        //originalRotation = transform.rotation;

        // Get renderer component
        cardRenderer = GetComponent<Renderer>();
        if (cardRenderer == null)
        {
            cardRenderer = GetComponentInChildren<Renderer>();
        }

        // Get collider component
        cardCollider = GetComponent<Collider>();
        if (cardCollider == null)
        {
            cardCollider = GetComponentInChildren<Collider>();
        }

        // Get mouse input
        mouse = Mouse.current;

        memoryManager = FindAnyObjectByType<MemoryGameLogic>();
    }

    void Update()
    {
        CheckRaycastClick();

    }

    void CheckRaycastClick()
    {
        if (mouse != null)
        {
            // Check for left mouse button click
            if (mouse.leftButton.wasPressedThisFrame)
            {
                // Create ray from camera through mouse position
                Ray ray = Camera.main.ScreenPointToRay(mouse.position.ReadValue());
                RaycastHit hit;

                // Check if ray hits this card's collider
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider == cardCollider)
                    {
                        Flip();
                    }
                }
            }
        }
    }

    public IEnumerator FlipCard()
    {
        isFlipping = true;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = originalPosition + Vector3.up * upwardMovement;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 180, 0);

        float elapsedTime = 0f;

        while (elapsedTime < flipDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / flipDuration;
            float curveValue = flipCurve.Evaluate(t);

            // Interpolate position (move upward)
            transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);

            // Interpolate rotation (flip 180 degrees)
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, curveValue);

            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < flipDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (flipDuration / 2);
            float curveValue = flipCurve.Evaluate(t);

            // Interpolate position (move upward)
            transform.position = Vector3.Lerp(endPosition, startPosition, curveValue);


            yield return null;
        }

        // Ensure final values
        transform.position = startPosition;

        
        isFlipping = false;
    }

    // Public method to flip programmatically
    public void Flip()
    {
        if (!isFlipping)
        {
            if (memoryManager.CanFlipCard())
            {
                StartCoroutine(FlipCard());
                isFlipped = !isFlipped;
                memoryManager.RegisterAttempt();
            }
        }
    }

    // Reset card to original state
    public void ResetCard()
    {
        StopAllCoroutines();
        transform.position = originalPosition;
        //transform.rotation = originalRotation;
        isFlipped = false;
        isFlipping = false;
    }


    public class Card
    {
        public int id;
        public bool isFlipped;
        public bool isMatched;
        public Card(int id)
        {
            this.id = id;
            this.isFlipped = false;
        }
    }
}
