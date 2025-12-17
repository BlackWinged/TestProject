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
    [Tooltip("Beeeeg image x axis")]
    private int thumbnail_size_x = 500;
    [SerializeField]
    [Tooltip("Beeeeg image y axis")]
    private int thumbnail_size_y = 400;

    private SpriteRenderer spriteRenderer;

    //set to public so I can debug this shit
    public Vector2 mousePosition;
    private BoxCollider2D ownCollider;
    private string fullImageUrl = "";
    private string smallImageUrl = "";
    private bool embigulated = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ownCollider = GetComponent<BoxCollider2D>();
        ownCollider.size = spriteRenderer.size;
    }



    IEnumerator FetchImage(string imageUrl)
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

                // Assign the sprite to the SpriteRenderer component
                spriteRenderer.sprite = sprite;
                spriteRenderer.size = new Vector2(thumbnail_size_x, thumbnail_size_y);
                var canvii = GetComponentsInChildren<TMP_Text>();
                canvii.Where(x => x.name.Contains("Loading")).ToList().ForEach(x => x.enabled = false);
                canvii.Where(x => x.name.Contains("Author")).ToList().ForEach(x => x.enabled = true);
            }
            else
            {
                Utils.LogErrorMessage($"Image with url: {imageUrl} failed download");
                Debug.LogError($"Failed to fetch image: {request.error}");
            }
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

    bool CheckRaycastClick()
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
        } else
        {
            if (embigulated)
            {
                StartCoroutine(FetchImage(smallImageUrl));
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                embigulated = false;
            }
        }
    }
}
