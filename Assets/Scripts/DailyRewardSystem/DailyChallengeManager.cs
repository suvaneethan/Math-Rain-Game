using UnityEngine;
using System;
using System.Collections;

public class DailyChallengeManager : MonoBehaviour
{
    public static DailyChallengeManager Instance;

    public int targetAnswers = 20;  //20
    public int targetCombo = 10; //10
    public int targetRuns = 3;//3

    int currentAnswers;
    int bestCombo;
    int currentRuns;

    bool rewardClaimed;
    string lastDate;
    public System.Action OnDailyUpdated;

    public bool IsReady { get; private set; } = false;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadData();

            IsReady = true; // 🔥 IMPORTANT
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadData()
    {
        string today = DateTime.Now.ToString("yyyyMMdd");
        lastDate = PlayerPrefs.GetString("DC_Date", "");

        if (lastDate != today)
        {
            currentAnswers = 0;
            bestCombo = 0;
            currentRuns = 0;
            rewardClaimed = false;

            lastDate = today;
            SaveData();
        }
        else
        {
            currentAnswers = PlayerPrefs.GetInt("DC_Answers", 0);
            bestCombo = PlayerPrefs.GetInt("DC_Combo", 0);
            currentRuns = PlayerPrefs.GetInt("DC_Runs", 0);
            rewardClaimed = PlayerPrefs.GetInt("DC_Reward", 0) == 1;
        }
    }

    void SaveData()
    {
        PlayerPrefs.SetString("DC_Date", lastDate);
        PlayerPrefs.SetInt("DC_Answers", currentAnswers);
        PlayerPrefs.SetInt("DC_Combo", bestCombo);
        PlayerPrefs.SetInt("DC_Runs", currentRuns);
        PlayerPrefs.SetInt("DC_Reward", rewardClaimed ? 1 : 0);
        PlayerPrefs.Save();

        OnDailyUpdated?.Invoke(); // 🔥 CRITICAL FIX
    }

    public void AddAnswer()
    {
        if (IsCompleted()) return;

        currentAnswers = Mathf.Min(currentAnswers + 1, targetAnswers);

        SaveData();
    }

    public void UpdateCombo(int combo)
    {
        if (IsCompleted()) return;

        if (combo > bestCombo)
        {
            bestCombo = Mathf.Min(combo, targetCombo);
            SaveData();
        }
    }

    public void AddRun()
    {
        if (IsCompleted()) return;

        currentRuns = Mathf.Min(currentRuns + 1, targetRuns);

        SaveData();
    }

    public bool IsCompleted()
    {
        return currentAnswers >= targetAnswers &&
               bestCombo >= targetCombo &&
               currentRuns >= targetRuns;
    }

    public bool CanClaimReward()
    {
        return IsCompleted() && !rewardClaimed;
    }

    public int GetReward() => 200;

    public void ClaimReward()
    {
        Debug.Log("🔥 ClaimReward CALLED");

        if (!CanClaimReward())
        {
            Debug.Log("❌ Cannot claim reward");
            return;
        }

        if (EconomyManager.Instance == null)
        {
            Debug.LogError("❌ EconomyManager NULL");
            return;
        }

        rewardClaimed = true;
        SaveData();

        StartCoroutine(RewardFlow());
    }
    IEnumerator RewardFlow()
    {
        int reward = GetReward();

        yield return new WaitForSeconds(0.1f);

        // 🎯 Animate coins internally
        yield return StartCoroutine(
            EconomyManager.Instance.AddCoinsAnimated(reward)
        );

        // 🎯 AFTER animation → update UI properly
        var home = FindObjectOfType<HomeUI>();
        if (home != null)
        {
            int finalCoins = EconomyManager.Instance.GetCoins();
            home.StartCoinScrollAnimation(finalCoins);
        }

        Debug.Log("🎯 Reward Animation Done");
    }
    public int GetAnswers() => currentAnswers;
    public int GetBestCombo() => bestCombo;
    public int GetRuns() => currentRuns;
}