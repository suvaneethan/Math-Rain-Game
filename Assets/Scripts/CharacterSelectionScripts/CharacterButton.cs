using UnityEngine;
using TMPro;

public class CharacterButton : MonoBehaviour
{
    public int index;

    public GameObject lockIcon;
    public TextMeshProUGUI costText;
    public CharacterCarousel carousel;

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (CharacterManager.Instance == null)
        {
            Debug.LogError("CharacterManager is NULL!");
            return;
        }

        if (CharacterManager.Instance.characters == null ||
            CharacterManager.Instance.characters.Length == 0)
        {
            Debug.LogError("Characters array is empty!");
            return;
        }

        if (index < 0 || index >= CharacterManager.Instance.characters.Length)
        {
            Debug.LogError("Invalid index: " + index);
            return;
        }

        var data = CharacterManager.Instance.characters[index];

        bool isLocked = !data.isUnlocked;

        // 🔒 Lock icon
        if (lockIcon != null)
            lockIcon.SetActive(isLocked);

        // 💰 Cost text
        if (costText != null)
        {
            costText.gameObject.SetActive(isLocked);
            costText.text = "💰 " + data.coinCost;
        }
    }

    public void OnClick()
    {
        if (CharacterManager.Instance == null) return;

        var data = CharacterManager.Instance.characters[index];

        // ✅ Already unlocked → select
        if (data.isUnlocked)
        {
            if (carousel != null)
                carousel.MoveToIndex(index);

            CharacterManager.Instance.SelectCharacter(index);
            return;
        }

        int playerCoins = EconomyManager.Instance.GetCoins();

        // 💰 Enough coins → unlock
        if (playerCoins >= data.coinCost)
        {
            bool success = CharacterManager.Instance.UnlockWithCoins(index);

            if (success)
            {
                Refresh();

                // 🔥 Update coin UI
                FindObjectOfType<HomeUI>()?.UpdateCoins();

                if (carousel != null)
                    carousel.MoveToIndex(index);

                CharacterManager.Instance.SelectCharacter(index);
            }
        }
        else
        {
            // ❌ Not enough coins → show popup
            if (PopupManager.Instance != null)
                PopupManager.Instance.ShowNotEnoughCoins();
            else
                Debug.Log("Not enough coins!");
        }
    }

    public void OnBuyWithCoins()
    {
        if (CharacterManager.Instance == null) return;

        bool success = CharacterManager.Instance.UnlockWithCoins(index);

        if (success)
        {
            Refresh();

            FindObjectOfType<HomeUI>()?.UpdateCoins();

            if (carousel != null)
                carousel.MoveToIndex(index);

            CharacterManager.Instance.SelectCharacter(index);
        }
        else
        {
            if (PopupManager.Instance != null)
                PopupManager.Instance.ShowNotEnoughCoins();
        }
    }

    public void OnWatchAd()
    {
        if (AdManager.Instance == null || !AdManager.Instance.IsAdReady)
            return;

        AdManager.Instance.ShowRewardedAd(() =>
        {
            CharacterManager.Instance.UnlockWithAd(index);
            Refresh();

            // 🔥 Optional: auto select after ad unlock
            if (carousel != null)
                carousel.MoveToIndex(index);

            CharacterManager.Instance.SelectCharacter(index);
        });
    }
}