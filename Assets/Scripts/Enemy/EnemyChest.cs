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

    [SerializeField] private AudioClip attackSound;
    [SerializeField] private float attackVolume = 1.3f;

    [SerializeField] private AudioClip walkSound;
    [SerializeField] private float walkVolume = 0.7f;

    [SerializeField] private AudioClip openSound;
    [SerializeField] private float openVolume = 1f;

    private Animator animator;
    private PlayerHealth playerHealth;

    private bool playerDetected = false;
    private bool isRunning = false;
    private bool isAttacking = false;

    private float attackTimer;
    private bool wasWalking;

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

        if (playerHealth != null && playerHealth.IsDead)
        {
            animator.SetBool("PlayerDetected", false);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);

            isAttacking = false;
            isRunning = false;
            wasWalking = false;

            return;
        }

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

        if (distance > losePlayerDistance)
        {
            playerDetected = false;
            isRunning = false;

            animator.SetBool("PlayerDetected", false);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);

            wasWalking = false;

            return;
        }

        if (distance <= attackDistance)
        {
            isRunning = false;

            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);

            wasWalking = false;

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

            if (!wasWalking)
            {
                PlayWalkSound();
            }

            MoveTowardsPlayer(runSpeed);
        }
        else
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", false);

            if (!wasWalking)
            {
                PlayWalkSound();
            }

            MoveTowardsPlayer(walkSpeed);
        }

        wasWalking = true;
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);

        animator.SetTrigger("Attack");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(
                attackSound,
                attackVolume
            );
        }

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

    private void PlayWalkSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(
                walkSound,
                walkVolume
            );
        }
    }

    public void PlayOpenSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(
                openSound,
                openVolume
            );
        }
    }
}