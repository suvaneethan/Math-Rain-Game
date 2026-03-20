


using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGamePaused = false;

    public RectTransform gameRoot;
    public TextMeshProUGUI coinText;
    bool rewardGiven = false;

    [Header("UI")]
    public GameOverUI gameOverUI;
    public GameObject revivePanel;

    public TextMeshProUGUI questionText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI levelPopupText;
    public AudioClip levelUpSound;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI bestScoreText;

    public Image damageFlash;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    [Header("Gameplay")]
    public AnswerSpawner spawner; // 🔥 assign in inspector
    public ScreenShake screenShake;

    public int maxLives = 3;

    int currentLives;
    int score = 0;
    int combo = 0;
    int bestScore = 0;
    int currentLevel = 1;

    public int correctAnswer;
    public int[] currentAnswers = new int[3];

    public bool isGameOver = false;
    public bool lifeHandled = false;
    public bool isAnswered = false;

    bool reviveUsed = false;
    private bool isAdProcessing = false;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 🔥 RESET GAME STATE
        isGameOver = false;
        isGamePaused = false;
        lifeHandled = false;
        isAnswered = false;
        reviveUsed = false;
        rewardGiven = false;

        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        currentLives = maxLives;
        UpdateUI();
        EconomyManager.Instance.ResetRunCoins();
        UpdateCoinUI();
        // 🔥 Ensure ad starts loading early
        if (AdManager.Instance != null)
        {
            StartCoroutine(CheckAdReadyRoutine());
        }
        if (spawner != null)
        {
            spawner.ClearAll(); // 🔥 remove any leftover objects
        }
    }

    void Update()
    {
        UpdateCoinUI();
    }
    // ================= QUESTION =================

    public void GenerateQuestion()
    {
        isAnswered = false;
        lifeHandled = false;

        int min = 1, max = 10;

        switch (currentLevel)
        {
            case 1: min = 1; max = 10; break;
            case 2: min = 5; max = 20; break;
            case 3: min = 10; max = 50; break;
            case 4: min = 20; max = 100; break;
            default: min = 50; max = 200; break;
        }

        int a = Random.Range(min, max);
        int b = Random.Range(min, max);

        correctAnswer = a + b;
        questionText.text = a + " + " + b;

        GenerateAnswers();
    }

    void GenerateAnswers()
    {
        int correctIndex = Random.Range(0, 3);

        for (int i = 0; i < 3; i++)
        {
            if (i == correctIndex)
                currentAnswers[i] = correctAnswer;
            else
            {
                int wrong;
                do
                {
                    wrong = correctAnswer + Random.Range(-3, 4);
                }
                while (wrong == correctAnswer || System.Array.Exists(currentAnswers, x => x == wrong));

                currentAnswers[i] = wrong;
            }
        }
    }

    // ================= ANSWER =================

    public void CheckAnswer(int value)
    {
        isAnswered = true;

        if (value == correctAnswer)
        {
            lifeHandled = true;
            combo++;

            int points = 1;
            if (combo >= 10) points = 5;
            else if (combo >= 5) points = 3;
            else if (combo >= 3) points = 2;

            score += points;

            int baseCoins = points * 2;
            int comboBonus = combo;
            int coinReward = baseCoins + comboBonus ;
            EconomyManager.Instance.AddRunCoins(coinReward);
            UpdateCoinUI();

            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(correctSound);

            if (combo >= 10)
                StartCoroutine(ComboSlowMotion());

            CheckLevelUp();
            UpdateUI();

            StartCoroutine(NextRoundDelay());
        }
        else
        {
            if (!lifeHandled)
            {
                lifeHandled = true;

                ShowDamageFlash();
                MissCorrectAnswer();
                spawner.ClearAndSpawnNew();
            }
        }

        // 🔥 Best score update
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }
    }

    // ================= UI =================
    public int GetScore()
    {
        return score;
    }

    void UpdateCoinUI()
    {
        if (coinText != null && EconomyManager.Instance != null)
        {
            coinText.text = "Coins: " + EconomyManager.Instance.GetRunCoins();
        }
    }
    void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        lifeText.text = "Lives: " + currentLives;
       

        if (combo <= 1)
            comboText.text = "";
        else if (combo < 5)
        {
            comboText.text = "🔥 Combo x" + combo;
            comboText.color = Color.yellow;
        }
        else if (combo < 10)
        {
            comboText.text = "⚡ SUPER x" + combo;
            comboText.color = new Color(1f, 0.5f, 0f);
        }
        else
        {
            comboText.text = "🚀 ULTRA x" + combo;
            comboText.color = Color.magenta;
        }

        bestScoreText.text = "BEST: " + bestScore;
    }

    void ShowDamageFlash()
    {
        if (isGameOver || isGamePaused) return;

        StopCoroutine(nameof(DamageFlashRoutine));
        StartCoroutine(DamageFlashRoutine());
    }

    IEnumerator DamageFlashRoutine()
    {
        Color c = damageFlash.color;
        c.a = 0.4f;
        damageFlash.color = c;

        yield return new WaitForSecondsRealtime(0.1f);

        c.a = 0;
        damageFlash.color = c;
    }

    // ================= LIFE =================

    public void MissCorrectAnswer()
    {
        currentLives--;

        audioSource.PlayOneShot(wrongSound);

        if (screenShake != null)
            screenShake.Shake();

        ShowDamageFlash();
        ResetCombo();
        UpdateUI();

        if (currentLives <= 0)
        {
            if (!reviveUsed)
            {
                revivePanel.SetActive(true);

                isGamePaused = true;
                Time.timeScale = 0f;

                // 🔥 STOP FLASH COMPLETELY
                StopCoroutine(nameof(DamageFlashRoutine));

                // Reset flash instantly
                Color c = damageFlash.color;
                c.a = 0;
                damageFlash.color = c;

                // STOP GAME
                spawner.enabled = false;
                spawner.StopAllCoroutines();
                spawner.ClearAll();

                StopAllCoroutines();
            }
            else
            {
                GameOver();
            }
        }
    }

    void RevivePlayer()
    {
        Debug.Log("RevivePlayer CALLED");

        reviveUsed = true;

        // ✅ SAFE UI CLOSE
        if (revivePanel != null)
        {
            revivePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("revivePanel is NULL!");
        }

        // ✅ RESUME GAME
        isGamePaused = false;
        Time.timeScale = 1f;

        // ✅ RESET LIFE
        currentLives = 1;
        UpdateUI();

        // ✅ SAFE SPAWNER RESTART
        if (spawner != null)
        {
            spawner.enabled = true;
            spawner.StopAllCoroutines();
            spawner.SpawnSet();
        }
        else
        {
            Debug.LogError("Spawner is NULL!");
        }
    }
    // ================= BUTTONS =================

    public void OnWatchAdRevive()
    {
        if (isAdProcessing) return;

        isAdProcessing = true;

        // 🔥 IMMEDIATE UI HIDE (important for mobile)
        if (revivePanel != null)
            revivePanel.SetActive(false);

        AdManager.Instance.ShowRewardedAd(() =>
        {
            isAdProcessing = false;
            StartCoroutine(DelayedRevive());
        });

        // 🔥 SAFETY FALLBACK (mobile fix)
        StartCoroutine(ForceCloseRevivePanel());
    }
    IEnumerator ForceCloseRevivePanel()
    {
        yield return new WaitForSecondsRealtime(2f);

        if (isGamePaused && revivePanel.activeSelf)
        {
            Debug.Log("⚠️ Force closing revive panel (mobile fix)");

            RevivePlayer();
        }
    }
    IEnumerator DelayedRevive()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        RevivePlayer();
    }

    IEnumerator WaitAndRetryAd()
    {
        Debug.Log("Waiting for ad...");

        float waitTime = 0;

        while (!AdManager.Instance.IsAdReady && waitTime < 3f)
        {
            waitTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (AdManager.Instance.IsAdReady)
        {
            Debug.Log("Ad loaded → showing now");
            OnWatchAdRevive();
        }
        else
        {
            Debug.Log("Still no ad → fallback revive");

            // 🔥 OPTION 2: Give free revive (best UX)
            RevivePlayer();
        }
    }
    public void OnNoThanks()
    {
        Debug.Log("No Thanks Clicked");

        isGamePaused = false;
        Time.timeScale = 1f;

        revivePanel.SetActive(false);

        GameOver();
    }

    // ================= GAME OVER =================

    void GameOver()
    {
        if (rewardGiven) return;

        rewardGiven = true;

        isGameOver = true;

        StopAllCoroutines();

        Color c = damageFlash.color;
        c.a = 0;
        damageFlash.color = c;

        spawner.enabled = false;

        int runCoins = EconomyManager.Instance.GetRunCoins();

        // 💰 convert run → total
        EconomyManager.Instance.AddCoins(runCoins);

        // 🎯 show UI
        gameOverUI.Show(score, bestScore, runCoins);

        UpdateCoinUI();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ================= EXTRA =================

    void ResetCombo()
    {
        combo = 0;
        UpdateUI();
    }

    void CheckLevelUp()
    {
        int oldLevel = currentLevel;

        if (score < 10) currentLevel = 1;
        else if (score < 25) currentLevel = 2;
        else if (score < 50) currentLevel = 3;
        else currentLevel = 4;

        // 🔥 Trigger popup only when level changes
        if (currentLevel > oldLevel)
        {
            StartCoroutine(ShowLevelPopup(currentLevel));
        }
    }
    IEnumerator ShowLevelPopup(int level)
    {
        if (levelPopupText == null) yield break;

        levelPopupText.gameObject.SetActive(true);
        levelPopupText.text = "Level: " + level;

        RectTransform rt = levelPopupText.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;

        // 🔊 sound
        if (audioSource != null && levelUpSound != null)
        {
            audioSource.PlayOneShot(levelUpSound);
        }

        // 🔥 POP IN
        float t = 0;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            rt.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.3f, t / 0.3f);
            yield return null;
        }

        // 🔥 SETTLE
        t = 0;
        while (t < 0.15f)
        {
            t += Time.unscaledDeltaTime;
            rt.localScale = Vector3.Lerp(Vector3.one * 1.3f, Vector3.one, t / 0.15f);
            yield return null;
        }

        // ⏳ stay visible
        yield return new WaitForSeconds(0.8f);

        // 🔥 FADE OUT
        CanvasGroup cg = levelPopupText.GetComponent<CanvasGroup>();
        if (cg == null) cg = levelPopupText.gameObject.AddComponent<CanvasGroup>();

        t = 0;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = 1 - (t / 0.3f);
            yield return null;
        }

        cg.alpha = 1;
        levelPopupText.gameObject.SetActive(false);
    }
    IEnumerator NextRoundDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        if (isGameOver || isGamePaused) yield break;

        spawner.ClearAndSpawnNew();
    }

    IEnumerator ComboSlowMotion()
    {
        Time.timeScale = 0.6f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
    }

    IEnumerator CheckAdReadyRoutine()
    {
        float timer = 0;

        while (!AdManager.Instance.IsAdReady && timer < 5f)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Debug.Log("Ad Ready Status: " + AdManager.Instance.IsAdReady);
    }
}