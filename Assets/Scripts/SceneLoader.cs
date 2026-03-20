using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public TextMeshProUGUI loadingText;

    void Start()
    {
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

            // 🔥 FIX: Only update the progress text if we aren't ready yet
            if (!isReady && loadingText != null)
            {
                loadingText.text = Mathf.RoundToInt(progress) + "%";
            }

            if (progress >= 99f && !isReady)
            {
                isReady = true;

                // Force it to 100% and lock it in
                if (loadingText != null)
                    loadingText.text = "100%";

                yield return new WaitForSeconds(0.2f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}