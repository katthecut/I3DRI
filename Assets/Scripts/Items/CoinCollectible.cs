using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private AudioClip collectSound;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return;

        CoinManager coinManager = FindFirstObjectByType<CoinManager>();

        if (coinManager != null)
        {
            coinManager.AddCoins(value);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(collectSound);
            }

            Destroy(gameObject);
        }
    }
}