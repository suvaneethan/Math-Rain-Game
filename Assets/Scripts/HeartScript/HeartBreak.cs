using UnityEngine;
using System.Collections;

public class HeartBreak : MonoBehaviour
{
    Vector3 startScale;
    RectTransform rt;
    CanvasGroup cg;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();

        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        startScale = transform.localScale;
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        if (rt == null) yield break;

        // 💥 POP
        float t = 0;
        while (t < 0.1f)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(startScale, startScale * 1.5f, t / 0.1f);
            yield return null;
        }

        // 🔥 SHAKE
        t = 0;
        Vector2 originalPos = rt.anchoredPosition;

        while (t < 0.15f)
        {
            t += Time.unscaledDeltaTime;
            rt.anchoredPosition = originalPos + Random.insideUnitCircle * 5f;
            yield return null;
        }

        // 💨 SHRINK + FADE
        t = 0;

        while (t < 0.25f)
        {
            t += Time.unscaledDeltaTime;
            float p = t / 0.25f;

            transform.localScale = Vector3.Lerp(startScale * 1.5f, Vector3.zero, p);

            if (cg != null)
                cg.alpha = 1 - p;

            yield return null;
        }

        Destroy(gameObject);
    }
}