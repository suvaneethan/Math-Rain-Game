using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    public string playerName = "Player";
    public string rank = "Beginner";
    public int level = 1;

    public int totalCoins;
    public int lastScore;
    public int highScore;
    public float totalPlayTime;

    public int currentXP = 0;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ✅ MUST
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveData()
    {
        PlayerPrefs.SetString("Name", playerName);
        PlayerPrefs.SetString("Rank", rank);
        PlayerPrefs.SetInt("Level", level);

        PlayerPrefs.SetInt("Coins", totalCoins);
        PlayerPrefs.SetInt("LastScore", lastScore);
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.SetFloat("PlayTime", totalPlayTime);

        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        playerName = PlayerPrefs.GetString("Name", "Player");
        rank = PlayerPrefs.GetString("Rank", "Beginner");
        level = PlayerPrefs.GetInt("Level", 1);

        totalCoins = PlayerPrefs.GetInt("Coins", 0);
        lastScore = PlayerPrefs.GetInt("LastScore", 0);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        totalPlayTime = PlayerPrefs.GetFloat("PlayTime", 0);
    }

    public void UpdateRankAndLevel(int score)
    {
        // 🎯 LEVEL SYSTEM
        level = Mathf.Clamp(score / 10 + 1, 1, 100);

        // 🏆 RANK SYSTEM
        if (score < 10)
            rank = "Beginner";
        else if (score < 25)
            rank = "Rookie";
        else if (score < 50)
            rank = "Pro";
        else if (score < 100)
            rank = "Master";
        else
            rank = "Legend";
    }

    public void AddXP(int score)
    {
        currentXP += score;

        while (currentXP >= GetXPForNextLevel(level))
        {
            currentXP -= GetXPForNextLevel(level);
            level++;
        }

        UpdateRank();
    }

    void UpdateRank()
    {
        if (level < 3)
            rank = "Beginner";
        else if (level < 6)
            rank = "Rookie";
        else if (level < 10)
            rank = "Pro";
        else if (level < 20)
            rank = "Master";
        else
            rank = "Legend";
    }
    public int GetXPForNextLevel(int level)
    {
        return level * 10; // 🔥 simple scaling (you can tune later)
    }
}