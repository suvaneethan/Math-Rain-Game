//using UnityEngine;
//using System.Collections;

//public class CharacterCarousel : MonoBehaviour
//{
//    public Transform[] characters;

//    public float spacing = 400f;
//    public float moveSpeed = 10f;

//    public float selectedScale = 1.3f;
//    public float normalScale = 0.9f;

//    [Header("Inertia")]
//    public float deceleration = 5f;
//    public float velocityMultiplier = 0.02f;

//    float currentOffset = 0f;
//    float targetOffset = 0f;

//    float velocity = 0f;

//    bool isDragging = false;
//    Vector2 lastTouchPos;

//    void Start()
//    {
//        if (CharacterManager.Instance != null)
//        {
//            targetOffset = CharacterManager.Instance.selectedIndex * spacing;
//            currentOffset = targetOffset;
//        }

//        UpdatePositions(true);
//    }

//    void Update()
//    {
//        HandleDrag();
//        ApplyInertia();
//        UpdatePositions(false);
//    }

//    // ================= DRAG =================
//    void HandleDrag()
//    {
//#if UNITY_EDITOR
//        if (Input.GetMouseButtonDown(0))
//        {
//            isDragging = true;
//            lastTouchPos = Input.mousePosition;
//            velocity = 0;
//        }
//        else if (Input.GetMouseButtonUp(0))
//        {
//            isDragging = false;
//            SnapToNearest();
//        }
//        else if (Input.GetMouseButton(0) && isDragging)
//        {
//            Vector2 currentPos = Input.mousePosition;
//            float delta = currentPos.x - lastTouchPos.x;

//            currentOffset -= delta;
//            velocity = delta;

//            lastTouchPos = currentPos;
//        }
//#else
//        if (Input.touchCount == 0) return;

//        Touch touch = Input.GetTouch(0);

//        if (touch.phase == TouchPhase.Began)
//        {
//            isDragging = true;
//            lastTouchPos = touch.position;
//            velocity = 0;
//        }
//        else if (touch.phase == TouchPhase.Moved)
//        {
//            float delta = touch.position.x - lastTouchPos.x;

//            currentOffset -= delta;
//            velocity = delta;

//            lastTouchPos = touch.position;
//        }
//        else if (touch.phase == TouchPhase.Ended)
//        {
//            isDragging = false;
//            SnapToNearest();
//        }
//#endif
//    }

//    // ================= INERTIA =================
//    void ApplyInertia()
//    {
//        if (isDragging) return;

//        if (Mathf.Abs(velocity) > 0.1f)
//        {
//            currentOffset -= velocity * velocityMultiplier;
//            velocity = Mathf.Lerp(velocity, 0, Time.deltaTime * deceleration);
//        }
//    }

//    // ================= SNAP =================
//    void SnapToNearest()
//    {
//        int index = Mathf.RoundToInt(currentOffset / spacing);
//        index = Mathf.Clamp(index, 0, characters.Length - 1);

//        targetOffset = index * spacing;

//        CharacterManager.Instance.SelectCharacter(index);

//        StartCoroutine(BounceEffect());
//    }

//    public void MoveToIndex(int index)
//    {
//        if (index < 0 || index >= characters.Length) return;

//        targetOffset = index * spacing;

//        CharacterManager.Instance.SelectCharacter(index);

//        StartCoroutine(BounceEffect());
//    }

//    // ================= BOUNCE =================
//    IEnumerator BounceEffect()
//    {
//        float duration = 0.15f;
//        float time = 0;

//        float overshoot = 40f;

//        float start = currentOffset;
//        float end = targetOffset;

//        while (time < duration)
//        {
//            time += Time.deltaTime;

//            float t = time / duration;
//            float ease = Mathf.Sin(t * Mathf.PI);

//            currentOffset = Mathf.Lerp(start, end, t) + overshoot * ease;

//            yield return null;
//        }

//        currentOffset = targetOffset;
//    }

//    // ================= POSITION =================
//    void UpdatePositions(bool instant)
//    {
//        if (!isDragging)
//        {
//            currentOffset = Mathf.Lerp(currentOffset, targetOffset, Time.deltaTime * moveSpeed);
//        }

//        for (int i = 0; i < characters.Length; i++)
//        {
//            float x = (i * spacing) - currentOffset;
//            characters[i].localPosition = new Vector3(x, 0, 0);

//            float distance = Mathf.Abs(x) / spacing;
//            float scale = Mathf.Lerp(selectedScale, normalScale, distance);

//            characters[i].localScale = Vector3.Lerp(
//                characters[i].localScale,
//                Vector3.one * scale,
//                Time.deltaTime * moveSpeed
//            );
//        }
//    }
//}

using UnityEngine;
using System.Collections;

public class CharacterCarousel : MonoBehaviour
{
    public Transform[] characters;

    public float spacing = 400f;
    public float moveSpeed = 10f;

    public float selectedScale = 1.3f;
    public float normalScale = 0.9f;

    [Header("Inertia")]
    public float deceleration = 5f;
    public float velocityMultiplier = 0.02f;

    float currentOffset = 0f;
    float targetOffset = 0f;
    float velocity = 0f;

    bool isDragging = false;
    bool isBouncing = false; // 🔥 FIX 1: Prevents Update & Coroutine from fighting
    Vector2 lastTouchPos;

    void Start()
    {
        if (CharacterManager.Instance != null)
        {
            targetOffset = CharacterManager.Instance.selectedIndex * spacing;
            currentOffset = targetOffset;
        }

        UpdatePositions(true);
    }

    void Update()
    {
        HandleDrag();
        ApplyInertia();

        // 🔥 FIX 2: Only apply the normal smooth follow if the bounce isn't running
        if (!isBouncing)
        {
            UpdatePositions(false);
        }
        else
        {
            // Keep updating the visuals (scale/position) during the bounce
            ApplyVisuals();
        }
    }

    // ================= DRAG =================
    void HandleDrag()
    {
        // 🔥 FIX 3: Unified Input. Unity maps single touches to Mouse 0 automatically.
        // This works seamlessly on PC, WebGL, Android, iOS, and with Bluetooth mice.
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            isBouncing = false; // Cancel bounce if player grabs it mid-animation
            StopAllCoroutines();

            lastTouchPos = Input.mousePosition;
            velocity = 0;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            SnapToNearest();
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 currentPos = Input.mousePosition;
            float delta = currentPos.x - lastTouchPos.x;

            currentOffset -= delta;
            velocity = delta;

            lastTouchPos = currentPos;
        }
    }

    // ================= INERTIA =================
    void ApplyInertia()
    {
        if (isDragging || isBouncing) return;

        if (Mathf.Abs(velocity) > 0.1f)
        {
            currentOffset -= velocity * velocityMultiplier;
            velocity = Mathf.Lerp(velocity, 0, Time.deltaTime * deceleration);
        }
    }

    // ================= SNAP =================
    void SnapToNearest()
    {
        int index = Mathf.RoundToInt(currentOffset / spacing);
        index = Mathf.Clamp(index, 0, characters.Length - 1);

        targetOffset = index * spacing;

        if (CharacterManager.Instance != null)
        {
            CharacterManager.Instance.SelectCharacter(index);
        }

        StartCoroutine(BounceEffect());
    }

    public void MoveToIndex(int index)
    {
        if (index < 0 || index >= characters.Length) return;

        targetOffset = index * spacing;

        if (CharacterManager.Instance != null)
        {
            CharacterManager.Instance.SelectCharacter(index);
        }

        StopAllCoroutines();
        StartCoroutine(BounceEffect());
    }

    // ================= BOUNCE =================
    IEnumerator BounceEffect()
    {
        isBouncing = true;
        float duration = 0.15f;
        float time = 0;

        float overshoot = 40f;
        float start = currentOffset;
        float end = targetOffset;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float ease = Mathf.Sin(t * Mathf.PI);

            currentOffset = Mathf.Lerp(start, end, t) + overshoot * ease;
            yield return null;
        }

        currentOffset = targetOffset;
        isBouncing = false;
    }

    // ================= POSITION =================
    void UpdatePositions(bool instant)
    {
        if (!isDragging)
        {
            currentOffset = Mathf.Lerp(currentOffset, targetOffset, Time.deltaTime * moveSpeed);
        }
        ApplyVisuals();
    }

    void ApplyVisuals()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] == null) continue;

            float x = (i * spacing) - currentOffset;
            characters[i].localPosition = new Vector3(x, 0, 0);

            float distance = Mathf.Abs(x) / spacing;
            float scale = Mathf.Lerp(selectedScale, normalScale, distance);

            characters[i].localScale = Vector3.Lerp(
                characters[i].localScale,
                Vector3.one * scale,
                Time.deltaTime * moveSpeed
            );
        }
    }
}