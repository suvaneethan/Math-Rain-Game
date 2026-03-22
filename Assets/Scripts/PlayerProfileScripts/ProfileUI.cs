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

    public RectTransform panelRoot;

    int lastCoins = -1;
    Coroutine pulseRoutine;

    void OnEnable()
    {
        StartCoroutine(WaitAndUpdate());
    }

    IEnumerator WaitAndUpdate()
    {
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
        nameText.text = "Name: " + PlayerDataManager.Instance.playerName;
        rankText.text = "Rank: " + data.rank;
        levelText.text = "Level: " + data.level;

        // 🔹 COINS (AAA LOGIC)
        int newCoins = data.totalCoins;
        coinsText.text = "Total Coins: " + newCoins;

        if (lastCoins != -1 && newCoins > lastCoins)
        {
            PlayCoinPulse();
            PlayCoinTextPunch();
        }

        lastCoins = newCoins;

        // 🔹 STATS
        lastScoreText.text = "Last Score: " + data.lastScore;
        highScoreText.text = "High Score: " + data.highScore;
        playTimeText.text = "Play Time: " + FormatTime(data.totalPlayTime);

        // 🔹 XP BAR
        float xpNeeded = data.GetXPForNextLevel(data.level);
        xpFill.fillAmount = Mathf.Clamp01(data.currentXP / xpNeeded);

        UpdateRankIcon(data.rank);
    }

    void UpdateRankIcon(string rank)
    {
        switch (rank)
        {
            case "Beginner": rankIcon.sprite = beginnerIcon; break;
            case "Rookie": rankIcon.sprite = rookieIcon; break;
            case "Pro": rankIcon.sprite = proIcon; break;
            case "Master": rankIcon.sprite = masterIcon; break;
            case "Legend": rankIcon.sprite = legendIcon; break;
            default: rankIcon.sprite = beginnerIcon; break;
        }
    }

    public void PlayCoinPulse()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(PulseEffect());
    }

    public void PlayCoinTextPunch()
    {
        StartCoroutine(CoinPunch());
    }

    IEnumerator CoinPunch()
    {
        coinsText.transform.localScale = Vector3.one;

        Vector3 original = Vector3.one;
        Vector3 target = original * 1.15f;

        float t = 0;

        while (t < 0.1f)
        {
            t += Time.deltaTime;
            coinsText.transform.localScale = Vector3.Lerp(original, target, t / 0.1f);
            yield return null;
        }

        t = 0;

        while (t < 0.1f)
        {
            t += Time.deltaTime;
            coinsText.transform.localScale = Vector3.Lerp(target, original, t / 0.1f);
            yield return null;
        }
    }

    IEnumerator PulseEffect()
    {
        Vector3 original = panelRoot.localScale;
        Vector3 target = original * 1.08f;

        float t = 0;

        while (t < 0.12f)
        {
            t += Time.deltaTime;
            panelRoot.localScale = Vector3.Lerp(original, target, t / 0.12f);
            yield return null;
        }

        t = 0;

        while (t < 0.12f)
        {
            t += Time.deltaTime;
            panelRoot.localScale = Vector3.Lerp(target, original, t / 0.12f);
            yield return null;
        }
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return minutes + "m " + seconds + "s";
    }
}