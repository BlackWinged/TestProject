using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TestProject
{
    public class Utils
    {
        public static Vector3 GetWorldPoint(Vector2 screenPoint)
        {
            var cam = Camera.main;

            Vector3 worldPoint = screenPoint;

            // For orthographic cameras, use the camera's Z position
            worldPoint.z = cam.nearClipPlane;
            return cam.ScreenToWorldPoint(worldPoint);
        }

        public static Bounds GetVisibleWorldBounds(Camera targetCamera, float padding = 0.5f)
        {
            if (targetCamera.orthographic)
            {
                // Orthographic camera
                float height = targetCamera.orthographicSize * 2f;
                float width = height * targetCamera.aspect;

                Vector3 center = targetCamera.transform.position;

                return new Bounds(center, new Vector3(width - padding * 2, height - padding * 2, 0));
            }
            else
            {
                // Perspective camera - calculate bounds at zPosition
                float distance = Mathf.Abs(targetCamera.transform.position.z);
                float height = 2.0f * distance * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                float width = height * targetCamera.aspect;

                Vector3 center = targetCamera.transform.position + targetCamera.transform.forward * distance;

                return new Bounds(center, new Vector3(width - padding * 2, height - padding * 2, 0));
            }
        }

        /// <summary>
        /// Instantiates and arranges a specified number of prefab GameObjects in a grid layout within the given bounds.
        /// </summary>
        /// <remarks>The method arranges objects in rows and columns, starting from the lower-left corner
        /// of the specified bounds. If the total object count does not evenly divide by the column count, the last row
        /// may contain fewer objects. Jitter introduces random variation to object positions, which can be used to
        /// avoid perfectly aligned grids. All instantiated objects are placed at z = 0.</remarks>
        /// <param name="prefab">The GameObject prefab to instantiate for each grid cell. Cannot be null.</param>
        /// <param name="objectCount">The total number of GameObjects to create and place in the grid.</param>
        /// <param name="bounds">The bounds within which the grid will be positioned. Determines the spatial limits for placement.</param>
        /// <param name="colNumber">The number of columns in the grid. Must be greater than zero.</param>
        /// <param name="ySpacing">The vertical spacing between rows in the grid, in world units. Defaults to 0.6.</param>
        /// <param name="jitter">The maximum random offset applied to the position of each GameObject, in world units. Set to 0 for no
        /// jitter.</param>
        /// <param name="initialYspaceOffset">The initial vertical offset added to the starting position of the grid, in world units. Defaults to 0.</param>
        /// <returns>An enumerable collection of the instantiated GameObjects, each positioned according to the grid layout.</returns>

        public static IEnumerable<GameObject> PopulateGrid(GameObject prefab, int objectCount, Bounds bounds, int colNumber, float ySpacing = 0.6f, float jitter = 0f, float initialYspaceOffset = 0f)
        {
            var randy = new System.Random((int)Time.time);
            float xSpacing = bounds.size.x / (colNumber + 1);
            //float ySpacing = bounds.size.y / (rows + 1);

            // Starting position (bottom-left of bounds)

            Vector3 startPos = bounds.min + new Vector3(xSpacing, ySpacing + initialYspaceOffset, 0);
            startPos.z = 0;

            for (int row = 0; row < Mathf.Ceil(objectCount / colNumber); row++)
            {
                for (int col = 0; col < colNumber; col++)
                {
                    var newJitter = randy.Next(-1, 1) * jitter;

                    Vector3 position = startPos + new Vector3(col * (xSpacing + newJitter) , row * (-ySpacing + newJitter), 0);
                    GameObject instance = Object.Instantiate(prefab, position, Quaternion.identity);
                    yield return instance;
                }
            }
        }

        public static void LogErrorMessage(string message)
        {
            var eh = GameObject.FindGameObjectWithTag(TagEnums.ErrorHandler);
            if (eh == null) return;

            var ehl = eh.GetComponent<ErrorHandlingLogic>();
            ehl.AddErrorMessage(message);
        }
    }
}
