using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DailyRewardUI : MonoBehaviour
{
    public GameObject panel;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI infoText;

    public GameObject notificationBadge;

    [Header("Animation")]
    public RectTransform panelTransform;
    public RectTransform rewardTextTransform;
    public RectTransform claimButtonTransform;

    public Button claimButton;

    bool isClaiming = false;

    void OnEnable()
    {
        RefreshNotification();
    }

    void Start()
    {
        if (notificationBadge != null)
            notificationBadge.SetActive(DailyRewardManager.Instance.CanClaim());

        if (DailyRewardManager.Instance != null)
        {
            DailyRewardManager.Instance.OnDailyRewardAvailable += RefreshNotification;
        }

        panel.SetActive(false);
    }

    public void OnRewardButtonClick()
    {
        panel.SetActive(true);

        if (DailyRewardManager.Instance.CanClaim())
            ShowReward();
        else
            ShowAlreadyClaimed();
    }

    void ShowReward()
    {
        int day = DailyRewardManager.Instance.GetCurrentDay();
        int reward = DailyRewardManager.Instance.GetReward();

        dayText.text = "DAY " + day;
        rewardText.text = "Reward +" + reward;

        dayText.gameObject.SetActive(true);
        rewardText.gameObject.SetActive(true);
        infoText.gameObject.SetActive(false);

        claimButton.interactable = true;

        StartCoroutine(OpenAnimation());
        StartCoroutine(ButtonPulse());
    }

    void ShowAlreadyClaimed()
    {
        dayText.text = "DAILY REWARD";

        rewardText.gameObject.SetActive(false);

        infoText.gameObject.SetActive(true);
        infoText.text = "🎯 Already collected today!\nTry Daily Challenge";

        claimButton.interactable = false;

        StartCoroutine(OpenAnimation());
    }

    IEnumerator OpenAnimation()
    {
        panelTransform.localScale = Vector3.zero;

        float t = 0;
        while (t < 0.25f)
        {
            t += Time.deltaTime;
            panelTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.15f, t / 0.25f);
            yield return null;
        }

        panelTransform.localScale = Vector3.one;
    }

    IEnumerator ButtonPulse()
    {
        while (true)
        {
            float scale = 1f + Mathf.Sin(Time.time * 4f) * 0.05f;
            claimButtonTransform.localScale = Vector3.one * scale;
            yield return null;
        }
    }

    // 🎉 CLAIM
    public void OnClaim()
    {
        if (isClaiming) return;

        isClaiming = true;

        claimButton.interactable = false;

        if (notificationBadge != null)
            notificationBadge.SetActive(false);

        StartCoroutine(ClaimSequence());
    }

    IEnumerator ClaimSequence()
    {
        // ⏳ delay
        yield return new WaitForSeconds(0.15f);

        // 💥 reward punch
        yield return StartCoroutine(BigRewardPunch());

        int reward = DailyRewardManager.Instance.GetReward();

        DailyRewardManager.Instance.ClaimReward();

        // 🎯 NEW CLEAN COIN FLY
        Vector3 startPos = claimButtonTransform.position;

        var profile = FindObjectOfType<ProfileUI>();

        if (profile != null)
        {
            CoinFlyManager.Instance.PlayCoinFly(
                startPos,
                profile.coinsText.transform,
                15
            );
        }

        // 📈 update UI coins
        var home = FindObjectOfType<HomeUI>();
        if (home != null)
        {
            home.StartCoinScrollAnimation(EconomyManager.Instance.GetCoins());
        }
    }

    IEnumerator BigRewardPunch()
    {
        Vector3 start = Vector3.one;
        Vector3 big = Vector3.one * 1.8f;

        float t = 0;

        while (t < 0.1f)
        {
            t += Time.deltaTime;
            rewardTextTransform.localScale = Vector3.Lerp(start, big, t / 0.1f);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        t = 0;

        while (t < 0.15f)
        {
            t += Time.deltaTime;
            rewardTextTransform.localScale = Vector3.Lerp(big, start, t / 0.15f);
            yield return null;
        }
    }

    public void OnClose()
    {
        panel.SetActive(false);
    }

    void RefreshNotification()
    {
        if (notificationBadge == null) return;

        bool canClaim = DailyRewardManager.Instance.CanClaim();

        notificationBadge.SetActive(canClaim);

        if (canClaim)
        {
            StartCoroutine(BadgePop());
            StartCoroutine(BadgePulse());
        }
    }

    IEnumerator BadgePulse()
    {
        RectTransform rt = notificationBadge.GetComponent<RectTransform>();

        while (notificationBadge.activeSelf)
        {
            float scale = 1f + Mathf.Sin(Time.time * 6f) * 0.15f;
            rt.localScale = Vector3.one * scale;
            yield return null;
        }

        rt.localScale = Vector3.one;
    }

    IEnumerator BadgePop()
    {
        RectTransform rt = notificationBadge.GetComponent<RectTransform>();

        rt.localScale = Vector3.zero;

        float t = 0;

        while (t < 0.2f)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.2f, t / 0.2f);
            yield return null;
        }

        rt.localScale = Vector3.one;
    }
}