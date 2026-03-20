using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class AnswerBox : MonoBehaviour
{
    public TextMeshProUGUI text;
    int value;
    public bool isCorrect;

    bool isClicked = false; 
    

    public Image bgImage;
    public Sprite[] bgSprites;
    public void Setup(int index)
    {
        value = GameManager.Instance.currentAnswers[index];
        text.text = value.ToString();

        isCorrect = (value == GameManager.Instance.correctAnswer);

        isClicked = false;

        SetRandomBackground();

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (isClicked) return;

        isClicked = true;

        GetComponent<Button>().interactable = false;
        GetComponent<Collider2D>().enabled = false;

        if (isCorrect)
        {
            DisableAllButtons();
            StartCoroutine(CorrectFlow());
        }
        else
        {
            StartCoroutine(WrongFlow());
        }
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

    void SetRandomBackground()
    {
        if (bgSprites == null || bgSprites.Length == 0) return;

        int index = Random.Range(0, bgSprites.Length);
        bgImage.sprite = bgSprites[index];
    }

    IEnumerator CorrectFlow()
    {
        StartCoroutine(GlowEffect());
        StartCoroutine(KickEffect());

        yield return new WaitForSeconds(0.15f); // 🔥 allow effect

        GameManager.Instance.CheckAnswer(value);
    }

    IEnumerator WrongFlow()
    {
        StartCoroutine(ShakeEffect());

        yield return new WaitForSeconds(0.15f);

        GameManager.Instance.CheckAnswer(value);
    }
    IEnumerator GlowEffect()
    {
        Color original = bgImage.color;

        bgImage.color = Color.white;
        yield return new WaitForSeconds(0.08f);

        bgImage.color = original;
    }

    IEnumerator KickEffect()
    {
        RectTransform rt = GetComponent<RectTransform>();

        Vector2 start = rt.anchoredPosition;
        Vector2 up = start + new Vector2(0, 20f);

        float time = 0;

        while (time < 0.1f)
        {
            time += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(start, up, time / 0.1f);
            yield return null;
        }

        time = 0;

        while (time < 0.1f)
        {
            time += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(up, start, time / 0.1f);
            yield return null;
        }
    }

    IEnumerator ShakeEffect()
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 original = rt.anchoredPosition;

        float duration = 0.15f;
        float strength = 15f;

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