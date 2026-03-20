using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float duration = 0.15f;
    public float magnitude = 10f; // rotation angle

    private float timer = 0f;
    private float originalZ;

    void Awake()
    {
        originalZ = transform.localEulerAngles.z;
    }

    public void Shake()
    {
        timer = duration;
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;

            float zRotation = Random.Range(-magnitude, magnitude);

            transform.localRotation = Quaternion.Euler(0f, 0f, originalZ + zRotation);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, originalZ);
        }
    }
}