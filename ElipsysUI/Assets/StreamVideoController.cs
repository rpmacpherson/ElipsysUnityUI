using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamVideoController : MonoBehaviour
{


    [SerializeField]
    bool recording;

    [SerializeField]
    bool awaitingStart = false;

    [SerializeField]
    bool awaitingStop = false;

    [SerializeField]
    ExternalViewServerController externalServer;
    [SerializeField]
    TobiiViewServerController tobiiServer;

    [SerializeField]
    TMPro.TMP_Text recording_indicator;
    [SerializeField]
    float recording_time;

    public void StartRecording()
    {
        if (!awaitingStart)
        {
            externalServer.StartRecording();
            tobiiServer.StartRecording();
            awaitingStart = true;
        }

    }

    public void StopRecoreding()
    {
        externalServer.StopRecording();
        tobiiServer.StopRecording();
        awaitingStop = true;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (awaitingStart)
        {
            if (externalServer.recording && tobiiServer.recording)
            {
                awaitingStart = false;
                recording_indicator.gameObject.SetActive(true);
            }

        }
        if (awaitingStop)
        {
            if (!externalServer.recording && !tobiiServer.recording)
            {
                awaitingStop = false;
                recording_indicator.gameObject.SetActive(false);
            }
        }
    }
}
