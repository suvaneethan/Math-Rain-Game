using UnityEngine;

public class FallDown : MonoBehaviour
{
    public float speed = 150f;

    RectTransform rt;
    AnswerBox box;
    bool isHandled = false;
    void Start()
    {
        rt = GetComponent<RectTransform>();
        box = GetComponent<AnswerBox>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGamePaused)
            return;

        float dynamicSpeed = speed;

        if (GameManager.Instance != null)
        {
            dynamicSpeed += GameManager.Instance.GetScore() * 10f;
            dynamicSpeed = Mathf.Clamp(dynamicSpeed, 150f, 600f);
        }

        rt.anchoredPosition -= new Vector2(0, dynamicSpeed * Time.deltaTime);
    }





    void OnTriggerEnter2D(Collider2D other)
    {
        if (isHandled) return;

        if (other.CompareTag("Danger"))
        {
            isHandled = true;

            // 🔥 ONLY if not already handled
            if (!GameManager.Instance.lifeHandled)
            {
                GameManager.Instance.lifeHandled = true;
                GameManager.Instance.MissCorrectAnswer();
                FindObjectOfType<AnswerSpawner>().ClearAndSpawnNew();
            }

            Destroy(gameObject);
        }
    }
}