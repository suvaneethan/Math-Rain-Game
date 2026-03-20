using UnityEngine;
using System.Collections;

public class AnswerSpawner : MonoBehaviour
{
    public GameObject answerPrefab;
    public Transform parent;

    private float[] lanes = { -300f, 0f, 300f };

    void Start()
    {
        SpawnSet();
    }

    public void SpawnSet()
    {
        if (GameManager.Instance.isGameOver || GameManager.Instance.isGamePaused) return;

        ClearAll();

        GameManager.Instance.GenerateQuestion();

        for (int i = 0; i < 3; i++)
        {
            GameObject obj = Instantiate(answerPrefab, parent);

            obj.GetComponent<AnswerBox>().Setup(i);

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.anchoredPosition = new Vector2(lanes[i], 900f);

            obj.AddComponent<AnswerPop>();
        }
    }

    public void ClearAndSpawnNew()
    {
        if (GameManager.Instance.isGameOver || GameManager.Instance.isGamePaused) return;

        StopAllCoroutines(); // 🔥 IMPORTANT
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        if (GameManager.Instance.isGameOver || GameManager.Instance.isGamePaused) yield break;

        foreach (Transform child in parent)
        {
            if (!child.GetComponent<AnswerCrunch>())
                child.gameObject.AddComponent<AnswerCrunch>();
        }

        // 🔥 FIXED (works even when paused)
        yield return new WaitForSecondsRealtime(0.25f);

        if (GameManager.Instance.isGameOver || GameManager.Instance.isGamePaused) yield break;

        SpawnSet();
    }

    public void ClearAll()
    {
        StopAllCoroutines(); // 🔥 EXTRA SAFETY

        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}