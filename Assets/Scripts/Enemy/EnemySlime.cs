using UnityEngine;

public class EnemySlime : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private float detectionDistance = 6f;
    [SerializeField] private float losePlayerDistance = 10f;
    [SerializeField] private float attackDistance = 1.5f;

    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDamage = 10f;

    [SerializeField] private AudioClip movementSound;
    [SerializeField] private float movementVolume = 1f;

    [SerializeField] private AudioClip attackSound;
    [SerializeField] private float attackVolume = 1.3f;

    private Animator animator;
    private EnemyHealth enemyHealth;

    private bool playerDetected;
    private float attackTimer;
    private float movementSoundTimer;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        if (player == null || animator == null)
            return;

        if (enemyHealth != null && enemyHealth.Health <= 0f)
            return;

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (!playerDetected)
        {
            if (distance <= detectionDistance)
            {
                playerDetected = true;

                animator.SetBool("PlayerDetected", true);
            }
            else
            {
                animator.SetBool("PlayerDetected", false);
                animator.SetBool("IsWalking", false);

                animator.SetFloat("MoveX", 0f);
                animator.SetFloat("MoveY", 0f);

                return;
            }
        }

        if (distance > losePlayerDistance)
        {
            playerDetected = false;

            animator.SetBool("PlayerDetected", false);
            animator.SetBool("IsWalking", false);

            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", 0f);

            movementSoundTimer = 0f;

            return;
        }

        if (distance <= attackDistance)
        {
            animator.SetBool("IsWalking", false);

            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", 0f);

            movementSoundTimer = 0f;

            TurnTowardsPlayer();

            if (attackTimer <= 0f)
            {
                Attack();
            }

            return;
        }

        animator.SetBool("IsWalking", true);

        if (movementSoundTimer <= 0f)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(
                    movementSound,
                    movementVolume
                );
            }

            movementSoundTimer = 0.8f;
        }
        else
        {
            movementSoundTimer -= Time.deltaTime;
        }

        MoveTowardsPlayer(direction);
    }

    private void MoveTowardsPlayer(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.001f)
            return;

        direction.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            720f * Time.deltaTime
        );

        transform.position += direction * moveSpeed * Time.deltaTime;

        Vector3 localDirection = transform.InverseTransformDirection(direction);

        animator.SetFloat("MoveX", localDirection.x);
        animator.SetFloat("MoveY", localDirection.z);
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

    private void Attack()
    {
        attackTimer = attackCooldown;

        animator.SetTrigger("Attack");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(
                attackSound,
                attackVolume
            );
        }

        PlayerHealth playerHealth =
            player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }
}