using UnityEngine;

public class EnemyChest : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private float detectionDistance = 6f;
    [SerializeField] private float walkDistance = 4f;
    [SerializeField] private float attackDistance = 1.5f;

    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;

    //brzina okretanja prema playeru
    [SerializeField] private float rotationSpeed = 360f;

    private Animator animator;
    private Rigidbody rb;

    private bool playerDetected = false;

    private float currentSpeed = 0f;
    private Vector3 movementDirection = Vector3.zero;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (player == null || animator == null || rb == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        //detekcija playera
        if (!playerDetected && distance <= detectionDistance)
        {
            playerDetected = true;
            animator.SetBool("PlayerDetected", true);
        }

        if (!playerDetected)
        {
            currentSpeed = 0f;
            movementDirection = Vector3.zero;

            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);

            return;
        }

        //napad
        if (distance <= attackDistance)
        {
            currentSpeed = 0f;
            movementDirection = Vector3.zero;

            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);

            animator.SetTrigger("Attack");

            return;
        }

        //hodanje
        if (distance <= walkDistance)
        {
            currentSpeed = walkSpeed;

            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", false);
        }
        //trcanje
        else
        {
            currentSpeed = runSpeed;

            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", true);
        }

        movementDirection = player.position - transform.position;

        movementDirection.y = 0f;

        if (movementDirection.sqrMagnitude > 0.001f)
        {
            movementDirection.Normalize();
        }
        else
        {
            movementDirection = Vector3.zero;
            currentSpeed = 0f;
        }

        animator.SetFloat("MoveX", movementDirection.x);
        animator.SetFloat("MoveY", movementDirection.z);
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        if (movementDirection == Vector3.zero || currentSpeed <= 0f)
            return;

        //okretanje prema playeru
        RotateTowardsPlayer();

        //kretanje prema playeru
        Vector3 nextPosition =
            rb.position +
            movementDirection * currentSpeed * Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }

    private void RotateTowardsPlayer()
    {
        Vector3 smjer = player.position - transform.position;

        smjer.y = 0f;

        if (smjer.sqrMagnitude <= 0.001f)
            return;

        Quaternion zeljenaRotacija = Quaternion.LookRotation(smjer);

        Quaternion novaRotacija = Quaternion.RotateTowards(
            rb.rotation,
            zeljenaRotacija,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(novaRotacija);
    }
}