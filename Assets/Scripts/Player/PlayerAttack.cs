using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private float timeBetweenAttack;
    public float startTimeBetweenAttack = 0.5f;

    public int damage = 10;

    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayer;

    private Animator animator;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (timeBetweenAttack > 0)
        {
            timeBetweenAttack -= Time.deltaTime;
        }

        if (timeBetweenAttack <= 0f && playerMovement != null)
        {
            playerMovement.SetCanMove(true);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (timeBetweenAttack > 0f) return;

        timeBetweenAttack = startTimeBetweenAttack;

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        animator.SetTrigger("Attack");

        Collider[] enemiesHit = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider enemyCollider in enemiesHit)
        {
            EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}