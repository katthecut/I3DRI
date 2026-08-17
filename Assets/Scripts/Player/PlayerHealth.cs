using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    //health
    private float health;
    [SerializeField] private float maxHealth = 100f;

    //ui
    [SerializeField] private HealthBarSystem healthBar;

    private bool isDead;

    public float Health => health;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        health = maxHealth;
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

        if (Input.GetKeyDown("g")) {
            SetHealth(-20f);
        }
        if (Input.GetKeyDown("h")) {
            SetHealth(20f);
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

        AddHealth(-damage);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        //animations
    }
}