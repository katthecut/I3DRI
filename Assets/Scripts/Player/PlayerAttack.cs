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

    [SerializeField] private float defenseDuration = 0.8f;

    private float defenseTimer;
    private bool isDefending;

    private Animator animator;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (timeBetweenAttack > 0f)
        {
            timeBetweenAttack -= Time.deltaTime;
        }

        if (isDefending)
        {
            defenseTimer -= Time.deltaTime;

            if (defenseTimer <= 0f)
            {
                StopDefense();
            }
        }

        //moze opet hodati samo ako nije napad ili defense u tijeku 
        if (!isDefending &&
            timeBetweenAttack <= 0f &&
            playerMovement != null)
        {
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();

            if (playerHealth == null || !playerHealth.IsGettingHit())
            {
                playerMovement.SetCanMove(true);
            }
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        //ne moze napadati dok brani
        if (isDefending)
            return;

        //attack cooldown
        if (timeBetweenAttack > 0f)
            return;

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
            EnemyHealth enemyHealth =
                enemyCollider.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    public void Defense(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        //ne moze poceti defense dok napada
        if (isDefending)
            return;

        //prekida trenutni attack
        timeBetweenAttack = 0f;

        animator.ResetTrigger("Attack");

        isDefending = true;
        defenseTimer = defenseDuration;

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        animator.SetTrigger("Defend");
    }

    private void StopDefense()
    {
        isDefending = false;

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
        }
    }

    public bool IsDefending()
    {
        return isDefending;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRange
        );
    }
}