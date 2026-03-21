using UnityEngine;
using TMPro;

public class DailyChallengeUI : MonoBehaviour
{
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI runText;

    public GameObject claimButton;
    public GameObject notificationBadge; // 🔴 NEW

    void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (DailyChallengeManager.Instance == null) return;

        var dc = DailyChallengeManager.Instance;

        answerText.text = $"{dc.GetAnswers()} / 20 Answers";
        comboText.text = $"{dc.GetBestCombo()} / 10 Combo";
        runText.text = $"{dc.GetRuns()} / 3 Runs";

        bool canClaim = dc.CanClaimReward();

        if (claimButton != null)
            claimButton.SetActive(canClaim);

        // 🔴 NOTIFICATION LOGIC
        if (notificationBadge != null)
            notificationBadge.SetActive(canClaim);
    }

    public void OnClaim()
    {
        DailyChallengeManager.Instance.ClaimReward();

        if (FindObjectOfType<HomeUI>() != null)
            FindObjectOfType<HomeUI>().UpdateCoins();

        UpdateUI();
    }
}