using UnityEngine;

public class HeartCollectible : MonoBehaviour
{
    [SerializeField] private float healAmount = 25f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.AddHealth(healAmount);

            Destroy(gameObject);
        }
    }
}