using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    public RectTransform layer;
    public float moveDistance = 30f;
    public float moveSpeed = 0.3f;
}

public class ParallaxBG : MonoBehaviour
{
    public ParallaxLayer[] layers;

    Vector2[] startPositions;

    void Start()
    {
        startPositions = new Vector2[layers.Length];

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].layer != null)
                startPositions[i] = layers[i].layer.anchoredPosition;
        }
    }

    void Update()
    {
        AnimateLayers();
    }

    void AnimateLayers()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].layer == null) continue;

            float offsetX = Mathf.Sin(Time.time * layers[i].moveSpeed) * layers[i].moveDistance;

            // 🔥 ADD VERTICAL MOVEMENT
            float offsetY = Mathf.Cos(Time.time * layers[i].moveSpeed * 0.7f) * (layers[i].moveDistance * 0.5f);

            layers[i].layer.anchoredPosition =
                startPositions[i] + new Vector2(offsetX, offsetY);
        }
    }
}