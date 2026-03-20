using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint; // assign in Inspector

    void Start()
    {
        Debug.Log("CharacterManager: " + CharacterManager.Instance);

        var charData = CharacterManager.Instance.GetSelectedCharacter();

        Debug.Log("Selected Character Index: " + CharacterManager.Instance.selectedIndex);

        if (charData != null && charData.prefab != null)
        {
            Instantiate(charData.prefab, spawnPoint.position, Quaternion.identity);
            Debug.Log("Character Spawned");
        }
        else
        {
            Debug.LogError("❌ Character prefab missing!");
        }
    }
}