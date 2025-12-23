using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IEnumeratorTrigger : MonoBehaviour
{
    public UnityEvent OnWaitComplete;
    public float waitTime;
    public bool waitOnAwake;

    public void StartWaiting()
    {
        StartCoroutine(WaitToExecute(waitTime));
    }
    IEnumerator WaitToExecute(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        OnWaitComplete?.Invoke();
    }
    public void Awake()
    {
        if (waitOnAwake) StartWaiting();
    }
}
