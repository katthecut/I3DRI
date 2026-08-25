using UnityEngine;

public class HeartCollectible : MonoBehaviour
{
    [SerializeField] private float healAmount = 25f;
    [SerializeField] private AudioClip collectSound;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.AddHealth(healAmount);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(collectSound);
            }

            Destroy(gameObject);
        }
    }
}