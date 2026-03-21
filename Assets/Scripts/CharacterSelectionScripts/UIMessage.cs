using UnityEngine;
using TMPro;

public class UIMessage : MonoBehaviour
{
    public static UIMessage Instance;

    public TextMeshProUGUI messageText;

    void Awake()
    {
        Instance = this;

        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    // ✅ Auto hide (normal messages)
    public void Show(string msg, float duration = 2f)
    {
        if (messageText == null) return;

        messageText.text = msg;
        messageText.gameObject.SetActive(true);

        CancelInvoke(nameof(Hide));
        Invoke(nameof(Hide), duration);
    }

    // ✅ Show without auto hide (for loading)
    public void ShowPersistent(string msg)
    {
        if (messageText == null) return;

        CancelInvoke(nameof(Hide));

        messageText.text = msg;
        messageText.gameObject.SetActive(true);
    }

    // ✅ Manual hide
    public void Hide()
    {
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }
}