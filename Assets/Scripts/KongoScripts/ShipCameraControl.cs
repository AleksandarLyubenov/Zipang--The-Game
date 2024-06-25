using UnityEngine;

public class ShipCameraControl : MonoBehaviour
{
    public Transform shipTransform;
    public float zoomSpeed = 100f;
    public float rotationSpeed = 360f;
    public float minZoom = 50f;
    public float maxZoom = 450f;

    private float currentZoom = 10f;
    private float currentRotationX = 0f;
    private float currentRotationY = 0f;

    void Update()
    {
        // Zoom in/out
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scrollInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Rotate around the ship
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            currentRotationX += rotationX;
            currentRotationY -= rotationY;
            currentRotationY = Mathf.Clamp(currentRotationY, -85f, 85f); // Limiting the vertical rotation
        }

        // Apply the camera transformations
        Vector3 direction = new Vector3(0, 0, -currentZoom);
        Quaternion rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0);
        transform.position = shipTransform.position + rotation * direction;
        transform.LookAt(shipTransform.position);
    }
}
