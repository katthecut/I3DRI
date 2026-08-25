using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float distance = 8f;
    [SerializeField] private float height = 1.5f;

    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private float zoomSpeed = 1f;

    [SerializeField] private float minCameraHeight = 0.5f;

    [SerializeField] private float sensitivity = 0.03f;
    [SerializeField] private float minVerticalRotation = -20f;
    [SerializeField] private float maxVerticalRotation = 45f;

    private float horizontalRotation;
    private float verticalRotation;

    private void Start()
    {
        if (target == null)
            return;

        Vector3 rotation = transform.eulerAngles;

        horizontalRotation = rotation.y;
        verticalRotation = rotation.x;

        if (verticalRotation > 180f)
            verticalRotation -= 360f;

        verticalRotation = Mathf.Clamp(
            verticalRotation,
            minVerticalRotation,
            maxVerticalRotation
        );

        distance = Mathf.Clamp(
            distance,
            6f,
            maxDistance
        );

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            horizontalRotation += mouseDelta.x * sensitivity;
            verticalRotation -= mouseDelta.y * sensitivity;

            verticalRotation = Mathf.Clamp(
                verticalRotation,
                minVerticalRotation,
                maxVerticalRotation
            );

            float scroll = Mouse.current.scroll.ReadValue().y;

            if (scroll < 0f)
            {
                distance += zoomSpeed;
            }

            distance = Mathf.Clamp(
                distance,
                6f,
                maxDistance
            );
        }

        Quaternion rotation = Quaternion.Euler(
            verticalRotation,
            horizontalRotation,
            0f
        );

        Vector3 targetPosition =
            target.position + Vector3.up * height;

        Vector3 cameraPosition =
            targetPosition +
            rotation * Vector3.back * distance;

        //kamera ne smije ici ispod zadane visine
        if (cameraPosition.y < minCameraHeight)
        {
            cameraPosition.y = minCameraHeight;
        }

        transform.position = cameraPosition;

        transform.LookAt(targetPosition);
    }
}