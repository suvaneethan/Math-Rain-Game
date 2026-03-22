using System.Collections;
using UnityEngine;

public class CoinFlyManager : MonoBehaviour
{
    public static CoinFlyManager Instance;

    public GameObject coinPrefab;
    public RectTransform canvas;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip coinSound;
    void Awake()
    {
        Instance = this;
    }

    public void PlayCoinFly(Vector3 startPos, Transform target, int count = 15)
    {
        StartCoroutine(FlyCoins(startPos, target, count));
    }

    IEnumerator FlyCoins(Vector3 startPos, Transform target, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject coin = Instantiate(coinPrefab, canvas);
            coin.transform.position = startPos;
            coin.transform.localScale = Vector3.zero;

            StartCoroutine(ScaleIn(coin));
            StartCoroutine(MoveCoin(coin, target));

            // 🔊 PLAY SOUND
            if (audioSource != null && coinSound != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f); // variation
                audioSource.PlayOneShot(coinSound, 0.4f);
            }

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.6f);

        var profile = FindObjectOfType<ProfileUI>();
        if (profile != null)
        {
            profile.PlayCoinPulse();
            profile.PlayCoinTextPunch();
        }
    }

    IEnumerator MoveCoin(GameObject coin, Transform target)
    {
        Vector3 start = coin.transform.position;
        Vector3 end = target.position;

        float duration = Random.Range(0.5f, 0.7f);
        float t = 0;

        Vector3 control = (start + end) / 2 + new Vector3(Random.Range(-100, 100), 150, 0);

        while (t < 1)
        {
            t += Time.deltaTime / duration;

            Vector3 pos =
                Mathf.Pow(1 - t, 2) * start +
                2 * (1 - t) * t * control +
                Mathf.Pow(t, 2) * end;

            coin.transform.position = pos;

            yield return null;
        }

        Destroy(coin);
    }

    IEnumerator ScaleIn(GameObject coin)
    {
        float t = 0;

        while (t < 0.2f)
        {
            t += Time.deltaTime;
            coin.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / 0.2f);
            yield return null;
        }
    }
}