using UnityEngine;

public class FallDown : MonoBehaviour
{
    public float baseSpeed = 150f;

    RectTransform rt;
    bool isHandled = false;

    float currentSpeed;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.isGamePaused)
            return;

        // 🎯 TARGET SPEED
        float targetSpeed = baseSpeed + GameManager.Instance.GetScore() * 10f;
        targetSpeed = Mathf.Clamp(targetSpeed, 150f, 600f);

        // 🔥 SMOOTH SPEED
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 2f);

        // 🎯 NORMALIZED VALUE (0 → 1)
        float t = Mathf.InverseLerp(200f, 600f, currentSpeed);

        // 🔥 UI ZOOM EFFECT
        if (GameManager.Instance.gameRoot != null)
        {
            float targetScale = Mathf.Lerp(1f, 1.08f, t); // max zoom 8%

            Vector3 currentScale = GameManager.Instance.gameRoot.localScale;
            Vector3 newScale = Vector3.Lerp(currentScale, Vector3.one * targetScale, Time.deltaTime * 4f);

            GameManager.Instance.gameRoot.localScale = newScale;
        }
      

#if UNITY_EDITOR
        Debug.Log("Speed: " + currentSpeed);
#endif

        // 🎯 MOVE
        rt.anchoredPosition -= new Vector2(0, currentSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isHandled) return;

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