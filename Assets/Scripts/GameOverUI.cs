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

        // 🔥 Fade background
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            panel.alpha = t / fadeTime;
            yield return null;
        }

        // 🔥 Card scale pop
        yield return StartCoroutine(Pop(container, 0.25f));

        // 🔥 Title small bounce
        yield return StartCoroutine(PopText(titleText));

        // 🔢 Score
        yield return StartCoroutine(AnimateScore(finalScore, bestScore));

        // 💰 Coins
        yield return StartCoroutine(AnimateCoins(coinsEarned));

        // 🟡 New best
        if (finalScore >= bestScore)
            yield return StartCoroutine(NewBestEffect());

        // 🔘 Buttons
        yield return StartCoroutine(Pop(restartButton, 0.2f));
        yield return StartCoroutine(Pop(homeButton, 0.2f));

        // 💰 Total coins
        totalCoinText.text = "TotalCoins: " + EconomyManager.Instance.GetCoins();

        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    // ================= ANIMATIONS =================

    IEnumerator AnimateScore(int finalScore, int bestScore)
    {
        float duration = 1f;
        float time = 0;
        int last = -1;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            t = 1 - Mathf.Pow(1 - t, 3);

            int current = Mathf.RoundToInt(Mathf.Lerp(0, finalScore, t));

            scoreText.text = "Score: " + current;
            bestScoreText.text = "Best: " + bestScore;

            if (current != last)
            {
                last = current;
                if (audioSource && tickSound)
                {
                    audioSource.pitch = Random.Range(0.95f, 1.1f);
                    audioSource.PlayOneShot(tickSound);
                }
            }

            yield return null;
        }

        scoreText.text = "Score: " + finalScore;
    }

    IEnumerator AnimateCoins(int coins)
    {
        yield return new WaitForSecondsRealtime(0.2f);

        int display = 0;

        while (display < coins)
        {
            int step = Mathf.Max(1, coins / 20);
            display += step;
            display = Mathf.Min(display, coins);

            coinRewardText.text = "Coins: " + display;

            // 🔥 small punch
            coinRewardText.transform.localScale = Vector3.one * 1.2f;
            yield return new WaitForSecondsRealtime(0.02f);
            coinRewardText.transform.localScale = Vector3.one;
        }

        coinRewardText.text = "Coins: " + coins;
    }

    IEnumerator NewBestEffect()
    {
        newBestText.gameObject.SetActive(true);

        RectTransform rt = newBestText.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;

        float t = 0;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            rt.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.2f, t / 0.3f);
            yield return null;
        }
    }

    IEnumerator Pop(RectTransform target, float duration)
    {
        target.localScale = Vector3.zero;

        float t = 0;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            target.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.1f, t / duration);
            yield return null;
        }

        target.localScale = Vector3.one;
    }

    IEnumerator PopText(TextMeshProUGUI txt)
    {
        RectTransform rt = txt.GetComponent<RectTransform>();

        rt.localScale = Vector3.zero;

        float t = 0;
        while (t < 0.2f)
        {
            t += Time.unscaledDeltaTime;
            rt.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / 0.2f);
            yield return null;
        }
    }
}