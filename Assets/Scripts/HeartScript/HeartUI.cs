using UnityEngine;
using System.Collections.Generic;

public class HeartUI : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform container;

    List<GameObject> hearts = new List<GameObject>();

    public void Setup(int maxLives)
    {
        // Clear old
        foreach (Transform child in container)
            Destroy(child.gameObject);

        hearts.Clear();

        for (int i = 0; i < maxLives; i++)
        {
            GameObject h = Instantiate(heartPrefab, container);
            hearts.Add(h);
        }
    }

    public void LoseLife(int currentLives)
    {
        if (currentLives < 0 || currentLives >= hearts.Count) return;

        GameObject heart = hearts[currentLives];

        // 🔥 Add animation script
        heart.AddComponent<HeartBreak>();

        // Remove from list
        hearts.RemoveAt(currentLives);
    }

    public void ResetHearts(int lives)
    {
        Setup(lives);
    }
}