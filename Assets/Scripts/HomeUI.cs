using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeUI : MonoBehaviour
{
    public TextMeshProUGUI totalCoinText;

    void Start()
    {
        Time.timeScale = 1f;
        UpdateCoins();
    }

    void OnEnable()
    {
        Invoke(nameof(UpdateCoins), 0.1f);

        var profile = FindObjectOfType<ProfileUI>();
        if (profile != null)
            profile.UpdateUI();
    }
  
    public void UpdateCoins()
    {
        if (EconomyManager.Instance == null)
        {
            Debug.LogError("EconomyManager missing!");
            return;
        }

        if (totalCoinText == null)
        {
            Debug.LogError("totalCoinText NOT assigned!");
            return;
        }

        totalCoinText.text = "Coins: " + EconomyManager.Instance.GetCoins();
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