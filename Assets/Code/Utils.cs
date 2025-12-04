using UnityEngine;

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
    }
}
