using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    public GameObject notEnoughCoinsPanel;

    void Awake()
    {
        Instance = this;
        notEnoughCoinsPanel.SetActive(false);
    }

    public void ShowNotEnoughCoins()
    {
        notEnoughCoinsPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        notEnoughCoinsPanel.SetActive(false);
    }
}