using TestProject;
using UnityEngine;

public class PopulateScreenWithPrefabs : MonoBehaviour
{
    [Header("Prefab Settings")]
    [SerializeField] private GameObject prefabToInstantiate;
    [SerializeField] private string prefabPath = "SpritePrefabs/Fetched Image";
    
    [Header("Population Settings")]
    [SerializeField] private int numberOfInstances = 20;
    [SerializeField] private bool useGridLayout = true;
    [SerializeField] private Vector2 gridSize = new Vector2(5, 4);
    [SerializeField] private float spacing = 1.5f;
    [SerializeField] private bool randomizePositions = false;
    
    [Header("Camera Settings")]
    [SerializeField] private float zPosition = 0f;
    [SerializeField] private float padding = 0.5f;

    private Camera targetCamera;

    private void Start()
    {
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

        if (useGridLayout)
        {
            Utils.PopulateGrid(prefabToInstantiate, numberOfInstances, visibleBounds, (int)gridSize.x, spacing);
        }
        else
        {
            PopulateRandom(visibleBounds);
        }
    }

   

    private void PopulateRandom(Bounds bounds)
    {
        for (int i = 0; i < numberOfInstances; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                zPosition
            );
            InstantiatePrefab(randomPosition);
        }
    }

    private void InstantiatePrefab(Vector3 position)
    {
        GameObject instance = Instantiate(prefabToInstantiate, position, Quaternion.identity);
        instance.transform.SetParent(transform); // Parent to this GameObject for organization
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

