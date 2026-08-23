using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;

    //udaljenost kamere i visina
    [SerializeField] private float distance = 6f;
    [SerializeField] private float height = 1.5f;

    //brzina okretanja kamere
    [SerializeField] private float sensitivity = 0.03f;

    //ogranicenje gledanja gore i dolje
    [SerializeField] private float minVerticalRotation = -20f;
    [SerializeField] private float maxVerticalRotation = 45f;

    private float horizontalRotation;
    private float verticalRotation;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraController: Player nije postavljen kao Target.");
            return;
        }

        Vector3 trenutnaRotacija = transform.eulerAngles;

        horizontalRotation = trenutnaRotacija.y;
        verticalRotation = trenutnaRotacija.x;

        if (verticalRotation > 180f)
            verticalRotation -= 360f;

        verticalRotation = Mathf.Clamp(
            verticalRotation,
            minVerticalRotation,
            maxVerticalRotation
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
            Vector2 pomakMisa = Mouse.current.delta.ReadValue();

            horizontalRotation += pomakMisa.x * sensitivity;
            verticalRotation -= pomakMisa.y * sensitivity;

            verticalRotation = Mathf.Clamp(
                verticalRotation,
                minVerticalRotation,
                maxVerticalRotation
            );
        }

        Quaternion rotacijaKamere = Quaternion.Euler(
            verticalRotation,
            horizontalRotation,
            0f
        );

        Vector3 pozicijaCilja =
            target.position + Vector3.up * height;

        Vector3 pozicijaKamere =
            pozicijaCilja +
            rotacijaKamere * Vector3.back * distance;

        transform.position = pozicijaKamere;

        transform.LookAt(pozicijaCilja);
    }
}