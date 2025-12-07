using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System.Linq;

public class FetchOwnImage : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get the SpriteRenderer component on this GameObject
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
        imageDownloadUrl += "/200/300";
        StartCoroutine(FetchImage(imageDownloadUrl));

    }
}
