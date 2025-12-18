using System;
using System.Collections;
using System.Linq;
using TestProject;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class FetchOwnImage : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Smol image x axis")]
    private int thumbnail_size_x = 500;
    [SerializeField]
    [Tooltip("Smol image y axis")]
    private int thumbnail_size_y = 400;
    [SerializeField]
    [Tooltip("Time required to embiggen in seconds")]
    private float EmbiggenTimer = 0.7f;

    private SpriteRenderer spriteRenderer;

    //set to public so I can debug this shit
    public Vector2 mousePosition;
    private BoxCollider2D ownCollider;
    private string fullImageUrl = "";
    private string smallImageUrl = "";
    private bool embigulated = false;
    private Vector3 originalScale;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ownCollider = GetComponent<BoxCollider2D>();
        ownCollider.size = spriteRenderer.size;
        originalScale = transform.localScale;
    }



    IEnumerator FetchImage(string imageUrl, Action callback = null)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Get the downloaded texture
                Texture2D texture = DownloadHandlerTexture.GetContent(request);

                // Create a sprite from the texture
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );


                var scale_x = (float)thumbnail_size_x / texture.width;
                var scale_y = (float)thumbnail_size_y / texture.height;

                spriteRenderer.sprite = sprite;
                spriteRenderer.transform.localScale = new Vector3(
                    originalScale.x * scale_x,
                    originalScale.y * scale_y,
                    originalScale.z
                    );
                var canvii = GetComponentsInChildren<TMP_Text>();
                canvii.Where(x => x.name.Contains("Loading")).ToList().ForEach(x => x.enabled = false);
                if (!embigulated)
                {
                    canvii.Where(x => x.name.Contains("Author")).ToList().ForEach(x => x.enabled = true);
                }
                else
                {
                    canvii.Where(x => x.name.Contains("Author")).ToList().ForEach(x => x.enabled = false);
                }
                    StartCoroutine(ResizeRenderer(callback));
            }
            else
            {
                Utils.LogErrorMessage($"Image with url: {imageUrl} failed download");
                Debug.LogError($"Failed to fetch image: {request.error}");
            }
        }
    }

    public IEnumerator ResizeRenderer(Action callback)
    {
        var timer = 0f;
        var currentScale = transform.localScale;

        while (timer < EmbiggenTimer)
        {
            var progress = timer / EmbiggenTimer;
            transform.localScale = Vector3.Lerp(currentScale, originalScale, progress);

            timer += Time.deltaTime;
            yield return null;
        }


        if (callback != null)
        {
            callback();
        }
    }

    public void SetImageData(PicsumResponse imageData, bool fullSized = false)
    {
        GetComponentsInChildren<TMP_Text>()
            .Where(x => x.name.Contains("Author"))
            .ToList()
            .ForEach(x => x.text = imageData.author);

        //need this for the beeeeg image
        fullImageUrl = imageData.download_url;

        var imageComponents = imageData.download_url.Split("/").ToList();
        imageComponents = imageComponents.Take(imageComponents.Count - 2).ToList();
        var imageDownloadUrl = string.Join("/", imageComponents);
        imageDownloadUrl += $"/{thumbnail_size_x}/{thumbnail_size_y}";
        smallImageUrl = imageDownloadUrl;
        StartCoroutine(FetchImage(imageDownloadUrl));
    }

    bool CheckRaycastClick(Action callback = null)
    {
        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.up, 0.02f);
        if (hit)
        {
            if (hit.collider == ownCollider)
            {
                return true;
            }
        }
        return false;
    }

    public void OnCursorPosition(InputValue value) => mousePosition = value.Get<Vector2>();
    public void OnClickScreen(InputValue value)
    {
        if (CheckRaycastClick())
        {
            StartCoroutine(FetchImage(fullImageUrl));
            transform.position = new Vector3(transform.position.x, transform.position.y, -2);
            embigulated = true;
        }
        else
        {
            if (embigulated)
            {
                StartCoroutine(FetchImage(smallImageUrl, () =>
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                }));
                embigulated = false;
            }
        }
    }
}
