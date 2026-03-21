using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GlobalButtonSound : MonoBehaviour
{
    [Header("Click Sound")]
    public AudioClip clickSound;

    private AudioSource audioSource;

    void Awake()
    {
        // 🔊 Create AudioSource automatically
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // 2D sound

        DontDestroyOnLoad(gameObject); // 🔥 persists across scenes
    }

    void Start()
    {
        StartCoroutine(AttachContinuously());
    }

    IEnumerator AttachContinuously()
    {
        while (true)
        {
            AddSoundToAllButtons();
            yield return new WaitForSeconds(1f);
        }
    }

    void AddSoundToAllButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);

        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveListener(PlayClickSound);
            btn.onClick.AddListener(PlayClickSound);
        }
    }

    void PlayClickSound()
    {
        if (clickSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(clickSound, 0.8f);
        }
    }
}