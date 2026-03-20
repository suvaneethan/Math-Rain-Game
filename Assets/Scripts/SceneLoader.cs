using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI loadingText; // For "Loading... 45%"
    public TextMeshProUGUI tipsText;    // Optional: For "Tip: Jump over spikes!"

    [Header("Loading Screen Settings")]
    public string[] gameTips = {
        "Did you know? Math makes you smarter!",
        "Watch out for the spikes!",
        "Collect coins to boost your score.",
        "Take your time and calculate carefully."
    };
    public float tipChangeInterval = 2.5f;

    void Start()
    {
        // Start cycling tips if a Text component was assigned
        if (tipsText != null && gameTips.Length > 0)
        {
            StartCoroutine(AnimateTips());
        }

        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("GameScene");
        op.allowSceneActivation = false;

        float progress = 0;
        bool isReady = false;

        while (!op.isDone)
        {
            float target = Mathf.Clamp01(op.progress / 0.9f) * 100f;
            progress = Mathf.Lerp(progress, target, Time.deltaTime * 5f);

            // 🔥 FIX: Animate dots and update progress
            if (!isReady && loadingText != null)
            {
                // Calculates 0, 1, 2, or 3 based on game time to animate dots
                int numDots = Mathf.FloorToInt(Time.unscaledTime * 3f) % 4;
                string animatedDots = new string('.', numDots);

                loadingText.text = $"Loading{animatedDots} {Mathf.RoundToInt(progress)}%";
            }

            if (progress >= 99f && !isReady)
            {
                isReady = true;

                // Force it to 100% and show a "Ready" state
                if (loadingText != null)
                    loadingText.text = "Ready! 100%";

                yield return new WaitForSeconds(0.2f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    // Coroutine to smoothly cycle through game tips
    IEnumerator AnimateTips()
    {
        int currentTipIndex = Random.Range(0, gameTips.Length);

        while (true)
        {
            tipsText.text = gameTips[currentTipIndex];

            yield return new WaitForSeconds(tipChangeInterval);

            // Move to the next tip, loop back to 0 if at the end
            currentTipIndex = (currentTipIndex + 1) % gameTips.Length;
        }
    }
}