using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public int index;

    public GameObject lockIcon;
    public TextMeshProUGUI costText;

    public Button buyButton;        // ✅ Assign in Inspector
    public Button watchAdButton;    // ✅ Assign in Inspector

    public CharacterCarousel carousel;

    void Awake()
    {
        // 🎯 Attach events
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClicked);
        }

        if (watchAdButton != null)
        {
            watchAdButton.onClick.RemoveAllListeners();
            watchAdButton.onClick.AddListener(OnWatchAd);
        }
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (CharacterManager.Instance == null) return;

        var data = CharacterManager.Instance.characters[index];
        bool isLocked = !data.isUnlocked;

        // 🔒 Lock icon
        if (lockIcon != null)
            lockIcon.SetActive(isLocked);

        // 💰 Cost text
        if (costText != null)
        {
            costText.gameObject.SetActive(isLocked);

            if (isLocked)
                costText.text = "💰 " + data.coinCost;
        }
    }

    // 💰 BUY BUTTON
    public void OnBuyClicked()
    {
        Debug.Log("Buy Clicked");

        var data = CharacterManager.Instance.characters[index];

        if (data.isUnlocked)
        {
            UIMessage.Instance?.Show("✅ Already Unlocked");
            return;
        }

        int playerCoins = EconomyManager.Instance.GetCoins();

        if (playerCoins >= data.coinCost)
        {
            if (CharacterManager.Instance.UnlockWithCoins(index))
            {
                CharacterManager.Instance.RefreshAllButtons();
                FindObjectOfType<HomeUI>()?.UpdateCoins();

                carousel?.MoveToIndex(index);
                CharacterManager.Instance.SelectCharacter(index);

                UIMessage.Instance?.Show("✅ Unlocked");
            }
        }
        else
        {
            UIMessage.Instance?.Show("❌ Not enough coins");
        }
    }

    // 🎬 WATCH AD BUTTON
    public void OnWatchAd()
    {
        Debug.Log("Watch Ad Clicked");

        if (AdManager.Instance == null || !AdManager.Instance.IsAdReady)
        {
            UIMessage.Instance?.Show("❌ Ad not ready");
            return;
        }

        // ⏳ Loading
        UIMessage.Instance?.ShowPersistent("⏳ Loading Ad...");

        AdManager.Instance.ShowRewardedAd(() =>
        {
            UIMessage.Instance?.Hide();

            CharacterManager.Instance.UnlockWithAd(index);
            CharacterManager.Instance.RefreshAllButtons();

            carousel?.MoveToIndex(index);
            CharacterManager.Instance.SelectCharacter(index);

            UIMessage.Instance?.Show("✅ Unlocked");
        });
    }
}