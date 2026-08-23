using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private int coins;

    public int Coins => coins;

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        coins += amount;
    }

    public void RemoveCoins(int amount)
    {
        if (amount <= 0)
            return;

        coins = Mathf.Max(0, coins - amount);
    }

    public bool HasEnoughCoins(int amount)
    {
        return coins >= amount;
    }
}