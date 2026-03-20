using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> executionQueue = new Queue<Action>();
    private static MainThreadDispatcher _instance;

    void Awake()
    {
        // Prevent duplicates if you reload the scene
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // ✅ KEEP ALIVE
    }

    public static void RunOnMainThread(Action action)
    {
        if (action == null) return;

        // Safely lock the queue while adding
        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        // Quick check before doing heavy lifting
        if (executionQueue.Count > 0)
        {
            Action[] actionsToRun;

            // Safely lock the queue to pull everything out
            lock (executionQueue)
            {
                actionsToRun = executionQueue.ToArray();
                executionQueue.Clear();
            }

            // Run the actions OUTSIDE the lock to prevent deadlocks
            for (int i = 0; i < actionsToRun.Length; i++)
            {
                actionsToRun[i]?.Invoke();
            }
        }
    }
}