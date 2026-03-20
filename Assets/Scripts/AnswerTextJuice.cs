using UnityEngine;
using TMPro;

public class AnswerTextJuice : MonoBehaviour
{
    public float moveAmount = 4f;
    public float scaleAmount = 0.04f;
    public float colorSpeed = 2f;

    float speed;
    float offset;

    RectTransform rect;
    TMP_Text tmp;

    Vector2 startPos;
    Vector3 startScale;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        tmp = GetComponent<TMP_Text>();

        startPos = rect.anchoredPosition;
        startScale = rect.localScale;

        // 🔥 Random per text
        speed = Random.Range(2f, 4f);
        offset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float t = Time.time * speed + offset;

        // 🎈 Bounce movement
        float x = Mathf.Sin(t) * moveAmount;
        float y = Mathf.Cos(t * 1.2f) * moveAmount;
        rect.anchoredPosition = startPos + new Vector2(x, y);

        // 🟢 Jelly scale (safe small)
        float scaleX = 1 + Mathf.Sin(t) * scaleAmount;
        float scaleY = 1 + Mathf.Cos(t * 1.1f) * scaleAmount;
        rect.localScale = new Vector3(
            startScale.x * scaleX,
            startScale.y * scaleY,
            startScale.z
        );

        // 🌈 Rainbow per-letter color
        AnimateRainbow();
    }

    void AnimateRainbow()
    {
        tmp.ForceMeshUpdate();
        var textInfo = tmp.textInfo;

        float time = Time.time * colorSpeed;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int index = charInfo.vertexIndex;
            int matIndex = charInfo.materialReferenceIndex;
            var colors = textInfo.meshInfo[matIndex].colors32;

            // 🔥 Smooth rainbow using HSV
            float hue = Mathf.Repeat(time + i * 0.1f, 1f);
            Color col = Color.HSVToRGB(hue, 1f, 1f);

            Color32 c = col;

            colors[index + 0] = c;
            colors[index + 1] = c;
            colors[index + 2] = c;
            colors[index + 3] = c;
        }

        tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}