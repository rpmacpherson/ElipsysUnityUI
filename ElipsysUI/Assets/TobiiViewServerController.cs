using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class TobiiViewServerController : MonoBehaviour
{
    private const string ServerUrl = "http://localhost:8081"; // Update with your server URL

    [SerializeField]
    bool drawBoundingBoxes = false;
    [SerializeField]
    bool drawGazeAnnotations = false;
    [SerializeField]
    public bool recording = false;
    [SerializeField]
    public float batteryLevel = -1;
    [SerializeField]
    CalibrationState calibrationState = CalibrationState.UNCALIBRATED;
    [SerializeField]
    string participantName;

    [SerializeField]
    string test_newFilename;

    [SerializeField]
    UnityEvent OnBatteryChanged;
    private void Start()
    {
        // Query all the state variables to cache them within the app
        StartCoroutine(QueryDrawBoundingBoxesState());
        StartCoroutine(QueryDrawGazeAnnotationsState());
        StartCoroutine(QueryRecordingState());
        StartCoroutine(QueryBatteryState());
        StartCoroutine(QueryCalibrationState());
        StartCoroutine(QueryFilename());
    }

    [ContextMenu("Start Recording")]
    public void StartRecording()
    {
        StartCoroutine(SendStartRecordingRequest());
        StartCoroutine(QueryRecordingState());
    }

    [ContextMenu("Stop Recording")]
    public void StopRecording()
    {
        StartCoroutine(SendStopRecordingRequest());
        StartCoroutine(QueryRecordingState());
    }

    private IEnumerator SendStartRecordingRequest(float delay = 0)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        UnityWebRequest www = UnityWebRequest.PostWwwForm(ServerUrl + "/start_recording", "");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error starting recording: " + www.error);
        }
        else
        {
            Debug.Log("Recording started successfully.");
        }
    }

    private IEnumerator SendStopRecordingRequest(float delay = 0)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        UnityWebRequest www = UnityWebRequest.PostWwwForm(ServerUrl + "/stop_recording", "");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error stopping recording: " + www.error);
        }
        else
        {
            Debug.Log("Recording stopped successfully.");
        }
    }

    private IEnumerator QueryDrawBoundingBoxesState(float delay = 0)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        UnityWebRequest request = UnityWebRequest.Get(ServerUrl + "/get_bounding_box_state");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            print(response);
            drawBoundingBoxes = response.Equals("True", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            Debug.LogError("Error querying bounding box state: " + request.error);
        }
    }

    private IEnumerator QueryDrawGazeAnnotationsState(float delay = 0)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        UnityWebRequest request = UnityWebRequest.Get(ServerUrl + "/get_gaze_annotation_state");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            string response = request.downloadHandler.text;
            print(response);
            drawGazeAnnotations = response.Equals("True", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            Debug.LogError("Error querying gaze annotation state: " + request.error);
        }
    }

    private IEnumerator QueryRecordingState(float delay = 0)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        UnityWebRequest request = UnityWebRequest.Get(ServerUrl + "/get_recording_state");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            string response = request.downloadHandler.text;
            print(response);
            recording = response.Equals("True", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            Debug.LogError("Error querying pose annotation state: " + request.error);
        }
    }

    private IEnumerator QueryBatteryState(float delay = 0)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        UnityWebRequest request = UnityWebRequest.Get(ServerUrl + "/get_battery_state");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            float response = Convert.ToSingle(request.downloadHandler.text);
            print(response);
            batteryLevel = response;
            OnBatteryChanged?.Invoke();
        }
        else
        {
            Debug.LogError("Error querying battery state: " + request.error);
        }
    }

    [ContextMenu("Debug_BatteryLevel")]
    public void Debug_SetBatteryLevel()
    {
        batteryLevel = UnityEngine.Random.Range(0f, 1f);
        OnBatteryChanged?.Invoke();
    }


    private IEnumerator QueryCalibrationState(float delay = 0)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        UnityWebRequest request = UnityWebRequest.Get(ServerUrl + "/get_calibration_state");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            string response = request.downloadHandler.text.Replace("CalibrationState.","");
            print(response);
            Enum.TryParse(response, out calibrationState);
        }
        else
        {
            Debug.LogError("Error querying calibration state: " + request.error);
        }
    }

    private IEnumerator QueryFilename(float delay = 0)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        UnityWebRequest request = UnityWebRequest.Get(ServerUrl + "/get_filename");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            participantName = request.downloadHandler.text;
        }
        else
        {
            Debug.LogError("Error querying filename: " + request.error);
        }
    }

    [ContextMenu("Calibrate")]
    public void Calibrate()
    {
        StartCoroutine(SendRequest("/calibrate", "{}"));
        StartCoroutine(QueryCalibrationState(3f));
    }
    [ContextMenu("Set Filename")]
    public void Test_SetFilename()
    {
        SetFilename(test_newFilename);
    }
    public void SetFilename(string name)
    {
        StartCoroutine(SendRequest("/set_filename", "{\"filename\": \"" + name + "\"}"));
        StartCoroutine(QueryFilename());
    }

    [ContextMenu("Toggle BB Annotation")]
    public void SetBoundingBoxAnnotation()
    {
        StartCoroutine(SendRequest("/set_bounding_box", "{\"bounding_box_annotation\": " + (!drawBoundingBoxes ? "true" : "false") + "}"));
        StartCoroutine(QueryDrawBoundingBoxesState(.1f));
    }

    [ContextMenu("Toggle Gaze Annotation")]
    public void SetGazeAnnotation()
    {
        StartCoroutine(SendRequest("/set_gaze_annotation", "{\"gaze_annotation\": " + (!drawGazeAnnotations ? "true" : "false") + "}"));
        StartCoroutine(QueryDrawGazeAnnotationsState(.1f));
    }


    private IEnumerator SendRequest(string endpoint, string jsonData)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(ServerUrl + endpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error sending request: " + request.error);
        }
        else
        {
            Debug.Log("Request sent successfully.");
        }
    }

    public enum CalibrationState
    {
        UNCALIBRATED = 1,
        CALIBRATING = 2,
        SUCCESS = 3,
        FAILURE = 4
    }

}
