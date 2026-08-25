using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private float health;

    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private HealthBarSystem healthBar;

    [SerializeField] private float getHitDuration = 0.5f;
    [SerializeField] private float invincibilityAfterHit = 0.75f;

    [SerializeField] private AudioClip hitSound;
    [SerializeField] private float hitVolume = 0.7f;

    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float deathVolume = 1f;
    [SerializeField] private float deathSoundDelay = 0.4f;

    private bool isDead;
    private float getHitTimer;
    private float invincibilityTimer;

    private Animator animator;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    public float Health => health;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        health = maxHealth;

        animator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();

        SyncUI();
    }

    private void Start()
    {
        health = MaxHealth;
        SyncUI();
    }

    private void Update()
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

        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
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
        if (isDead)
            return;

        health = Mathf.Clamp(value, 0f, maxHealth);

        SyncUI();

        if (health <= 0f)
        {
            Die();
        }
    }

    public void AddHealth(float amount)
    {
        SetHealth(health + amount);
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (damage <= 0f)
            return;

        if (invincibilityTimer > 0f)
            return;

        if (playerAttack != null && playerAttack.IsDefending())
            return;

        AddHealth(-damage);

        if (isDead)
            return;

        invincibilityTimer = getHitDuration + invincibilityAfterHit;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(hitSound, hitVolume);
        }

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

    public bool IsInvincible()
    {
        return invincibilityTimer > 0f;
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        StartCoroutine(PlayDeathSoundAfterDelay());
        StartCoroutine(ShowGameOverAfterDelay());
    }

    private System.Collections.IEnumerator PlayDeathSoundAfterDelay()
    {
        yield return new WaitForSecondsRealtime(deathSoundDelay);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(deathSound, deathVolume);
        }
    }

    private System.Collections.IEnumerator ShowGameOverAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f);

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOver();
        }
    }
}