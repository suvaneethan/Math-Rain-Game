using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("SFX")]
   
    public AudioClip newQuestion;
    public AudioClip correct;
    public AudioClip wrong;
    public AudioClip combo;
    public AudioClip ultra;

    [Header("Music")]
    public AudioClip bgMusic;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMusic();
    }

    // 🎼 Background Music
    public void PlayMusic()
    {
        if (bgMusic == null || musicSource == null) return;

        musicSource.clip = bgMusic;
        musicSource.loop = true;
        musicSource.volume = 0.5f;
        musicSource.Play();
    }

    // 🔊 General SFX
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.pitch = Random.Range(0.95f, 1.05f);
        sfxSource.PlayOneShot(clip, volume);
    }

  

    // 🔄 Combo Sound
    public void PlayCombo(int comboCount)
    {
        if (comboCount >= 10)
            PlaySFX(ultra);
        else if (comboCount >= 5)
            PlaySFX(combo);
        else
            PlaySFX(correct);
    }
}