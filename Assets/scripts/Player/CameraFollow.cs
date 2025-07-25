using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private float fixedY; // Stores the initial Y position

    void Start()
    {
        // Lock the camera's Y position at start
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Only follow player in X direction
        Vector3 desiredPosition = new Vector3(player.position.x + offset.x, fixedY, transform.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}

