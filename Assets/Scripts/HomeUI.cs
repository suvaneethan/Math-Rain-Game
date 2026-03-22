using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HomeUI : MonoBehaviour
{
    public TextMeshProUGUI totalCoinText;

    int displayedCoins;
    int targetCoins;
    Coroutine coinAnimRoutine;

    void Start()
    {
        Time.timeScale = 1f;

        displayedCoins = EconomyManager.Instance.GetCoins();
        targetCoins = displayedCoins;

        UpdateCoins();
    }

    void OnEnable()
    {
        Invoke(nameof(UpdateCoins), 0.1f);

        var profile = FindObjectOfType<ProfileUI>();
        if (profile != null)
            profile.UpdateUI();
    }

    // 🔥 NORMAL UPDATE
    public void UpdateCoins()
    {
        if (EconomyManager.Instance == null || totalCoinText == null)
        {
            Debug.LogError("Missing references!");
            return;
        }

        int coins = EconomyManager.Instance.GetCoins();

        displayedCoins = coins;
        targetCoins = coins;

        totalCoinText.text = "Coins: " + coins;
    }

    // 🚀 AAA SCROLL ANIMATION (IMPORTANT)
    public void StartCoinScrollAnimation(int newAmount)
    {
        if (coinAnimRoutine != null)
            StopCoroutine(coinAnimRoutine);

        targetCoins = newAmount;

        coinAnimRoutine = StartCoroutine(ScrollCoins());
    }

    IEnumerator ScrollCoins()
    {
        while (displayedCoins < targetCoins)
        {
            displayedCoins += 1; // 🔥 REAL SCROLL

            totalCoinText.text = "Coins: " + displayedCoins;

            yield return new WaitForSeconds(0.01f); // 🔥 speed control
        }

        displayedCoins = targetCoins;
        totalCoinText.text = "Coins: " + targetCoins;
    }

    public void OnPlay()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    public void OnExit()
    {
        Application.Quit();
    }
}