//using UnityEngine;
//using System.Collections;

//public class AnswerSpawner : MonoBehaviour
//{
//    public Transform parent;

//    private float[] lanes = { -300f, 0f, 300f };

//    void Start()
//    {
//        StartCoroutine(DelayedStart());
//    }

//    IEnumerator DelayedStart()
//    {
//        yield return new WaitForSecondsRealtime(0.2f);
//        SpawnSet();
//    }

//    // ================= MAIN SPAWN =================

//    public void SpawnSet()
//    {
//        if (GameManager.Instance == null) return;
//        GameManager.Instance.isSpawningNow = true;

//        GameManager.Instance.lifeHandled = false;
//        GameManager.Instance.isAnswered = false;

//        if (GameManager.Instance.isGameOver && !GameManager.Instance.isReviving)
//        {
//            GameManager.Instance.isSpawningNow = false;
//            return;
//        }

//        ClearAll();

//        GameManager.Instance.GenerateQuestion();

//        GameObject selectedPrefab = CharacterManager.Instance.GetSelectedCharacter().prefab;

//        for (int i = 0; i < 3; i++)
//        {
//            GameObject obj = Instantiate(selectedPrefab, parent);

//            obj.GetComponent<AnswerBox>().Setup(i);

//            RectTransform rt = obj.GetComponent<RectTransform>();
//            rt.localScale = Vector3.one;
//            rt.anchoredPosition = new Vector2(lanes[i], 900f);

//            obj.AddComponent<AnswerPop>();
//        }

//        StartCoroutine(UnlockSpawn());
//        GameManager.Instance.spawnRequested = false; // 🔥 IMPORTANT
//    }
//    IEnumerator UnlockSpawn()
//    {
//        yield return new WaitForSecondsRealtime(0.2f);

//        if (GameManager.Instance != null)
//            GameManager.Instance.isSpawningNow = false;
//    }
//    // ================= SAFE RESPAWN =================

//    public void ClearAndSpawnNew()
//    {
//        if (GameManager.Instance == null) return;

//        if (GameManager.Instance.isGameOver && !GameManager.Instance.isReviving)
//            return;

//        if (GameManager.Instance.isSpawningNow) return; // 🔥 ADD THIS

//        StartCoroutine(SpawnRoutine());
//    }

//    IEnumerator SpawnRoutine()
//    {
//        if (GameManager.Instance.isGameOver) yield break;

//        // 🔥 Add crunch effect before removing
//        foreach (Transform child in parent)
//        {
//            if (!child.GetComponent<AnswerCrunch>())
//                child.gameObject.AddComponent<AnswerCrunch>();
//        }

//        // 🔥 Always realtime (ignore pause issues)
//        yield return new WaitForSecondsRealtime(0.25f);

//        if (GameManager.Instance.isGameOver) yield break;

//        SpawnSet();
//    }


//    // ================= CLEAR =================

//    public void ClearAll()
//    {
//        foreach (Transform child in parent)
//        {

//            // 🔥 small delay avoids frame conflict
//            Destroy(child.gameObject, 0.05f);
//        }
//    }
//}

////////deep ai 
///
using UnityEngine;
using System.Collections;

public class AnswerSpawner : MonoBehaviour
{
    public Transform parent;
    private float[] lanes = { -300f, 0f, 300f };
    private bool respawnRequested = false;   // ← NEW

    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        SpawnSet();
    }

    public void SpawnSet()
    {
        if (GameManager.Instance.isSpawningNow) return;
        GameManager.Instance.isSpawningNow = true;

        respawnRequested = false;   // ← NEW

        if (GameManager.Instance == null) return;

        GameManager.Instance.lifeHandled = false;
        GameManager.Instance.isAnswered = false;

        if (GameManager.Instance.isGameOver && !GameManager.Instance.isReviving)
        {
            GameManager.Instance.isSpawningNow = false;
            return;
        }

        ClearAll();
        GameManager.Instance.GenerateQuestion();

        GameObject selectedPrefab = CharacterManager.Instance.GetSelectedCharacter().prefab;

        for (int i = 0; i < 3; i++)
        {
            GameObject obj = Instantiate(selectedPrefab, parent);
            obj.GetComponent<AnswerBox>().Setup(i);
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.anchoredPosition = new Vector2(lanes[i], 900f);
            obj.AddComponent<AnswerPop>();
        }

        StartCoroutine(UnlockSpawn());
        GameManager.Instance.spawnRequested = false;
    }

    IEnumerator UnlockSpawn()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        GameManager.Instance.isSpawningNow = false;

        // ← NEW: handle pending spawn request
        if (respawnRequested)
        {
            respawnRequested = false;
            ClearAndSpawnNew();
        }
    }

    public void ClearAndSpawnNew()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isGameOver && !GameManager.Instance.isReviving) return;

        // ← NEW: queue request if already spawning
        if (GameManager.Instance.isSpawningNow)
        {
            respawnRequested = true;
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        if (GameManager.Instance.isGameOver) yield break;

        foreach (Transform child in parent)
        {
            if (!child.GetComponent<AnswerCrunch>())
                child.gameObject.AddComponent<AnswerCrunch>();
        }

        yield return new WaitForSecondsRealtime(0.25f);

        if (GameManager.Instance.isGameOver) yield break;

        SpawnSet();
    }

    public void ClearAll()
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject, 0.05f);
        }
    }
}