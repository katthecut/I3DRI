using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private CoinManager coinManager;
    [SerializeField] private TMP_Text coinText;

    private void Update()
    {
        if (coinManager == null || coinText == null)
            return;

        coinText.text = "Coins: " + coinManager.Coins;
    }
}