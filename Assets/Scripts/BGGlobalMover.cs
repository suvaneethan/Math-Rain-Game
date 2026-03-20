using UnityEngine;

public class BGGlobalMover : MonoBehaviour
{
    public RectTransform bgRoot;
    public float moveDistance = 80f;
    public float moveSpeed = 0.3f;

    Vector2 startPos;

    void Start()
    {
        if (bgRoot != null)
            startPos = bgRoot.anchoredPosition;
    }

    void Update()
    {
        if (bgRoot == null) return;

        float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        bgRoot.anchoredPosition = startPos + new Vector2(offset, 0);
    }
}