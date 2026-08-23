using UnityEngine;

public class EnemyChest : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private float detectionDistance = 6f;
    [SerializeField] private float losePlayerDistance = 10f;
    [SerializeField] private float walkDistance = 3f;
    [SerializeField] private float runAgainDistance = 4.5f;
    [SerializeField] private float attackDistance = 1.5f;

    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 7f;

    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int attackDamage = 15;

    private Animator animator;
    private PlayerHealth playerHealth;

    private bool playerDetected = false;
    private bool isRunning = false;
    private bool isAttacking = false;

    private float attackTimer;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (player == null || animator == null)
            return;

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (isAttacking)
        {
            TurnTowardsPlayer();

            if (attackTimer <= 0f)
            {
                isAttacking = false;
            }

            return;
        }

        //detekcija playera
        if (!playerDetected && distance <= detectionDistance)
        {
            playerDetected = true;
            isRunning = true;

            animator.SetBool("PlayerDetected", true);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", true);
        }

        if (!playerDetected)
            return;

        //privremeni gubitak playera kada pobjegne dovoljno daleko
        if (distance > losePlayerDistance)
        {
            playerDetected = false;
            isRunning = false;

            animator.SetBool("PlayerDetected", false);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);

            return;
        }

        //napad
        if (distance <= attackDistance)
        {
            isRunning = false;

            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);

            TurnTowardsPlayer();

            if (attackTimer <= 0f)
            {
                StartAttack();
            }

            return;
        }

        if (isRunning)
        {
            if (distance <= walkDistance)
            {
                isRunning = false;
            }
        }
        else
        {
            if (distance > runAgainDistance)
            {
                isRunning = true;
            }
        }

        if (isRunning)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", true);

            MoveTowardsPlayer(runSpeed);
        }
        else
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", false);

            MoveTowardsPlayer(walkSpeed);
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);

        animator.SetTrigger("Attack");

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    private void MoveTowardsPlayer(float speed)
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        direction.Normalize();

        transform.position += direction * speed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            720f * Time.deltaTime
        );
    }

    private void TurnTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            720f * Time.deltaTime
        );
    }
}