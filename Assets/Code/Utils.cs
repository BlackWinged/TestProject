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

        public static IEnumerable<GameObject> PopulateGrid(GameObject prefab, int objectCount, Bounds bounds, int colNumber, float ySpacing = 0.6f, float jitter = 0f)
        {
            var randy = new System.Random((int)Time.time);
            // Calculate spacing
            float xSpacing = bounds.size.x / (colNumber + 1);
            //float ySpacing = bounds.size.y / (rows + 1);

            // Starting position (bottom-left of bounds)
            Vector3 startPos = bounds.min + new Vector3(xSpacing, ySpacing, 0);
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
