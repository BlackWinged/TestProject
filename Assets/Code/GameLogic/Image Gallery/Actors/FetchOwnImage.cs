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
    [Tooltip("Beeeeg image")]
    private GameObject beeeegImage;

    private SpriteRenderer spriteRenderer;

    //set to public so I can debug this shit
    public Vector2 mousePosition;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    public void SetImageData(PicsumResponse imageData)
    {
        GetComponentsInChildren<TMP_Text>()
            .Where(x => x.name.Contains("Author"))
            .ToList()
            .ForEach(x => x.text = imageData.author);

        var imageComponents = imageData.download_url.Split("/").ToList();
        imageComponents = imageComponents.Take(imageComponents.Count - 2).ToList();
        var imageDownloadUrl = string.Join("/", imageComponents);
        imageDownloadUrl += "/500/400";
        StartCoroutine(FetchImage(imageDownloadUrl));

    }

    //void CheckRaycastClick()
    //{
    //    // Check for left mouse button click
    //    if (mouse.leftButton.wasPressedThisFrame)
    //    {
    //        // Create ray from camera through mouse position
    //        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
    //        RaycastHit hit;

    //        // Check if ray hits this card's collider
    //        if (Physics.Raycast(ray, out hit) && !IsDiscarded)
    //        {
    //            if (hit.collider == cardCollider)
    //            {
    //                Flip();
    //            }
    //        }
    //    }
    //}

    public void OnCursorPosition(InputValue value) => mousePosition = value.Get<Vector2>();
    public void OnClickScreen(InputValue value)
    {
        //CheckRaycastClick();
        var test = value;
    }
}
