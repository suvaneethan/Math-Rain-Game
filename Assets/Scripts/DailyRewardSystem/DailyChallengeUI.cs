using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DailyChallengeUI : MonoBehaviour
{
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI runText;

    public Button claimButton;
    public GameObject notificationBadge;

    public Image answerFill;
    public Image comboFill;
    public Image runFill;

    Coroutine glowRoutine;

    float answerTarget, comboTarget, runTarget;

    int lastAnswer, lastCombo, lastRun;


    void OnEnable()
    {
        StartCoroutine(WaitAndInit());
    }

    IEnumerator WaitAndInit()
    {
        // 🔥 wait until manager exists
        yield return new WaitUntil(() => DailyChallengeManager.Instance != null);

        // 🔥 wait until data loaded
        yield return new WaitUntil(() => DailyChallengeManager.Instance.IsReady);

        DailyChallengeManager.Instance.OnDailyUpdated += UpdateUI;

        // 🔥 FORCE FIRST UPDATE
        UpdateUI();
    }

    void OnDisable()
    {
        if (DailyChallengeManager.Instance != null)
            DailyChallengeManager.Instance.OnDailyUpdated -= UpdateUI;
    }

    void Update()
    {
        // 🔥 Smooth fill (no coroutine spam)
        answerFill.fillAmount = Mathf.Lerp(answerFill.fillAmount, answerTarget, Time.deltaTime * 6f);
        comboFill.fillAmount = Mathf.Lerp(comboFill.fillAmount, comboTarget, Time.deltaTime * 6f);
        runFill.fillAmount = Mathf.Lerp(runFill.fillAmount, runTarget, Time.deltaTime * 6f);

        // 🎨 Smooth gradient color
        answerFill.color = GetSmoothColor(answerFill.fillAmount);
        comboFill.color = GetSmoothColor(comboFill.fillAmount);
        runFill.color = GetSmoothColor(runFill.fillAmount);

        // 💓 subtle breathing
        Pulse(answerText);
        Pulse(comboText);
        Pulse(runText);
    }

    public void UpdateUI()
    {
        if (DailyChallengeManager.Instance == null) return;

        var dc = DailyChallengeManager.Instance;
        // 🔥 ALWAYS SET TEXT (CRITICAL FIX)
        answerText.text = $"<b>ANS</b>  {dc.GetAnswers()} / {dc.targetAnswers}";
        comboText.text = $"<b>COMBO</b>  {dc.GetBestCombo()} / {dc.targetCombo}";
        runText.text = $"<b>RUN</b>  {dc.GetRuns()} / {dc.targetRuns}";
        // 🎯 Targets
        answerTarget = (float)dc.GetAnswers() / dc.targetAnswers;
        comboTarget = (float)dc.GetBestCombo() / dc.targetCombo;
        runTarget = (float)dc.GetRuns() / dc.targetRuns;

      
        // 🎯 Animate text only if changed
        if (dc.GetAnswers() != lastAnswer)
            StartCoroutine(AnimateText(answerText, "ANS", dc.GetAnswers(), dc.targetAnswers));

        if (dc.GetBestCombo() != lastCombo)
            StartCoroutine(AnimateText(comboText, "COMBO", dc.GetBestCombo(), dc.targetCombo, true));

        if (dc.GetRuns() != lastRun)
            StartCoroutine(AnimateText(runText, "RUN", dc.GetRuns(), dc.targetRuns));

        lastAnswer = dc.GetAnswers();
        lastCombo = dc.GetBestCombo();
        lastRun = dc.GetRuns();

        bool canClaim = dc.CanClaimReward();

        claimButton.gameObject.SetActive(canClaim);
        notificationBadge.SetActive(canClaim);

        if (canClaim)
        {
            if (glowRoutine == null)
                glowRoutine = StartCoroutine(GlowEffect());
        }
        else
        {
            if (glowRoutine != null)
            {
                StopCoroutine(glowRoutine);
                glowRoutine = null;
            }

            claimButton.transform.localScale = Vector3.one;
        }

        CheckCompletionFlash(answerFill);
        CheckCompletionFlash(comboFill);
        CheckCompletionFlash(runFill);
    }

    // 🔥 TEXT COUNT + PUNCH
    IEnumerator AnimateText(TextMeshProUGUI txt, string label, int value, int max, bool strong = false)
    {
        int start = 0;
        float t = 0;

        while (t < 0.35f)
        {
            t += Time.deltaTime;
            int current = Mathf.RoundToInt(Mathf.Lerp(start, value, t / 0.35f));

            txt.text = $"<b>{label}</b>  {current} / {max}";
            yield return null;
        }

        txt.text = $"<b>{label}</b>  {value} / {max}";

        StartCoroutine(TextPunch(txt, strong));
    }

    IEnumerator TextPunch(TextMeshProUGUI txt, bool strong)
    {
        Vector3 original = txt.transform.localScale;
        Vector3 target = original * (strong ? 1.25f : 1.12f);

        float t = 0;

        while (t < 0.1f)
        {
            t += Time.deltaTime;
            txt.transform.localScale = Vector3.Lerp(original, target, t / 0.1f);
            yield return null;
        }

        t = 0;

        while (t < 0.1f)
        {
            t += Time.deltaTime;
            txt.transform.localScale = Vector3.Lerp(target, original, t / 0.1f);
            yield return null;
        }
    }

    // 🎨 SMOOTH COLOR BLEND (NOT HARD SWITCH)
    Color GetSmoothColor(float value)
    {
        if (value < 0.5f)
            return Color.Lerp(new Color(1f, 0.3f, 0.3f), new Color(1f, 0.8f, 0.2f), value * 2f);
        else
            return Color.Lerp(new Color(1f, 0.8f, 0.2f), new Color(0.3f, 1f, 0.3f), (value - 0.5f) * 2f);
    }

    // 💓 SUBTLE BREATHING
    void Pulse(TextMeshProUGUI txt)
    {
        float scale = 1f + Mathf.Sin(Time.time * 2f) * 0.02f;
        txt.transform.localScale = Vector3.one * scale;
    }

    // 💥 COMPLETION FLASH
    void CheckCompletionFlash(Image img)
    {
        if (img.fillAmount >= 0.99f)
        {
            StartCoroutine(Flash(img));
        }
    }

    IEnumerator Flash(Image img)
    {
        Color original = img.color;

        img.color = Color.white;

        yield return new WaitForSeconds(0.1f);

        img.color = original;
    }

    // 🔥 CLAIM GLOW
    IEnumerator GlowEffect()
    {
        while (true)
        {
            float scale = 1f + Mathf.Sin(Time.time * 5f) * 0.06f;
            claimButton.transform.localScale = Vector3.one * scale;
            yield return null;
        }
    }

    public void OnClaim()
    {
        Vector3 startPos = claimButton.transform.position;

        var profile = FindObjectOfType<ProfileUI>();

        if (profile != null)
        {
            CoinFlyManager.Instance.PlayCoinFly(
                startPos,
                profile.coinsText.transform,
                15
            );
        }

        DailyChallengeManager.Instance.ClaimReward();

        UpdateUI();
    }
}