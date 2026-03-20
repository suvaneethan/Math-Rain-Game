using UnityEngine;
using System.Collections;

public class AnswerPop : MonoBehaviour
{
    public float duration = 0.9f;

    Vector3 targetScale;

    void OnEnable()
    {
        targetScale = transform.localScale;

        // Start from 0 (invisible)
        transform.localScale = Vector3.zero;

        StartCoroutine(Pop());
    }

    IEnumerator Pop()
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            // 🔥 Ease out (smooth)
            float ease = 1 - Mathf.Pow(1 - t, 3);

            // 🔥 Overshoot (pop effect)
            float scale = Mathf.Lerp(0f, 1.2f, ease);

            transform.localScale = targetScale * scale;

            transform.Rotate(0, 0, Random.Range(-5f, 5f));

            yield return null;
        }

        // 🔥 settle back to normal size
        transform.localScale = targetScale;
    }
}