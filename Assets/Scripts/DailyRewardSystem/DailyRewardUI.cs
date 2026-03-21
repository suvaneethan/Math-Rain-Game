using UnityEngine;
using TMPro;

public class DailyRewardUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI rewardText;
    public GameObject notificationBadge;
    void Start()
    {
        if (DailyRewardManager.Instance != null &&
            DailyRewardManager.Instance.CanClaim())
        {
            Show();
        }
        else
        {
            panel.SetActive(false);
        }

        if (notificationBadge != null)
            notificationBadge.SetActive(false);
    }

    public void Show()
    {
        panel.SetActive(true);

        int day = DailyRewardManager.Instance.GetCurrentDay();
        int reward = DailyRewardManager.Instance.GetReward();

        dayText.text = "Day " + day;
        rewardText.text = reward + " Coins";
    }

    public void OnClaim()
    {
        DailyRewardManager.Instance.ClaimReward();

        if (FindObjectOfType<HomeUI>() != null)
            FindObjectOfType<HomeUI>().UpdateCoins();

        panel.SetActive(false);
    }

    public void OnClose()
    {
        panel.SetActive(false);
    }
}