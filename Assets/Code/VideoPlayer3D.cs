using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VideoSliderLogic3D : MonoBehaviour
{
    [Header("Progress Tracking (UI)")]
    [Tooltip("UI Slider for video progress tracking and seeking")]
    [SerializeField] private Slider progressSlider;
    
    [Tooltip("Optional: Text to display current time (e.g., '1:23 / 5:45')")]
    [SerializeField] private TextMeshProUGUI timeText;

    private VideoPlayer videoPlayer;
    private Material videoMaterial;
    private bool isDraggingSlider = false;

    void Start()
    {
        SetupVideoPlayer();
        SetupProgressSlider();
    }

    void SetupProgressSlider()
    {
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
            
            // Add listener for when user drags the slider
            progressSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    void SetupVideoPlayer()
    {
        // Get or create VideoPlayer component
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }

        // Get target renderer if not assigned
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
            if (targetRenderer == null)
            {
                Debug.LogError("VideoPlayer3D: No Renderer component found! Please assign a targetRenderer.");
                return;
            }
        }

        // Configure VideoPlayer
        videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
        videoPlayer.targetMaterialRenderer = targetRenderer;
        videoPlayer.targetMaterialProperty = texturePropertyName;
        videoPlayer.isLooping = loop;
        videoPlayer.playOnAwake = false;

        // Set video source
        if (videoClip != null)
        {
            videoPlayer.clip = videoClip;
        }
        else if (!string.IsNullOrEmpty(videoURL))
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = videoURL;
        }
        else
        {
            Debug.LogWarning("VideoPlayer3D: No video clip or URL assigned!");
        }
    }

    void Update()
    {
        // Update progress slider based on video time
        if (videoPlayer != null && videoPlayer.isPrepared && !isDraggingSlider)
        {
            double currentTime = videoPlayer.time;
            double totalTime = GetVideoLength();
            
            if (totalTime > 0)
            {
                // Update slider value (0-1 range)
                if (progressSlider != null)
                {
                    progressSlider.value = (float)(currentTime / totalTime);
                }
                
                // Update time text if assigned
                UpdateTimeText(currentTime, totalTime);
            }
        }
    }

    double GetVideoLength()
    {
        if (videoPlayer == null) return 0;
        
        // Try to get length from clip first
        if (videoPlayer.clip != null)
        {
            return videoPlayer.clip.length;
        }
        
        // For URL-based videos, try to get frame count and frame rate
        if (videoPlayer.frameCount > 0 && videoPlayer.frameRate > 0)
        {
            return videoPlayer.frameCount / videoPlayer.frameRate;
        }
        
        return 0;
    }

    void OnSliderValueChanged(float value)
    {
        // This is called when user drags the slider
        if (videoPlayer != null && videoPlayer.isPrepared)
        {
            double totalTime = GetVideoLength();
            if (totalTime > 0)
            {
                // Seek to the position based on slider value
                double targetTime = value * totalTime;
                videoPlayer.time = targetTime;
            }
        }
    }

    void UpdateTimeText(double currentTime, double totalTime)
    {
        string timeString = $"{FormatTime(currentTime)} / {FormatTime(totalTime)}";
        
        if (timeText != null)
        {
            timeText.text = timeString;
        }
        
        if (timeTextLegacy != null)
        {
            timeTextLegacy.text = timeString;
        }
    }

    string FormatTime(double timeInSeconds)
    {
        int minutes = Mathf.FloorToInt((float)timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt((float)timeInSeconds % 60f);
        return $"{minutes}:{seconds:D2}";
    }

    // Public method to set slider dragging state (call from UI events)
    public void OnSliderBeginDrag()
    {
        isDraggingSlider = true;
    }

    public void OnSliderEndDrag()
    {
        isDraggingSlider = false;
    }

    // Public methods to control playback
    public void Play()
    {
        if (videoPlayer != null)
        {
            if (!videoPlayer.isPrepared)
            {
                videoPlayer.Prepare();
                videoPlayer.prepareCompleted += OnVideoPrepared;
            }
            else
            {
                videoPlayer.Play();
            }
        }
    }

    public void Pause()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Pause();
        }
    }

    public void Stop()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
    }

    public void SetVideoURL(string url)
    {
        if (videoPlayer != null)
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = url;
            videoURL = url;
        }
    }

    public void SetVideoClip(VideoClip clip)
    {
        if (videoPlayer != null)
        {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = clip;
            videoClip = clip;
        }
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
        
        if (progressSlider != null)
        {
            progressSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}

