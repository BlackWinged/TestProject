using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FetchOwnImage : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private string imageUrl = "https://picsum.photos/536/354";

    void Start()
    {
        // Get the SpriteRenderer component on this GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            Debug.LogError("FetchOwnImage: No SpriteRenderer component found on this GameObject!");
            return;
        }

        // Start fetching the image
        StartCoroutine(FetchImage());
    }

    IEnumerator FetchImage()
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
            }
            else
            {
                Debug.LogError($"Failed to fetch image: {request.error}");
            }
        }
    }
}
