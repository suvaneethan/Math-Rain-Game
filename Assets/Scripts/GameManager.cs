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
    public TextMeshProUGUI loadingAdText;

    public TextMeshProUGUI questionText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI levelPopupText;
    public AudioClip levelUpSound;
   // public TextMeshProUGUI lifeText;
    public TextMeshProUGUI bestScoreText;

    public Image damageFlash;

    public bool isSpawningNow = false;
   public bool spawnRequested = false;
    bool isProcessingMiss = false;
    public bool isMissLocked = false;
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

    public bool isReviving = false;
    private bool pendingRevive = false;

    int runScore = 0;
    float runTime = 0f;

    public HeartUI heartUI;

    // 🔥 Gameplay progression
    enum OperationType { Add, Subtract }

    OperationType currentOperation;
    void Awake()
    {
        Instance = this;

        // 🔥 Reset run coins at game start (SAFE PLACE)
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.ResetRunCoins();
            Debug.Log("🔄 RunCoins Reset in GameManager Awake");
        }
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
        UpdateCoinUI();

        heartUI.Setup(maxLives);

        // 🔥 Ensure ad starts loading early
        if (AdManager.Instance != null)
        {
            StartCoroutine(CheckAdReadyRoutine());
        }
        if (spawner != null)
        {
            spawner.ClearAll(); // 🔥 remove any leftover objects
        }

        questionText.outlineWidth = 0.35f;
        questionText.outlineColor = Color.black;    
    }

    void Update()
    {
       // UpdateCoinUI();

        if (!isGamePaused && !isGameOver)
        {
            runTime += Time.deltaTime;
        }

        // 🔥 PROCESS REVIVE SAFELY ON THE MAIN THREAD
        if (pendingRevive)
        {
            pendingRevive = false;
            RevivePlayer();
        }


    }
    // ================= QUESTION =================

    public void GenerateQuestion()
    {
        isAnswered = false;
        lifeHandled = false;

        int min = 1, max = 10;

        // 🎯 LEVEL BASED DIFFICULTY
        switch (currentLevel)
        {
            case 1:
                min = 1; max = 10;
                currentOperation = OperationType.Add;
                break;

            case 2:
                min = 5; max = 20;
                currentOperation = (Random.value > 0.5f) ? OperationType.Add : OperationType.Subtract;
                break;

            case 3:
                min = 10; max = 50;
                currentOperation = (Random.value > 0.5f) ? OperationType.Add : OperationType.Subtract;
                break;

            default:
                min = 20; max = 100;
                currentOperation = (Random.value > 0.5f) ? OperationType.Add : OperationType.Subtract;
                break;
        }

        if (combo >= 5)
        {
            max += 10;
        }
        if (combo >= 10)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.combo);
        }
        int a = Random.Range(min, max);
        int b = Random.Range(min, max);

        string question = "";

        // 🎯 OPERATION LOGIC
        if (currentOperation == OperationType.Add)
        {
            correctAnswer = a + b;
            question = a + " + " + b;
        }
        else
        {
            // 🔥 avoid negative confusion (early levels)
            if (currentLevel <= 2 && a < b)
            {
                int temp = a;
                a = b;
                b = temp;
            }

            correctAnswer = a - b;
            question = a + " - " + b;
        }

        StartCoroutine(AnimateQuestion(question));

        GenerateAnswers();
    }
    IEnumerator AnimateQuestion(string question)
    {
        RectTransform rt = questionText.GetComponent<RectTransform>();
        CanvasGroup cg = questionText.GetComponent<CanvasGroup>();

        if (cg == null)
            cg = questionText.gameObject.AddComponent<CanvasGroup>();

        // 🔊 sound
        AudioManager.Instance.PlaySFX(AudioManager.Instance.newQuestion, 0.8f);

        questionText.text = question;

        // 🔥 START BIG (zoom out → zoom in feel)
        rt.localScale = Vector3.one * 2.2f;
        cg.alpha = 0;

        float t = 0;

        // 🎯 ZOOM IN (main effect)
        while (t < 0.25f)
        {
            t += Time.deltaTime;
            float p = t / 0.25f;

            rt.localScale = Vector3.Lerp(Vector3.one * 2.2f, Vector3.one * 0.9f, p);
            cg.alpha = p;

            yield return null;
        }

        // 🔥 BOUNCE BACK (settle)
        t = 0;
        while (t < 0.12f)
        {
            t += Time.deltaTime;
            float p = t / 0.12f;

            rt.localScale = Vector3.Lerp(Vector3.one * 0.9f, Vector3.one, p);
            yield return null;
        }

        // 💥 SMALL PUNCH (extra feel)
        StartCoroutine(QuestionPunch(rt));
    }
    IEnumerator QuestionPunch(RectTransform rt)
    {
        Vector3 original = rt.localScale;

        float t = 0;

        while (t < 0.08f)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(original, original * 1.1f, t / 0.08f);
            yield return null;
        }

        t = 0;

        while (t < 0.08f)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(original * 1.1f, original, t / 0.08f);
            yield return null;
        }
    }
    void GenerateAnswers()
    {
        int correctIndex = Random.Range(0, 3);

        for (int i = 0; i < 3; i++)
        {
            if (i == correctIndex)
            {
                currentAnswers[i] = correctAnswer;
            }
            else
            {
                int wrong;

                int range = (currentLevel <= 2) ? 3 : 2; // 🔥 tighter = harder

                do
                {
                    int offset = Random.Range(1, range + 1);
                    wrong = (Random.value > 0.5f)
                        ? correctAnswer + offset
                        : correctAnswer - offset;
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

            if (DailyChallengeManager.Instance != null)
            {
                DailyChallengeManager.Instance.AddAnswer();
                DailyChallengeManager.Instance.UpdateCombo(combo);
            }

            int points = 1;
            if (combo >= 10) points = 5;
            else if (combo >= 5) points = 3;
            else if (combo >= 3) points = 2;

            score += points;
            runScore = score; 
            int baseCoins = points * 2;
            int comboBonus = combo;
            int coinReward = baseCoins + comboBonus ;
            EconomyManager.Instance.AddRunCoins(coinReward);
            UpdateCoinUI();

            AudioManager.Instance.PlayCombo(combo);

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
       if (coinText != null )
       {
            int coins = EconomyManager.Instance.GetRunCoins();
            coinText.text = "Coins: " + coins;

            Debug.Log("💰 UI Updated: " + coins);
        }
    }
    void UpdateUI()
    {
        scoreText.text = "Score: " + score;
      //  lifeText.text = "Lives: " + currentLives;
      

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
        if (isMissLocked) return; // 🔥 HARD BLOCK
        isMissLocked = true;
        if (isProcessingMiss) return; // 🔥 HARD BLOCK (CRITICAL)
        isProcessingMiss = true;

        if (isReviving) return;

        currentLives--;
        heartUI.LoseLife(currentLives);

        AudioManager.Instance.PlaySFX(AudioManager.Instance.wrong);

        if (screenShake != null)
            screenShake.Shake();

        StartCoroutine(PunchUI(scoreText.transform));

        ShowDamageFlash();
        ResetCombo();
        UpdateUI();

        spawner.ClearAndSpawnNew(); // 🔥 ALWAYS CALL

        StartCoroutine(ResetMissLock()); // 🔥 UNLOCK AFTER FRAME

        if (currentLives <= 0)
        {
            if (!reviveUsed)
            {
                revivePanel.SetActive(true);

                isGamePaused = true;
                Time.timeScale = 0f;

                spawner.enabled = false;
                spawner.StopAllCoroutines();
                spawner.ClearAll();
            }
            else
            {
                GameOver();
            }
        }
    }
    IEnumerator ResetMissLock()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        isProcessingMiss = false;
        isMissLocked = false; // 🔥 RESET
        spawnRequested = false;
    }   

    void RevivePlayer()
    {
        Debug.Log("✅ RevivePlayer CALLED");

        isReviving = true;

        reviveUsed = true;

        revivePanel.SetActive(false);

        isGamePaused = false;
        isGameOver = false;
        Time.timeScale = 1f;

        currentLives = 1;
        heartUI.Setup(maxLives);
        heartUI.SetLives(currentLives);

        lifeHandled = false;
        UpdateUI();

        // 🔥 FORCE RESET GAME STATE
        if (spawner != null)
        {
            spawner.enabled = true;
            spawner.ClearAll();

            // 🔥 DELAYED SPAWN (IMPORTANT)
            StartCoroutine(ForceSpawnAfterRevive());
        }

        StartCoroutine(ReviveProtection());
    }
    // ================= BUTTONS =================

    // ================= BUTTONS =================

    public void OnWatchAdRevive()
    {
        if (isAdProcessing) return;
        isAdProcessing = true;

        
            // 🔥 If not ready, start the waiting animation!
            StartCoroutine(WaitAndShowAdRoutine());
        
    }

    IEnumerator WaitAndShowAdRoutine()
    {
        // Turn on the loading text
        if (loadingAdText != null)
        {
            loadingAdText.gameObject.SetActive(true);
            loadingAdText.text = "Loading Ad...";
        }

        float waitTime = 0f;
        float maxWaitTime = 4f; // ⏳ Wait up to 4 seconds for the internet

        // Loop until ad is ready OR time runs out
        while (waitTime < maxWaitTime)
        {
            if (AdManager.Instance.IsAdReady)
                break;

            waitTime += Time.unscaledDeltaTime;

            // 🔥 Calculate remaining time and round up (4, 3, 2, 1)
            float timeRemaining = maxWaitTime - waitTime;
            int secondsLeft = Mathf.CeilToInt(timeRemaining);

            // 🎨 Animate the dots (...)
            int dots = Mathf.FloorToInt(waitTime * 3) % 4;

            // 🔥 Update text with the timer and the animated dots
            if (loadingAdText != null)
            {
                // This will look like: "Loading Ad (3s)..."
                loadingAdText.text = $"Loading Ad ({secondsLeft}s)" + new string('.', dots);
            }

            yield return null; // Wait for next frame
        }

        // Hide the text when done waiting
        if (loadingAdText != null)
            loadingAdText.gameObject.SetActive(false);

        // Did the ad finally load?
        if (AdManager.Instance.IsAdReady)
        {
            Debug.Log("Ad finished loading! Showing now.");
            ShowAdSafely();
        }
        else
        {
            // ❌ Internet is too slow or no ads available. Give free revive.
            Debug.Log("Ad timeout -> Giving Free Revive Fallback");
            isAdProcessing = false;
            RevivePlayer();
        }
    }

    void ShowAdSafely()
    {
        // Hide the panel before the ad pops up
        if (revivePanel != null)
            revivePanel.SetActive(false);

        bool started = AdManager.Instance.ShowRewardedAd(() =>
        {
            Debug.Log("Ad Reward Callback → Revive");
            isAdProcessing = false;
            pendingRevive = true;
        });

        if (!started)
        {
            Debug.Log("❌ Ad failed → fallback revive");

            isAdProcessing = false;
            pendingRevive = true; // 🔥 ALWAYS revive
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
        EndRun();
        DailyChallengeManager.Instance.AddRun();

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
        // 🔊 Level Up Sound
        AudioManager.Instance.PlaySFX(levelUpSound);

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
    IEnumerator ReviveProtection()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        isReviving = false;
    }
    IEnumerator ForceSpawnAfterRevive()
    {
        yield return new WaitForSecondsRealtime(0.05f);

        Debug.Log("🔥 FORCE SPAWN AFTER REVIVE");

        spawner.SpawnSet();
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

    IEnumerator PunchUI(Transform target)
    {
        Vector3 original = target.localScale;

        float t = 0;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(original, original * 1.2f, t / 0.1f);
            yield return null;
        }

        t = 0;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(original * 1.2f, original, t / 0.1f);
            yield return null;
        }
    }

    void EndRun()
    {
        var data = PlayerDataManager.Instance;

        data.lastScore = runScore;
        data.totalPlayTime += runTime;

        if (runScore > data.highScore)
            data.highScore = runScore;

        data.AddXP(runScore); // 🔥 new system

        data.SaveData();

        Debug.Log("✅ EndRun Saved | Score: " + runScore);
    }

    public int GetCombo()
    {
        return combo;
    }
}