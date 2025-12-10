using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TestProject;

public class PicsumConnector : MonoBehaviour
{
    private const string API_URL = "https://picsum.photos/v2/list?limit=100";

    public static IEnumerator GrabRandomImagesAsync(System.Action<List<PicsumResponse>> onCompleteCallback)
    {
        List<PicsumResponse> results = new List<PicsumResponse>();

        using (UnityWebRequest request = UnityWebRequest.Get(API_URL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                results = DeserializePicsumResponse(jsonResponse);
                Debug.Log($"Successfully fetched {results.Count} images from Picsum API");
            }
            else
            {
                Debug.LogError($"Failed to fetch images from Picsum API: {request.error}");
                Utils.LogErrorMessage("Failed to get list of all images and authors, possible picsum API downtime. Or, yknow, you fat fingered it");
            }
        }

        onCompleteCallback?.Invoke(results);
    }

    public static List<PicsumResponse> GrabRandomImages()
    {
        List<PicsumResponse> results = new List<PicsumResponse>();

        using (UnityWebRequest request = UnityWebRequest.Get(API_URL))
        {
            request.SendWebRequest();

            while (!request.isDone)
            {
                //there has got to be a better way to do this
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                //results = DeserializePicsumResponse(jsonResponse);
                results = DeserializePicsumResponse(jsonResponse);
                Debug.Log($"Successfully fetched {results.Count} images from Picsum API");
                return results;
            }
            else
            {
                Debug.LogError($"Failed to fetch images from Picsum API: {request.error}");
            }
        }
        return null;
    }

    /// <summary>
    /// Deserializes JSON response from Picsum API into a list of PicsumResponse objects
    /// </summary>
    private static List<PicsumResponse> DeserializePicsumResponse(string json)
    {
        List<PicsumResponse> results = new List<PicsumResponse>();

        try
        {
            string wrappedJson = "{\"items\":" + json + "}";

            PicsumResponseList responseList = JsonUtility.FromJson<PicsumResponseList>(wrappedJson);

            results.AddRange(responseList.items);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error deserializing Picsum API response: {e.Message}");
            Debug.LogError($"JSON content: {json.Substring(0, Mathf.Min(200, json.Length))}...");
        }

        return results;
    }
}
    
    