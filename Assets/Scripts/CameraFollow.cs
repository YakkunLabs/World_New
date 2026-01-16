using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The Player to follow

    public float mouseSensitivity = 5.0f;
    public float distanceFromPlayer = 5.0f;
    public Vector3 offset = new Vector3(0, 2f, 0); // Offset to look at head, not feet
    public Vector2 pitchLimits = new Vector2(-40, 85); // Limit up/down looking

    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Get Mouse Input
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 2. Clamp the up/down rotation so you can't flip over
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        // 3. Calculate Rotation and Position
        Vector3 targetRotation = new Vector3(pitch, yaw);
        transform.eulerAngles = targetRotation;

        // 4. Move Camera back from the target
        // (Target Position + Offset) - (Camera Direction * Distance)
        transform.position = (target.position + offset) - (transform.forward * distanceFromPlayer);
    }
}