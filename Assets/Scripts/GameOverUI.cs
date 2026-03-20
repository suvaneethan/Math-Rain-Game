using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public CanvasGroup panel;

    public RectTransform gameOverText;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI newBestText;

    public RectTransform restartButton;

    public float fadeTime = 0.4f;

    public AudioSource audioSource;
    public AudioClip tickSound;

    void Awake()
    {
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;

        restartButton.localScale = Vector3.zero;

        if (newBestText != null)
            newBestText.gameObject.SetActive(false);
    }

    public void Show(int finalScore, int bestScore)
    {
        StartCoroutine(ShowRoutine(finalScore, bestScore));
    }

    public void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HomeScene");
    }
    IEnumerator ShowRoutine(int finalScore, int bestScore)
    {
        panel.gameObject.SetActive(true);

        // 🔥 Fade in
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            panel.alpha = t / fadeTime;
            yield return null;
        }

        panel.alpha = 1;

        // 💥 GAME OVER drop
        yield return StartCoroutine(DropBounce(gameOverText));

        // 🔢 Show scores
        yield return StartCoroutine(ShowScores(finalScore, bestScore));

        // 🟡 NEW BEST
        if (finalScore >= bestScore)
        {
            yield return StartCoroutine(NewBestEffect());
        }

        // 🔘 Button
        yield return StartCoroutine(PopButton(restartButton));

        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    IEnumerator ShowScores(int finalScore, int bestScore)
    {
        float duration = 1.2f;
        float time = 0;

        int lastPlayed = -1;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            float t = time / duration;
            t = 1 - Mathf.Pow(1 - t, 3); // ease out

            int currentScore = Mathf.RoundToInt(Mathf.Lerp(0, finalScore, t));

            // ✅ CLEAN NUMBER (no 000 format)
            scoreText.text = "Score: " + currentScore.ToString();
            bestScoreText.text = "Best: " + bestScore.ToString();

            // 🔊 tick sound only when number changes
            if (currentScore != lastPlayed)
            {
                lastPlayed = currentScore;

                if (audioSource != null && tickSound != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(tickSound);
                }
            }

            yield return null;
        }

        // ✅ final value
        scoreText.text = "Score: " + finalScore.ToString();
    }

    IEnumerator NewBestEffect()
    {
        newBestText.gameObject.SetActive(true);

        RectTransform rt = newBestText.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;

        Color gold = new Color(1f, 0.84f, 0f);
        Color white = Color.white;

        float time = 0;

        while (time < 0.3f)
        {
            time += Time.unscaledDeltaTime;
            rt.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.4f, time / 0.3f);
            yield return null;
        }

        // 🔥 glow pulse
        for (int i = 0; i < 3; i++)
        {
            float t = 0;

            while (t < 0.2f)
            {
                t += Time.unscaledDeltaTime;

                float pulse = Mathf.PingPong(t * 5f, 1f);

                newBestText.color = Color.Lerp(white, gold, pulse);
                rt.localScale = Vector3.one * (1 + pulse * 0.2f);

                yield return null;
            }
        }

        newBestText.color = gold;
        rt.localScale = Vector3.one;
    }

    IEnumerator DropBounce(RectTransform rect)
    {
        Vector2 start = new Vector2(0, 800);
        Vector2 end = Vector2.zero;

        rect.anchoredPosition = start;

        float t = 0;

        while (t < 0.4f)
        {
            t += Time.unscaledDeltaTime;
            float ease = 1 - Mathf.Pow(1 - t / 0.4f, 3);
            rect.anchoredPosition = Vector2.Lerp(start, end, ease);
            yield return null;
        }

        // bounce
        t = 0;
        Vector2 up = end + new Vector2(0, 40);

        while (t < 0.15f)
        {
            t += Time.unscaledDeltaTime;
            rect.anchoredPosition = Vector2.Lerp(end, up, t / 0.15f);
            yield return null;
        }

        t = 0;
        while (t < 0.15f)
        {
            t += Time.unscaledDeltaTime;
            rect.anchoredPosition = Vector2.Lerp(up, end, t / 0.15f);
            yield return null;
        }
    }

    IEnumerator PopButton(RectTransform btn)
    {
        btn.localScale = Vector3.zero;

        float t = 0;

        while (t < 0.2f)
        {
            t += Time.unscaledDeltaTime;
            btn.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.2f, t / 0.2f);
            yield return null;
        }

        t = 0;
        while (t < 0.1f)
        {
            t += Time.unscaledDeltaTime;
            btn.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.one, t / 0.1f);
            yield return null;
        }
    }
}