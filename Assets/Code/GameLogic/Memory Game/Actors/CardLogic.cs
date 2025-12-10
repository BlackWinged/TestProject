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

    public string CardId = "";

    private bool isFlipping = false;
    public bool isFlipped = false;
    private Vector3 originalPosition;
    public bool IsDiscarded = false;
    private Renderer cardRenderer;
    private Collider cardCollider;
    private Mouse mouse;
    private MemoryGameController memoryManager;

    void Start()
    {
        originalPosition = transform.position;

        cardRenderer = GetComponent<Renderer>();
        if (cardRenderer == null)
        {
            cardRenderer = GetComponentInChildren<Renderer>();
        }

        cardCollider = GetComponent<Collider>();
        if (cardCollider == null)
        {
            cardCollider = GetComponentInChildren<Collider>();
        }

        mouse = Mouse.current;
        memoryManager = FindAnyObjectByType<MemoryGameController>();
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
                if (Physics.Raycast(ray, out hit) && !IsDiscarded)
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

            transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, curveValue);

            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < flipDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (flipDuration / 2);
            float curveValue = flipCurve.Evaluate(t);

            transform.position = Vector3.Lerp(endPosition, startPosition, curveValue);

            yield return null;
        }

        transform.position = startPosition;
        isFlipping = false;
    }

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
    
    public IEnumerator Discard(Vector3 startPosition, Vector3 endPosition)
    {
        float elapsedTime = 0f;
        StopAllCoroutines();

        while (elapsedTime < flipDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / flipDuration;
            float curveValue = flipCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);

            yield return null;
        }
    }

}
