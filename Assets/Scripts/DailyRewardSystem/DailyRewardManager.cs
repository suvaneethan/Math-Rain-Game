using UnityEngine;
using System;

public class DailyRewardManager : MonoBehaviour
{
    public static DailyRewardManager Instance;

    int currentDay;
    string lastClaimDate;

  
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void LoadData()
    {
        currentDay = PlayerPrefs.GetInt("DailyDay", 1);
        lastClaimDate = PlayerPrefs.GetString("LastClaimDate", "");
    }

    void SaveData()
    {
        PlayerPrefs.SetInt("DailyDay", currentDay);
        PlayerPrefs.SetString("LastClaimDate", lastClaimDate);
        PlayerPrefs.Save();
    }

    public bool CanClaim()
    {
        string today = DateTime.Now.ToString("yyyyMMdd");
        return lastClaimDate != today;
    }

    public int GetReward()
    {
        switch (currentDay)
        {
            case 1: return 50;
            case 2: return 100;
            case 3: return 150;
            case 4: return 200;
            case 5: return 300;
            case 6: return 400;
            case 7: return 700;
            default: return 50;
        }
    }

    public int GetCurrentDay() => currentDay;

    public void ClaimReward()
    {
        if (!CanClaim()) return;

        int reward = GetReward();

        EconomyManager.Instance.AddCoins(reward);

        lastClaimDate = DateTime.Now.ToString("yyyyMMdd");

        currentDay++;
        if (currentDay > 7) currentDay = 1;

        SaveData();

        Debug.Log("🎁 Daily Reward Claimed: " + reward);
    }
}