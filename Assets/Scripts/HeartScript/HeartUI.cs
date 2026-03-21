using UnityEngine;
using System.Collections.Generic;

public class HeartUI : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform container;

    List<GameObject> hearts = new List<GameObject>();

    public void Setup(int maxLives)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        hearts.Clear();

        for (int i = 0; i < maxLives; i++)
        {
            GameObject h = Instantiate(heartPrefab, container);
            hearts.Add(h);
        }
    }

    // 🔥 Lose life (remove ONE heart visually)
    public void LoseLife(int currentLives)
    {
        if (currentLives < 0 || currentLives >= hearts.Count) return;

        GameObject heart = hearts[currentLives];

        if (heart != null)
        {
            heart.AddComponent<HeartBreak>(); // animation
            hearts[currentLives] = null; // 🔥 IMPORTANT (don’t shift list)
        }
    }

    // 🔥 Set exact lives (used for revive)
    public void SetLives(int lives)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (hearts[i] == null) continue;

            if (i < lives)
                hearts[i].SetActive(true);
            else
                hearts[i].SetActive(false);
        }
    }
}