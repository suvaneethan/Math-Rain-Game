using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    int coins;
    int runCoins;

    void Awake()
    {
        // 🔥 SAFE SINGLETON
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; // 🔥 VERY IMPORTANT
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        coins = PlayerPrefs.GetInt("Coins", 0);

        Debug.Log("EconomyManager Init → Coins: " + coins);
    }

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

    public int GetCoins() => coins;

    public int GetRunCoins() => runCoins;

    public void AddRunCoins(int amount)
    {
        runCoins += amount;
    }

    public void ResetRunCoins()
    {
        runCoins = 0;
    }
}