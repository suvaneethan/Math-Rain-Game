//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;
//using System.Collections;

//public class AnswerBox : MonoBehaviour
//{
//    public TextMeshProUGUI text;
//    int value;
//    public bool isCorrect;

//    bool isClicked = false;

//    RectTransform rt;

//    // 🔥 text highlight colors
//    public Color normalText = Color.white;
//    public Color selectedText = new Color(1f, 1f, 0.5f); // yellowish
//    public Color correctText = Color.green;

//    void Awake()
//    {
//        rt = GetComponent<RectTransform>();
//    }

//    public void Setup(int index)
//    {
//        value = GameManager.Instance.currentAnswers[index];
//        text.text = value.ToString();

//        isCorrect = (value == GameManager.Instance.correctAnswer);
//        isClicked = false;

//        ApplyTextStyle();

//        GetComponent<Button>().onClick.RemoveAllListeners();
//        GetComponent<Button>().onClick.AddListener(OnClick);
//    }

//    void ApplyTextStyle()
//    {
//        // 🔥 Big readable text
//        text.fontSize = 100;

//        // 🔥 Clean white text
//        text.color = normalText;

//        // 🔥 Strong outline (VERY IMPORTANT)
//        text.outlineWidth = 0.4f;
//        text.outlineColor = Color.black;
//    }

//    void OnEnable()
//    {
//        StartCoroutine(EnableColliderDelay());
//    }

//    IEnumerator EnableColliderDelay()
//    {
//        Collider2D col = GetComponent<Collider2D>();

//        if (col != null)
//            col.enabled = false;

//        yield return new WaitForSecondsRealtime(0.2f);

//        if (col != null)
//            col.enabled = true;
//    }

//    void Update()
//    {
//        // 🔥 Snap position (remove blur)
//        if (rt != null)
//        {
//            Vector2 pos = rt.anchoredPosition;
//            rt.anchoredPosition = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
//        }
//    }

//    void OnClick()
//    {
//        if (isClicked) return;

//        isClicked = true;

//        GetComponent<Button>().interactable = false;
//        GetComponent<Collider2D>().enabled = false;

//        HighlightSelectedLane();

//        if (isCorrect)
//        {
//            DisableAllButtons();
//            StartCoroutine(CorrectFlow());
//        }
//        else
//        {
//            StartCoroutine(WrongFlow());
//        }
//    }

//    // 🟡 Highlight selected lane (TEXT only)
//    void HighlightSelectedLane()
//    {
//        AnswerBox[] all = FindObjectsOfType<AnswerBox>();

//        foreach (AnswerBox box in all)
//        {
//            box.text.color = normalText * 0.6f; // dim others
//        }

//        text.color = selectedText; // highlight selected
//    }

//    void DisableAllButtons()
//    {
//        AnswerBox[] all = FindObjectsOfType<AnswerBox>();

//        foreach (AnswerBox box in all)
//        {
//            Button btn = box.GetComponent<Button>();
//            if (btn != null)
//                btn.interactable = false;
//        }
//    }

//    IEnumerator CorrectFlow()
//    {
//        HighlightCorrectAnswer();

//        StartCoroutine(KickEffect());

//        yield return new WaitForSeconds(0.2f);

//        GameManager.Instance.CheckAnswer(value);
//    }

//    IEnumerator WrongFlow()
//    {
//        HighlightCorrectAnswer(); // 🔥 still show correct
//        StartCoroutine(ShakeEffect());

//        yield return new WaitForSeconds(0.2f);

//        GameManager.Instance.CheckAnswer(value);
//    }

//    // 🟢 Show correct answer clearly
//    void HighlightCorrectAnswer()
//    {
//        AnswerBox[] all = FindObjectsOfType<AnswerBox>();

//        foreach (AnswerBox box in all)
//        {
//            if (box.isCorrect)
//            {
//                box.text.color = correctText;
//                box.text.fontSize = 110; // 🔥 slight boost
//            }
//        }
//    }

//    IEnumerator KickEffect()
//    {
//        Vector2 start = rt.anchoredPosition;
//        Vector2 up = start + new Vector2(0, 30f);

//        float time = 0;

//        while (time < 0.1f)
//        {
//            time += Time.deltaTime;
//            rt.anchoredPosition = Vector2.Lerp(start, up, time / 0.1f);
//            yield return null;
//        }

//        time = 0;

//        while (time < 0.1f)
//        {
//            time += Time.deltaTime;
//            rt.anchoredPosition = Vector2.Lerp(up, start, time / 0.1f);
//            yield return null;
//        }
//    }

//    IEnumerator ShakeEffect()
//    {
//        Vector2 original = rt.anchoredPosition;

//        float duration = 0.15f;
//        float strength = 20f;

//        float time = 0;

//        while (time < duration)
//        {
//            time += Time.deltaTime;

//            float x = Random.Range(-1f, 1f) * strength;
//            rt.anchoredPosition = original + new Vector2(x, 0);

//            yield return null;
//        }

//        rt.anchoredPosition = original;
//    }
//}

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class AnswerBox : MonoBehaviour
{
    public TextMeshProUGUI text;
    int value;
    public bool isCorrect;

    bool isClicked = false;

    RectTransform rt;

    public Color normalText = Color.white;
    public Color correctText = Color.green;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void Setup(int index)
    {
        value = GameManager.Instance.currentAnswers[index];
        text.text = value.ToString();

        isCorrect = (value == GameManager.Instance.correctAnswer);
        isClicked = false;

        ApplyTextStyle();

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void ApplyTextStyle()
    {
        text.fontSize = 65;
        text.color = normalText;

        text.outlineWidth = 0.4f;
        text.outlineColor = Color.black;
    }

    void OnEnable()
    {
        StartCoroutine(EnableColliderDelay());
    }

    IEnumerator EnableColliderDelay()
    {
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            col.enabled = false;

        yield return new WaitForSecondsRealtime(0.2f);

        if (col != null)
            col.enabled = true;
    }

    void Update()
    {
        if (rt != null)
        {
            Vector2 pos = rt.anchoredPosition;
            rt.anchoredPosition = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
        }
    }

    void OnClick()
    {
        if (isClicked) return;

        isClicked = true;

        GetComponent<Button>().interactable = false;
        GetComponent<Collider2D>().enabled = false;

        DisableAllButtons();

        // 🎯 STEP 1: SHOW CORRECT ANSWER FIRST
        ShowCorrectAnswerCenter();

        // ❌ shake only for wrong
        if (!isCorrect)
        {
            StartCoroutine(ShakeEffect());
        }

        // ⚡ STEP 2: DELAY GAME LOGIC (VERY IMPORTANT)
        StartCoroutine(DelayedCheck());
    }
    IEnumerator DelayedCheck()
    {
        yield return new WaitForSeconds(0.5f); // allow animation

        GameManager.Instance.CheckAnswer(value);
    }
    void DisableAllButtons()
    {
        AnswerBox[] all = FindObjectsOfType<AnswerBox>();

        foreach (AnswerBox box in all)
        {
            Button btn = box.GetComponent<Button>();
            if (btn != null)
                btn.interactable = false;
        }
    }

    // 🎯 NEW: Always show correct answer in center
    void ShowCorrectAnswerCenter()
    {
        AnswerBox[] all = FindObjectsOfType<AnswerBox>();

        AnswerBox correctBox = null;

        foreach (AnswerBox box in all)
        {
            if (box.isCorrect)
            {
                correctBox = box;
                break;
            }
        }

        if (correctBox != null)
        {
            correctBox.StartCoroutine(correctBox.CenterFocusEffect());
        }

        // fade others
        foreach (AnswerBox box in all)
        {
            if (box != correctBox)
            {
                box.text.alpha = 0.3f;
            }
        }
    }

    IEnumerator CenterFocusEffect()
    {
        transform.SetAsLastSibling();

        Vector2 start = rt.anchoredPosition;
        Vector2 center = new Vector2(start.x, 0f);

        float time = 0;

        while (time < 0.2f)
        {
            time += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(start, center, time / 0.2f);
            yield return null;
        }

        // 💥 scale up
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;

        time = 0;

        while (time < 0.15f)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, time / 0.15f);
            yield return null;
        }

        // 🟢 highlight
        text.color = correctText;
    }

    IEnumerator ShakeEffect()
    {
        Vector2 original = rt.anchoredPosition;

        float duration = 0.15f;
        float strength = 20f;

        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            float x = Random.Range(-1f, 1f) * strength;
            rt.anchoredPosition = original + new Vector2(x, 0);

            yield return null;
        }

        rt.anchoredPosition = original;
    }
}