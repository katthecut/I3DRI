using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;

    public float moveSpeed = 6f;
    public float rotationSpeed = 720f;

    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float firstFootstepDelay = 0.25f;
    [SerializeField] private float footstepInterval = 0.4f;

    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private float jumpVolume = 0.7f;

    [SerializeField] private AudioClip walkSound;
    [SerializeField] private float walkVolume = 1f;

    private Vector2 moveInput;
    private Vector3 movement;

    private bool canMove = true;
    private bool isGrounded;
    private bool wasMoving;

    private float footstepTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("PlayerMovement requires a Rigidbody.");
            return;
        }

        rb.useGravity = true;

        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        CheckGrounded();

        if (!canMove)
        {
            movement = Vector3.zero;
            wasMoving = false;
            footstepTimer = 0f;

            if (animator != null)
            {
                animator.SetBool("Walking", false);
            }

            return;
        }

        Camera kamera = Camera.main;

        if (kamera == null)
            return;

        Vector3 smjerNaprijed = kamera.transform.forward;
        Vector3 smjerDesno = kamera.transform.right;

        smjerNaprijed.y = 0f;
        smjerDesno.y = 0f;

        smjerNaprijed.Normalize();
        smjerDesno.Normalize();

        movement =
            smjerNaprijed * moveInput.y +
            smjerDesno * moveInput.x;

        if (movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        bool isMoving = movement.sqrMagnitude > 0.01f;

        if (animator != null)
        {
            animator.SetBool("Walking", isMoving);
        }

        HandleFootsteps(isMoving);
    }

    private void HandleFootsteps(bool isMoving)
    {
        if (!isMoving || !isGrounded)
        {
            wasMoving = false;
            footstepTimer = 0f;
            return;
        }

        if (!wasMoving)
        {
            wasMoving = true;
            footstepTimer = firstFootstepDelay;
            return;
        }

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(
                    walkSound,
                    walkVolume
                );
            }

            footstepTimer = footstepInterval;
        }
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        Vector3 horizontalVelocity = movement * moveSpeed;

        rb.linearVelocity = new Vector3(
            horizontalVelocity.x,
            rb.linearVelocity.y,
            horizontalVelocity.z
        );

        if (movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            Quaternion newRotation = Quaternion.RotateTowards(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

            rb.MoveRotation(newRotation);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (rb == null)
            return;

        if (!isGrounded)
            return;

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            jumpForce,
            rb.linearVelocity.z
        );

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(
                jumpSound,
                jumpVolume
            );
        }
    }

    private void CheckGrounded()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    public void SetCanMove(bool value)
    {
        canMove = value;

        if (!canMove)
        {
            movement = Vector3.zero;
            wasMoving = false;
            footstepTimer = 0f;

            if (rb != null)
            {
                rb.linearVelocity = new Vector3(
                    0f,
                    rb.linearVelocity.y,
                    rb.linearVelocity.z
                );
            }

            if (animator != null)
            {
                animator.SetBool("Walking", false);
            }
        }
    }

    public bool CanMove()
    {
        return canMove;
    }

    public void Test()
    {
        Debug.Log("Test works");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}