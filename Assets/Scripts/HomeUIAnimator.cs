using UnityEngine;
using UnityEngine.EventSystems;

public class HomeUIAnimator : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform mainPanel;
    public RectTransform title;
    public RectTransform playButton;
    public RectTransform exitButton;

    [Header("Background PingPong")]
    public RectTransform background;
    public float moveDistance = 100f;
    public float moveSpeed = 0.5f; // lower = smoother

    [Header("Floating Settings")]
    public float floatSpeed = 2f;
    public float floatAmount = 15f;

    [Header("Title Pulse")]
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.05f;

    Vector3 panelStartPos;
    Vector3 titleStartScale;
    Vector2 bgStartPos;

    void Start()
    {
        if (mainPanel != null)
            panelStartPos = mainPanel.localPosition;

        if (title != null)
            titleStartScale = title.localScale;

        if (background != null)
            bgStartPos = background.anchoredPosition;
    }

    void Update()
    {
        AnimateFloating();
        AnimateTitlePulse();
        AnimateBackgroundPingPong();
    }

    // 🌊 Floating Panel
    void AnimateFloating()
    {
        if (mainPanel == null) return;

        float y = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        mainPanel.localPosition = panelStartPos + new Vector3(0, y, 0);
    }

    // ✨ Title Pulse
    void AnimateTitlePulse()
    {
        if (title == null) return;

        float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        title.localScale = titleStartScale * scale;
    }

    // 🌌 Smooth Left ↔ Right Background
    void AnimateBackgroundPingPong()
    {
        if (background == null) return;

        float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        background.anchoredPosition = bgStartPos + new Vector2(offset, 0);
    }

    // 🎮 Button Press Animation (Event Trigger)
    public void OnButtonDown(RectTransform btn)
    {
        if (btn != null)
            btn.localScale = Vector3.one * 0.9f;
    }

    public void OnButtonUp(RectTransform btn)
    {
        if (btn != null)
            btn.localScale = Vector3.one;
    }
}