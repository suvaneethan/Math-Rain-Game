using UnityEngine;
using System.Collections;

public class FallDown : MonoBehaviour
{
    public float baseSpeed = 150f;

    RectTransform rt;

    float currentSpeed;

    bool isHandled = false;
    bool canHit = false; // 🔥 controls when object can damage

    void Start()
    {
        rt = GetComponent<RectTransform>();
        currentSpeed = baseSpeed;

        // 🔥 IMPORTANT: delay collision to prevent instant hit
        StartCoroutine(EnableHitAfterDelay());
    }

    IEnumerator EnableHitAfterDelay()
    {
        yield return new WaitForSeconds(0.3f); // 🔥 tweak if needed
        canHit = true;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.isGamePaused)
            return;

        // 🎯 TARGET SPEED BASED ON SCORE
        float targetSpeed = baseSpeed + GameManager.Instance.GetScore() * 10f;
        targetSpeed = Mathf.Clamp(targetSpeed, 150f, 600f);

        // 🔥 SMOOTH SPEED TRANSITION
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 2f);

        // 🎯 UI ZOOM EFFECT (arcade feel)
        if (GameManager.Instance.gameRoot != null)
        {
            float t = Mathf.InverseLerp(200f, 600f, currentSpeed);
            float targetScale = Mathf.Lerp(1f, 1.08f, t);

            Vector3 newScale = Vector3.Lerp(
                GameManager.Instance.gameRoot.localScale,
                Vector3.one * targetScale,
                Time.deltaTime * 4f
            );

            GameManager.Instance.gameRoot.localScale = newScale;
        }

        // 🎯 MOVE DOWN
        rt.anchoredPosition -= new Vector2(0, currentSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isHandled) return;

        // 🔥 CRITICAL SAFETY CHECKS
        if (!canHit) return; // prevent instant spawn hit
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isGamePaused) return;
        if (GameManager.Instance.isReviving) return;
        if (other.CompareTag("Danger"))
        {
            isHandled = true;

            if (!GameManager.Instance.lifeHandled)
            {
                GameManager.Instance.lifeHandled = true;

                GameManager.Instance.MissCorrectAnswer();

                if (GameManager.Instance.spawner != null)
                {
                    GameManager.Instance.spawner.ClearAndSpawnNew();
                }
            }

            Destroy(gameObject);
        }
    }
}