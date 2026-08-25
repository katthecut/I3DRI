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

    [SerializeField] private AudioClip attackSound;

    private bool isDefending;

    private Animator animator;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (timeBetweenAttack > 0f)
        {
            timeBetweenAttack -= Time.deltaTime;
        }

        //moze opet hodati samo ako nije napad ili defense u tijeku 
        if (!isDefending &&
            timeBetweenAttack <= 0f &&
            playerMovement != null)
        {
            if (playerHealth == null ||
                (!playerHealth.IsGettingHit() && !playerHealth.IsDead))
            {
                playerMovement.SetCanMove(true);
            }
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (playerHealth != null && playerHealth.IsDead)
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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(attackSound);
        }

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
        if (playerHealth != null && playerHealth.IsDead)
            return;

        //RMB pritisnut
        if (context.started)
        {
            StartDefense();
        }

        //RMB pusten
        if (context.canceled)
        {
            StopDefense();
        }
    }

    private void StartDefense()
    {
        if (isDefending)
            return;

        //prekida trenutni attack cooldown
        timeBetweenAttack = 0f;

        isDefending = true;

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        animator.ResetTrigger("Attack");
        animator.SetBool("IsDefending", true);
    }

    private void StopDefense()
    {
        if (!isDefending)
            return;

        isDefending = false;

        animator.SetBool("IsDefending", false);

        if (playerMovement != null &&
            (playerHealth == null || !playerHealth.IsDead))
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