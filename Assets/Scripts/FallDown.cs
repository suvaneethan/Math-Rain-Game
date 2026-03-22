using UnityEngine;
using System.Collections;

public class FallDown : MonoBehaviour
{
    public float baseSpeed = 150f;

    RectTransform rt;

    float currentSpeed;

    bool isHandled = false;
    bool canHit = false;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        currentSpeed = baseSpeed;

        StartCoroutine(EnableHitAfterDelay());
    }

    IEnumerator EnableHitAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);
        canHit = true;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.isGamePaused)
            return;

        float score = GameManager.Instance.GetScore();

        // 🔥 EXPONENTIAL SPEED CURVE (CORE FEEL)
        float targetSpeed = baseSpeed + Mathf.Pow(score, 1.2f) * 8f;

        // 🔥 COMBO BOOST (DOPAMINE FEEL)
        if (GameManager.Instance.GetCombo() >= 5)
        {
            targetSpeed += 30f;
        }

        targetSpeed = Mathf.Clamp(targetSpeed, 150f, 650f);

        // 🔥 SMOOTH TRANSITION
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 2f);

        // 🎯 UI ZOOM EFFECT (AAA FEEL)
        if (GameManager.Instance.gameRoot != null)
        {
            float t = Mathf.InverseLerp(200f, 600f, currentSpeed);
            float targetScale = Mathf.Lerp(1f, 1.08f, t);

            GameManager.Instance.gameRoot.localScale = Vector3.Lerp(
                GameManager.Instance.gameRoot.localScale,
                Vector3.one * targetScale,
                Time.deltaTime * 4f
            );
        }

        // 🔥 MICRO VARIATION (LESS ROBOTIC)
        float variation = Mathf.Sin(Time.time * 5f) * 10f;

        // 🎯 MOVE DOWN
        rt.anchoredPosition -= new Vector2(0, (currentSpeed + variation) * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isHandled) return;
        if (!canHit) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isGamePaused) return;
        if (GameManager.Instance.isReviving) return;

        if (other.CompareTag("Danger"))
        {
            // 🔥 GLOBAL LOCK FIRST (CRITICAL)
            if (GameManager.Instance.lifeHandled) return;

            GameManager.Instance.lifeHandled = true;
            isHandled = true;

            Debug.Log("🔥 HIT DETECTED");

            StartCoroutine(HitImpact());

            GameManager.Instance.MissCorrectAnswer();

            Destroy(gameObject);
        }
    }

    // 💥 SMALL TIME SLOW FOR IMPACT
    IEnumerator HitImpact()
    {
        Time.timeScale = 0.8f;

        yield return new WaitForSecondsRealtime(0.05f);

        Time.timeScale = 1f;
    }
}