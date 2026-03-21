using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public CanvasGroup panel;

    [Header("Main Elements")]
    public RectTransform container; // 🔥 whole card (assign bg parent)
    public TextMeshProUGUI titleText;

    [Header("Stats")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI coinRewardText;
    public TextMeshProUGUI totalCoinText;

    [Header("Extras")]
    public TextMeshProUGUI newBestText;

    [Header("Buttons")]
    public RectTransform restartButton;
    public RectTransform homeButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip tickSound;
    public AudioClip slamSound; // 💡 AAA Touch: Add a heavy thud sound for when the panel drops
    public AudioClip cashSound; // 💡 AAA Touch: Add a cha-ching for the coins

    public float fadeTime = 0.3f;

    bool isShowing = false;

    void Awake()
    {
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;

        container.localScale = Vector3.zero;
        restartButton.localScale = Vector3.zero;
        homeButton.localScale = Vector3.zero;
        titleText.rectTransform.localScale = Vector3.zero;

        if (newBestText != null)
            newBestText.gameObject.SetActive(false);

        coinRewardText.text = "";
        totalCoinText.text = "";
    }

    public void Show(int finalScore, int bestScore, int coinsEarned)
    {
        if (isShowing) return;
        isShowing = true;

        StartCoroutine(ShowRoutine(finalScore, bestScore, coinsEarned));
    }

    public void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HomeScene");
    }

    IEnumerator ShowRoutine(int finalScore, int bestScore, int coinsEarned)
    {
        panel.gameObject.SetActive(true);

        // 1. FAST BACKGROUND FADE
        StartCoroutine(FadeCanvasGroup(panel, 1f, fadeTime));

        // 2. AAA SLAM: Container overshoots and settles
        yield return new WaitForSecondsRealtime(0.1f);
        if (audioSource && slamSound) audioSource.PlayOneShot(slamSound, 0.8f);
        StartCoroutine(ScaleEaseOutBack(container, Vector3.one, 0.4f));

        // 3. STAGGERED ELEMENTS: Don't wait for the container to finish!
        yield return new WaitForSecondsRealtime(0.15f);
        StartCoroutine(ScaleEaseOutBack(titleText.rectTransform, Vector3.one, 0.3f));

        // 4. DYNAMIC TALLYING
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(AnimateScore(finalScore, bestScore));

        // Offset the coins slightly so they don't count at the exact same time
        yield return new WaitForSecondsRealtime(0.2f);
        StartCoroutine(AnimateCoins(coinsEarned));

        // 5. NEW BEST BURST
        if (finalScore >= bestScore && finalScore > 0)
        {
            yield return new WaitForSecondsRealtime(0.5f); // Wait for score to tally a bit
            StartCoroutine(NewBestEffect());
        }

        // 6. STAGGERED BUTTON POPS
        yield return new WaitForSecondsRealtime(0.3f);
        StartCoroutine(ScaleEaseOutBack(restartButton, Vector3.one, 0.3f));
        yield return new WaitForSecondsRealtime(0.08f); // 💡 Notice the tiny stagger delay
        StartCoroutine(ScaleEaseOutBack(homeButton, Vector3.one, 0.3f));

        // 7. FINALIZE
        totalCoinText.text = "Total Coins: " + EconomyManager.Instance.GetCoins();

        // Add a continuous floating idle animation to the container
        StartCoroutine(IdleFloat(container));

        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    // ================= AAA ANIMATIONS =================

    IEnumerator AnimateScore(int finalScore, int bestScore)
    {
        float duration = 1.2f;
        float time = 0;
        int last = -1;

        // Reset pitch
        if (audioSource) audioSource.pitch = 1f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            // 🔥 EaseOutExpo curve: Starts counting fast, slows down smoothly at the end
            float t = time / duration;
            float ease = (t == 1) ? 1 : 1 - Mathf.Pow(2, -10 * t);

            int current = Mathf.RoundToInt(Mathf.Lerp(0, finalScore, ease));

            scoreText.text = "Score: " + current;
            bestScoreText.text = "Best: " + bestScore;

            if (current != last)
            {
                last = current;
                if (audioSource && tickSound)
                {
                    // 💡 AAA Touch: Pitch rises as the score gets closer to the end!
                    audioSource.pitch = Mathf.Lerp(0.9f, 1.5f, ease);
                    audioSource.PlayOneShot(tickSound, 0.4f);
                }

                // Micro-punch text on every tick
                scoreText.transform.localScale = Vector3.one * 1.05f;
            }
            else
            {
                // Settle back down quickly between ticks
                scoreText.transform.localScale = Vector3.Lerp(scoreText.transform.localScale, Vector3.one, Time.unscaledDeltaTime * 15f);
            }

            yield return null;
        }

        scoreText.transform.localScale = Vector3.one;
        scoreText.text = "Score: " + finalScore;
    }

    IEnumerator AnimateCoins(int coins)
    {
        float duration = 0.8f;
        float time = 0;
        int last = -1;

        if (audioSource && cashSound) audioSource.PlayOneShot(cashSound, 0.6f);

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            float ease = (t == 1) ? 1 : 1 - Mathf.Pow(2, -10 * t); // Smooth slow down

            int display = Mathf.RoundToInt(Mathf.Lerp(0, coins, ease));

            if (display != last)
            {
                last = display;
                coinRewardText.text = "Coins: " + display;
                coinRewardText.transform.localScale = Vector3.one * 1.15f;
            }
            else
            {
                coinRewardText.transform.localScale = Vector3.Lerp(coinRewardText.transform.localScale, Vector3.one, Time.unscaledDeltaTime * 10f);
            }

            yield return null;
        }

        coinRewardText.transform.localScale = Vector3.one;
        coinRewardText.text = "Coins: " + coins;
    }

    IEnumerator NewBestEffect()
    {
        newBestText.gameObject.SetActive(true);
        RectTransform rt = newBestText.GetComponent<RectTransform>();

        // Pop in with extreme overshoot
        yield return StartCoroutine(ScaleEaseOutBack(rt, Vector3.one, 0.4f, 2.5f));

        // Then pulse continuously
        while (true)
        {
            float scale = 1f + Mathf.Sin(Time.unscaledTime * 6f) * 0.08f; // Breathing effect
            rt.localScale = Vector3.one * scale;

            // 💡 AAA Touch: subtle rainbow color shift for the "New Best"
            float hue = Mathf.Repeat(Time.unscaledTime * 0.5f, 1f);
            newBestText.color = Color.HSVToRGB(hue, 0.8f, 1f);

            yield return null;
        }
    }

    // ================= CORE MATH & JUICE UTILITIES =================

    IEnumerator ScaleEaseOutBack(RectTransform target, Vector3 targetScale, float duration, float overshootMultiplier = 1.70158f)
    {
        target.localScale = Vector3.zero;
        float time = 0;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            // 🔥 AAA Math: The "Back" easing formula causes it to go slightly past the target and spring back
            float c3 = overshootMultiplier + 1f;
            float ease = 1f + c3 * Mathf.Pow(t - 1, 3) + overshootMultiplier * Mathf.Pow(t - 1, 2);

            target.localScale = Vector3.LerpUnclamped(Vector3.zero, targetScale, ease);
            yield return null;
        }

        target.localScale = targetScale;
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha, float duration)
    {
        float startAlpha = cg.alpha;
        float time = 0;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            // Smooth step for nicer fading
            cg.alpha = Mathf.SmoothStep(startAlpha, targetAlpha, time / duration);
            yield return null;
        }
        cg.alpha = targetAlpha;
    }

    IEnumerator IdleFloat(RectTransform target)
    {
        Vector2 startPos = target.anchoredPosition;
        while (true)
        {
            // Subtle slow vertical floating
            float yOffset = Mathf.Sin(Time.unscaledTime * 2f) * 5f;
            target.anchoredPosition = startPos + new Vector2(0, yOffset);
            yield return null;
        }
    }
}