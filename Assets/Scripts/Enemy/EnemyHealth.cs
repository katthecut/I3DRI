using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 20f;
    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f) return;

        currentHealth -= damage;

        Debug.Log($"{gameObject.name} primio je {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} je umro.");
        Destroy(gameObject);
    }
}