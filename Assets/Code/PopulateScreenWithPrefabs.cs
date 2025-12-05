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
        Bounds visibleBounds = GetVisibleWorldBounds();

        if (useGridLayout)
        {
            PopulateGrid(visibleBounds);
        }
        else
        {
            PopulateRandom(visibleBounds);
        }
    }

    private Bounds GetVisibleWorldBounds()
    {
        if (targetCamera.orthographic)
        {
            // Orthographic camera
            float height = targetCamera.orthographicSize * 2f;
            float width = height * targetCamera.aspect;
            
            Vector3 center = targetCamera.transform.position;
            center.z = zPosition;
            
            return new Bounds(center, new Vector3(width - padding * 2, height - padding * 2, 0));
        }
        else
        {
            // Perspective camera - calculate bounds at zPosition
            float distance = Mathf.Abs(targetCamera.transform.position.z - zPosition);
            float height = 2.0f * distance * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float width = height * targetCamera.aspect;
            
            Vector3 center = targetCamera.transform.position + targetCamera.transform.forward * distance;
            center.z = zPosition;
            
            return new Bounds(center, new Vector3(width - padding * 2, height - padding * 2, 0));
        }
    }

    private void PopulateGrid(Bounds bounds)
    {
        // Calculate grid dimensions
        int cols = Mathf.CeilToInt(gridSize.x);
        int rows = Mathf.CeilToInt(gridSize.y);
        
        // Adjust if numberOfInstances is set
        if (numberOfInstances > 0)
        {
            cols = Mathf.CeilToInt(Mathf.Sqrt(numberOfInstances * (bounds.size.x / bounds.size.y)));
            rows = Mathf.CeilToInt((float)numberOfInstances / cols);
        }

        // Calculate spacing
        float xSpacing = bounds.size.x / (cols + 1);
        float ySpacing = bounds.size.y / (rows + 1);

        // Starting position (bottom-left of bounds)
        Vector3 startPos = bounds.min + new Vector3(xSpacing, ySpacing, 0);

        int instanceCount = 0;
        for (int row = 0; row < rows && instanceCount < numberOfInstances; row++)
        {
            for (int col = 0; col < cols && instanceCount < numberOfInstances; col++)
            {
                Vector3 position = startPos + new Vector3(col * xSpacing, row * ySpacing, zPosition);
                InstantiatePrefab(position);
                instanceCount++;
            }
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

