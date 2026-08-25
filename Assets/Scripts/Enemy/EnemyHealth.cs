using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    //health
    private float health;
    [SerializeField] private float maxHealth = 100f;

    //heart drop
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private float heartSpawnHeight = 1f;

    //sounds
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float deathVolume = 1f;

    private bool isDead;

    private Animator animator;
    private EnemyChest enemyChest;

    public float Health => health;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        health = maxHealth;

        animator = GetComponentInChildren<Animator>();
        enemyChest = GetComponent<EnemyChest>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (damage <= 0f)
            return;

        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (enemyChest != null)
        {
            enemyChest.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(
                deathSound,
                deathVolume
            );
        }

        Destroy(gameObject, 2f);
    }

    private void OnDestroy()
    {
        if (!isDead)
            return;

        if (heartPrefab != null)
        {
            Vector3 spawnPosition = transform.position;
            spawnPosition.y += heartSpawnHeight;

            Instantiate(
                heartPrefab,
                spawnPosition,
                Quaternion.identity
            );
        }
    }
}