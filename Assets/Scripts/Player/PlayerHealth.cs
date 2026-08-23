using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    //health
    private float health;
    [SerializeField] private float maxHealth = 100f;

    //ui
    [SerializeField] private HealthBarSystem healthBar;

    [SerializeField] private float getHitDuration = 0.5f;

    private bool isDead;
    private float getHitTimer;

    private Animator animator;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    public float Health => health;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        health = maxHealth;

        animator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();

        SyncUI();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = MaxHealth;
        SyncUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (getHitTimer > 0f)
        {
            getHitTimer -= Time.deltaTime;

            if (getHitTimer <= 0f && !isDead)
            {
                if (playerMovement != null)
                {
                    playerMovement.SetCanMove(true);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(20f);
        }
    }
    private void SyncUI()
    {
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(health);
        }
        else
        {
            Debug.LogWarning("PlayerHealth: HealthBarSystem missing.");
        }
    }

    public void SetHealth(float value)
    {
        if (isDead) return;

        health = Mathf.Clamp(value, 0f, maxHealth);

        SyncUI();

        if (health <= 0f)
            Die();
    }

    public void AddHealth(float amount)
    {
        SetHealth(health + amount);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (damage <= 0f) return;

        //ne prima damage dok brani
        if (playerAttack != null && playerAttack.IsDefending())
            return;

        AddHealth(-damage);

        if (isDead)
            return;

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        getHitTimer = getHitDuration;

        if (animator != null)
        {
            animator.SetTrigger("GetHit");
        }
    }

    public bool IsGettingHit()
    {
        return getHitTimer > 0f;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }
}