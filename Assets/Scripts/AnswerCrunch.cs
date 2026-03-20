
using UnityEngine;
using System.Collections;

public class AnswerCrunch : MonoBehaviour
{
    public float duration = 0.5f;
    public float freezeTime = 0.05f; // 🔥 hit stop duration

    Vector3 startScale;
    Vector3 startPos;

    void OnEnable()
    {
        startScale = transform.localScale;
        startPos = transform.position;

        StartCoroutine(HitEffect());
    }

    IEnumerator HitEffect()
    {
        // 🔥 FREEZE FRAME
        yield return StartCoroutine(FreezeFrame());

        // 🔥 Then play crunch animation
        yield return StartCoroutine(Crunch());
    }

    IEnumerator FreezeFrame()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(freezeTime);
        Time.timeScale = 1f;
    }

    IEnumerator Crunch()
    {
        float time = 0;

        Vector2 burstDir = Vector2.up + Random.insideUnitCircle * 0.3f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float ease = 1 - Mathf.Pow(1 - t, 4);

            float scaleX = Mathf.Lerp(1f, 0f, ease);
            float scaleY = Mathf.Lerp(1f, 0.3f, ease);

            transform.localScale = new Vector3(
                startScale.x * scaleX,
                startScale.y * scaleY,
                startScale.z
            );

            transform.position = startPos + (Vector3)(burstDir * ease * 2f);

            transform.Rotate(0, 0, 700f * Time.deltaTime);

            yield return null;
        }

        Destroy(gameObject);
    }
}