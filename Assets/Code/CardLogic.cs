using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CardLogic : MonoBehaviour, IPointerClickHandler
{
    [Header("Flip Animation Settings")]
    [SerializeField] private float flipDuration = 0.5f;
    [SerializeField] private float upwardMovement = 0.5f;
    [SerializeField] private AnimationCurve flipCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Card Materials")]
    [Tooltip("Material for the front face of the card")]
    [SerializeField] private Material frontFaceMaterial;
    
    [Tooltip("Material for the back face of the card")]
    [SerializeField] private Material backFaceMaterial;

    private bool isFlipping = false;
    private bool isFlipped = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Renderer cardRenderer;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        
        // Get renderer component
        cardRenderer = GetComponent<Renderer>();
        if (cardRenderer == null)
        {
            cardRenderer = GetComponentInChildren<Renderer>();
        }
        
        // If materials are not assigned, try to find them
        if (cardRenderer != null && frontFaceMaterial == null)
        {
            frontFaceMaterial = cardRenderer.material;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isFlipping)
        {
            StartCoroutine(FlipCard());
        }
    }

    // Alternative: Use OnMouseDown if not using EventSystem
    void OnMouseDown()
    {
        if (!isFlipping)
        {
            StartCoroutine(FlipCard());
        }
    }

    IEnumerator FlipCard()
    {
        isFlipping = true;
        
        Vector3 startPosition = transform.position;
        Vector3 endPosition = originalPosition + Vector3.up * upwardMovement;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = originalRotation * Quaternion.Euler(0, 180, 0);
        
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
            
            // Switch material at halfway point (when card is edge-on)
            if (t >= 0.5f && !isFlipped)
            {
                SwitchMaterial();
            }
            else if (t < 0.5f && isFlipped)
            {
                SwitchMaterial();
            }

            yield return null;
        }

        // Ensure final values
        transform.position = endPosition;
        transform.rotation = endRotation;
        
        isFlipped = !isFlipped;
        isFlipping = false;
    }

    void SwitchMaterial()
    {
        if (cardRenderer != null && frontFaceMaterial != null && backFaceMaterial != null)
        {
            if (isFlipped)
            {
                cardRenderer.material = frontFaceMaterial;
            }
            else
            {
                cardRenderer.material = backFaceMaterial;
            }
        }
    }

    // Public method to flip programmatically
    public void Flip()
    {
        if (!isFlipping)
        {
            StartCoroutine(FlipCard());
        }
    }

    // Reset card to original state
    public void ResetCard()
    {
        StopAllCoroutines();
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        isFlipped = false;
        isFlipping = false;
        
        if (cardRenderer != null && frontFaceMaterial != null)
        {
            cardRenderer.material = frontFaceMaterial;
        }
    }

    public bool IsFlipped => isFlipped;
    public bool IsFlipping => isFlipping;

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
