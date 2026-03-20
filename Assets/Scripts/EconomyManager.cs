using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    int coins;      // 💰 total coins (saved)
    int runCoins;   // 🟡 coins for current run (not saved)

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Optional: keep across scenes
        DontDestroyOnLoad(gameObject);

        coins = PlayerPrefs.GetInt("Coins", 0);
        runCoins = 0;
    }

    // ================= TOTAL COINS =================

    public void AddCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();
    }

    public bool SpendCoins(int amount)
    {
        if (coins < amount) return false;

        coins -= amount;
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();
        return true;
    }

    public int GetCoins()
    {
        return coins;
    }

    // ================= RUN COINS =================

    public int GetRunCoins()
    {
        return runCoins;
    }

    public void AddRunCoins(int amount)
    {
        runCoins += amount;
    }

    public void ResetRunCoins()
    {
        runCoins = 0;
    }
}