using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugReset : MonoBehaviour
{
    public void ResetGameData()
    {
        Debug.Log("🧹 Resetting Game Data...");

        // ✅ Clear ALL PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // ✅ Reset CharacterManager runtime data
        if (CharacterManager.Instance != null)
        {
            for (int i = 0; i < CharacterManager.Instance.characters.Length; i++)
            {
                // First character unlocked, others locked
                CharacterManager.Instance.characters[i].isUnlocked = (i == 0);
            }

            CharacterManager.Instance.selectedIndex = 0;
        }

        // ✅ Reset EconomyManager runtime data
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.SetCoins(0); // 👈 make sure this method exists
        }

        // ✅ Reload scene (fresh state)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}