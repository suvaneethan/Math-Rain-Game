using System.Collections;
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

        Debug.Log("💰 RunCoins Added: " + amount + " | Total: " + runCoins);
    }
    public IEnumerator AddCoinsAnimated(int amount, float duration = 0.6f)
    {
        int startCoins = coins;
        int targetCoins = coins + amount;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            coins = Mathf.RoundToInt(Mathf.Lerp(startCoins, targetCoins, t));
            PlayerPrefs.SetInt("Coins", coins);

            yield return null;
        }

        coins = targetCoins;
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();

        Debug.Log("✨ Animated Coins Added: " + amount);
    }
    public void ResetRunCoins()
    {
        runCoins = 0;
    }
    public void SetCoins(int value)
    {
        coins = value;
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();
    }
}