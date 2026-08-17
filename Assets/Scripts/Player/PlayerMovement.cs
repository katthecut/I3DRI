using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;

    public float moveSpeed = 5f;
    float horizontalMove;
    float verticalMove;

    private Vector3 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        movement = new Vector3(horizontalMove, 0f, verticalMove);

        if (movement.magnitude > 1f)
            movement.Normalize();

        bool isMoving = movement.magnitude > 0.01f;

        if (animator != null)
        {
            animator.SetBool("Walking", isMoving);
        }
    }

    void FixedUpdate()
    {
        Vector3 appliedMovement = movement * moveSpeed;

        rb.linearVelocity = new Vector3(
            appliedMovement.x,
            rb.linearVelocity.y,
            appliedMovement.z
        );

        if (movement.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.MoveRotation(Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                10f * Time.fixedDeltaTime
            ));
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        horizontalMove = input.x;
        verticalMove = input.y;
    }
}