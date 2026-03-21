using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    [Header("Top Info")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI levelText;

    [Header("Stats")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI lastScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI playTimeText;

    [Header("XP System")]
    public Image xpFill;

    [Header("Rank Icon")]
    public Image rankIcon;
    public Sprite beginnerIcon;
    public Sprite rookieIcon;
    public Sprite proIcon;
    public Sprite masterIcon;
    public Sprite legendIcon;

    void OnEnable()
    {
        StartCoroutine(WaitAndUpdate());
    }

    IEnumerator WaitAndUpdate()
    {
        // wait until PlayerDataManager is ready
        while (PlayerDataManager.Instance == null)
            yield return null;

        UpdateUI();
    }
    public void UpdateUI()
    {
        var data = PlayerDataManager.Instance;

        if (data == null)
        {
            Debug.LogWarning("PlayerDataManager not found!");
            return;
        }

        // 🔹 BASIC INFO
        nameText.text = "Name: " + data.playerName;
        rankText.text = "Rank: " + data.rank;
        levelText.text = "Level: " + data.level;

        // 🔹 STATS
        coinsText.text = "Total Coins: " + data.totalCoins;
        lastScoreText.text = "Last Score: " + data.lastScore;
        highScoreText.text = "High Score: " + data.highScore;
        playTimeText.text = "Play Time: " + FormatTime(data.totalPlayTime);

        // 🔹 XP BAR
        float xpNeeded = data.GetXPForNextLevel(data.level);
        xpFill.fillAmount = Mathf.Clamp01(data.currentXP / xpNeeded);

        // 🔹 RANK ICON
        UpdateRankIcon(data.rank);
    }

    void UpdateRankIcon(string rank)
    {
        switch (rank)
        {
            case "Beginner":
                rankIcon.sprite = beginnerIcon;
                break;

            case "Rookie":
                rankIcon.sprite = rookieIcon;
                break;

            case "Pro":
                rankIcon.sprite = proIcon;
                break;

            case "Master":
                rankIcon.sprite = masterIcon;
                break;

            case "Legend":
                rankIcon.sprite = legendIcon;
                break;

            default:
                rankIcon.sprite = beginnerIcon;
                break;
        }
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return minutes + "m " + seconds + "s";
    }
}