using System.Collections.Generic;
using System.Linq;
using TestProject;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PopulateScreenWithPrefabs : MonoBehaviour
{
    [Header("Prefab Settings")]
    [SerializeField] private GameObject prefabToInstantiate;
    [SerializeField] private string prefabPath = "SpritePrefabs/Fetched Image";

    [Header("Population Settings")]
    [SerializeField] private int numberOfInstances = 20;
    [SerializeField] private bool useGridLayout = true;
    [SerializeField] private Vector2 gridSize = new Vector2(5, 4);
    [SerializeField] private float positionJitter = 0.5f;

    [Header("Camera Settings")]
    [SerializeField] private float zPosition = 0f;
    [SerializeField] private float padding = 0.5f;

    private Camera targetCamera;


    private void Start()
    {
        Utils.LogErrorMessage("test error message");
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        // Load prefab if not assigned
        if (prefabToInstantiate == null)
        {
            prefabToInstantiate = Resources.Load<GameObject>(prefabPath);
            if (prefabToInstantiate == null)
            {
                Debug.LogError($"Could not load prefab from path: {prefabPath}. Please assign it manually in the Inspector.");
                return;
            }
        }

        // Populate the screen
        PopulateScreen();
    }

    public void PopulateScreen()
    {
        if (prefabToInstantiate == null || targetCamera == null)
        {
            Debug.LogError("Prefab or Camera is not assigned!");
            return;
        }



        // Calculate visible world bounds
        Bounds visibleBounds = Utils.GetVisibleWorldBounds(targetCamera, padding);

        var result = new List<GameObject>();
        if (useGridLayout)
        {
            result.AddRange(Utils.PopulateGrid(prefabToInstantiate, numberOfInstances, visibleBounds, (int)gridSize.x, gridSize.y, positionJitter));
        }
        else
        {
            result = PopulateRandom(visibleBounds);
        }
        Debug.Log("Fetching images");
        List<PicsumResponse> imageData = new List<PicsumResponse>();

        StartCoroutine(PicsumConnector.GrabRandomImagesAsync((data) =>
        {
            imageData = data;
            Debug.Log("Finished fetching images");
            for (var i = 0; i < result.Count; i++)
            {
                var imageObject = result[i].GetComponent<FetchOwnImage>();
                //don't like this want random
                //var imageAndAuthor = imageData[i];
                var imageAndAuthor = imageData[Random.Range(0, imageData.Count() - 1)];
                if (imageObject == null)
                {
                    Debug.LogError("Prefab does not have a FetchAndSetImage component!");
                    continue;
                }
                imageObject.SetImageData(imageAndAuthor);
            }
        }));
    }


    private List<GameObject> PopulateRandom(Bounds bounds)
    {
        var result = new List<GameObject>();
        for (int i = 0; i < numberOfInstances; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                zPosition
            );
            GameObject instance = Instantiate(prefabToInstantiate, randomPosition, Quaternion.identity);
            instance.transform.SetParent(transform); // Parent to this GameObject for organization
            result.Add(instance);
        }

        return result;
    }


    // Method to clear all instantiated prefabs
    public void ClearInstances()
    {
        // Destroy all children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }

    // Method to repopulate (clears and repopulates)
    public void Repopulate()
    {
        ClearInstances();
        PopulateScreen();
    }
}

