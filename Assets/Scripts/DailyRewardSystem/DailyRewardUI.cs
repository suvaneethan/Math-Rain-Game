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

    [Header("Coins")]
    public GameObject coinPrefab;
    public Transform coinParent;
    public Transform coinTarget;

    [Header("Floating Text")]
    public GameObject floatingTextPrefab;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip coinTickClip;

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

        // 🔥 TURN OFF BADGE IMMEDIATELY
        if (notificationBadge != null)
            notificationBadge.SetActive(false);

        StartCoroutine(ClaimSequence());
    }

    IEnumerator ClaimSequence()
    {
        // ⏳ anticipation delay
        yield return new WaitForSeconds(0.15f);

        // 💥 BIG punch
        yield return StartCoroutine(BigRewardPunch());

        int reward = DailyRewardManager.Instance.GetReward();

        DailyRewardManager.Instance.ClaimReward();

        // 💥 first big burst
        StartCoroutine(SpawnCoinsInChunks(reward));

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

    // 🚀 COIN SYSTEM
    IEnumerator SpawnCoinsInChunks(int reward)
    {
        int chunkValue = 50;

        // 💥 initial burst
        yield return StartCoroutine(SpawnCoinBurst(10));

        while (reward > 0)
        {
            int currentChunk = Mathf.Min(chunkValue, reward);

            yield return StartCoroutine(SpawnCoinBurst(3));

            ShowFloatingText(currentChunk);

            if (audioSource != null && coinTickClip != null)
            {
                audioSource.pitch = Random.Range(0.95f, 1.1f);
                audioSource.PlayOneShot(coinTickClip, 0.7f);
            }

            reward -= currentChunk;

            yield return new WaitForSeconds(0.08f);
        }
    }

    IEnumerator SpawnCoinBurst(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject coin = Instantiate(coinPrefab, coinParent);
            RectTransform rt = coin.GetComponent<RectTransform>();

            rt.localScale = Vector3.zero;
            rt.anchoredPosition = Vector2.zero;

            StartCoroutine(CoinFly(rt));

            yield return new WaitForSeconds(0.01f);
        }
    }

    void ShowFloatingText(int amount)
    {
        if (floatingTextPrefab == null) return;

        GameObject obj = Instantiate(floatingTextPrefab, coinParent);
        TextMeshProUGUI txt = obj.GetComponent<TextMeshProUGUI>();

        txt.text = "Reward +" + amount + " Coins";

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, 60);

        StartCoroutine(FloatText(rt, txt));
    }

    IEnumerator FloatText(RectTransform rt, TextMeshProUGUI txt)
    {
        Vector2 start = rt.anchoredPosition;
        Vector2 end = start + Vector2.up * 120;

        float t = 0;

        while (t < 0.5f)
        {
            t += Time.deltaTime;

            rt.anchoredPosition = Vector2.Lerp(start, end, t / 0.5f);
            txt.alpha = 1 - (t / 0.5f);

            yield return null;
        }

        Destroy(rt.gameObject);
    }

    IEnumerator CoinFly(RectTransform coin)
    {
        Vector2 start = coin.anchoredPosition;

        Vector2 target;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            coinParent as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, coinTarget.position),
            null,
            out target
        );

        float t = 0;

        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float progress = t / 0.5f;

            Vector2 pos = Vector2.Lerp(start, target, progress);
            pos.y += Mathf.Sin(progress * Mathf.PI) * 80f;

            coin.anchoredPosition = pos;
            coin.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
            coin.Rotate(0, 0, 400f * Time.deltaTime);

            yield return null;
        }

        Destroy(coin.gameObject);
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
            StartCoroutine(BadgePop());   // 💥 new
            StartCoroutine(BadgePulse()); // existing
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