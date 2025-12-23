using UnityEngine;
using System;
using System.Collections.Generic;

// This class allows executing actions on the main Unity thread from background threads
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher instance;
    private static Queue<Action> actionQueue = new Queue<Action>();

    public static void EnqueueAction(Action action)
    {
        lock (actionQueue)
        {
            actionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        // Process action queue on the main Unity thread
        lock (actionQueue)
        {
            while (actionQueue.Count > 0)
            {
                Action action = actionQueue.Dequeue();
                action?.Invoke();
            }
        }
    }

    // Ensure only one instance of UnityMainThreadDispatcher exists
    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UnityMainThreadDispatcher>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("UnityMainThreadDispatcher");
                    instance = obj.AddComponent<UnityMainThreadDispatcher>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }
}
