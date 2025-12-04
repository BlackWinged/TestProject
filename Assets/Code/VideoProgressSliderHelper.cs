using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Helper component to attach to a UI Slider for video progress tracking.
/// This handles the drag events to prevent slider updates while user is dragging.
/// Attach this to the same GameObject as your Slider component.
/// </summary>
[RequireComponent(typeof(Slider))]
public class VideoProgressSliderHelper : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    private Slider slider;
    private VideoPlayer3D videoPlayer3D;

    void Start()
    {
        slider = GetComponent<Slider>();
        
        // Try to find VideoPlayer3D in the scene
        if (videoPlayer3D == null)
        {
            videoPlayer3D = FindAnyObjectByType<VideoPlayer3D>();
        }
    }

    public void SetVideoPlayer(VideoPlayer3D player)
    {
        videoPlayer3D = player;
    }

    // Called when user starts dragging
    public void OnPointerDown(PointerEventData eventData)
    {
        if (videoPlayer3D != null)
        {
            videoPlayer3D.OnSliderBeginDrag();
        }
    }

    // Called while user is dragging
    public void OnDrag(PointerEventData eventData)
    {
        // Slider value is already updated automatically
        // We just need to keep the dragging state active
    }

    // Called when user releases the slider
    public void OnEndDrag(PointerEventData eventData)
    {
        if (videoPlayer3D != null)
        {
            videoPlayer3D.OnSliderEndDrag();
        }
    }
}

