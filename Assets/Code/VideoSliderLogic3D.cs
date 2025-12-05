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

    [Tooltip("Video player we'd like to control")]
    [SerializeField]
    public VideoPlayer videoPlayer;
    private bool isDraggingSlider = false;

    void Start()
    {
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
        if (videoPlayer != null && videoPlayer.isPrepared && isDraggingSlider)
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

    void OnDestroy()
    {
        if (progressSlider != null)
        {
            progressSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}

