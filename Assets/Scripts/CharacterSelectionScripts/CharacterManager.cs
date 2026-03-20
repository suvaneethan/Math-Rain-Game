using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public CharacterData[] characters;
    public int selectedIndex = 0;

    void Awake()
    {
        // 🔥 SAFE SINGLETON
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; // 🔥 IMPORTANT
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadData();

        Debug.Log("CharacterManager Init");
    }

    void LoadData()
    {
        selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        if (selectedIndex >= characters.Length)
            selectedIndex = 0;

        for (int i = 0; i < characters.Length; i++)
        {
            int defaultUnlock = (i == 0) ? 1 : 0;

            characters[i].isUnlocked =
                PlayerPrefs.GetInt("Char_" + characters[i].id, defaultUnlock) == 1;
        }
    }

    public void SelectCharacter(int index)
    {
        if (index < 0 || index >= characters.Length) return;
        if (!characters[index].isUnlocked) return;

        selectedIndex = index;

        PlayerPrefs.SetInt("SelectedCharacter", index);
        PlayerPrefs.Save();
    }

    public bool UnlockWithCoins(int index)
    {
        if (index < 0 || index >= characters.Length) return false;

        var data = characters[index];

        if (data.isUnlocked) return true;

        int cost = data.coinCost;

        if (EconomyManager.Instance.GetCoins() < cost)
            return false;

        EconomyManager.Instance.SpendCoins(cost);

        data.isUnlocked = true;

        PlayerPrefs.SetInt("Char_" + data.id, 1);
        PlayerPrefs.Save();

        return true;
    }

    public void UnlockWithAd(int index)
    {
        if (index < 0 || index >= characters.Length) return;

        var data = characters[index];

        if (data.isUnlocked) return;

        data.isUnlocked = true;

        PlayerPrefs.SetInt("Char_" + data.id, 1);
        PlayerPrefs.Save();
    }

    public CharacterData GetSelectedCharacter()
    {
        if (selectedIndex < 0 || selectedIndex >= characters.Length)
            selectedIndex = 0;

        return characters[selectedIndex];
    }
}