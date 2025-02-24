using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;           // 🎯 The player to follow
    public Vector3 offset = new Vector3(0f, 0f, -10f); // 📏 Camera position relative to the player
    public float smoothSpeed = 0.125f; // 🕊️ Smoothness of following

    [Header("Boundary Settings (Optional)")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    void LateUpdate()
    {
        if (target == null) return;

        // 🎬 Desired camera position
        Vector3 desiredPosition = target.position + offset;

        // 🕹️ Apply boundaries if enabled
        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
        }

        // 🎞️ Smoothly move camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
