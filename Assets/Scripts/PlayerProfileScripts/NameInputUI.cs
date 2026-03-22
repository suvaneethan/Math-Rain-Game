using UnityEngine;
using TMPro;

public class NameInputUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_InputField inputField;

    void Start()
    {
        if (string.IsNullOrEmpty(PlayerDataManager.Instance.playerName)
            || PlayerDataManager.Instance.playerName == "Player")
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }
    }

    public void OnConfirm()
    {
        string name = inputField.text.Trim();

        if (string.IsNullOrEmpty(name))
            return;

        PlayerDataManager.Instance.playerName = name;
        PlayerDataManager.Instance.SaveData();

        panel.SetActive(false);

        // 🔥 update UI instantly
        var profile = FindObjectOfType<ProfileUI>();
        if (profile != null)
            profile.UpdateUI();
    }
}