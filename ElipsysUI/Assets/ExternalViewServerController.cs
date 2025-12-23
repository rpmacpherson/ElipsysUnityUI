using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ExternalViewServerController : MonoBehaviour
{
    private const string ServerUrl = "http://localhost:8080"; // Update with your server URL

    [SerializeField]
    bool drawBoundingBoxes = false;
    [SerializeField]
    bool drawPoseAnnotations = false;
    [SerializeField]
    public bool recording = false;
    [SerializeField]
    string participantName;
    [SerializeField]
    string test_newFilename;

    public int shotCount = -1;

    private void Start()
    {

        StartCoroutine(QueryDrawBoundingBoxesState());
        StartCoroutine(QueryDrawPoseAnnotationsState());
        StartCoroutine(QueryRecordingState());
        StartCoroutine(QueryShotCount());
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
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:8080/get_bounding_box_state");
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

    private IEnumerator QueryDrawPoseAnnotationsState(float delay = 0)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:8080/get_pose_annotation_state");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            
            string response = request.downloadHandler.text;
            print(response);
            drawPoseAnnotations = response.Equals("True", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            Debug.LogError("Error querying pose annotation state: " + request.error);
        }
    }

    private IEnumerator QueryRecordingState(float delay = 0)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:8080/get_recording_state");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            string response = request.downloadHandler.text;
            print(response);
            recording = response.Equals("True", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            Debug.LogError("Error querying recording state: " + request.error);
        }
    }

    private IEnumerator QueryShotCount(float delay = 0)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        UnityWebRequest request = UnityWebRequest.Get(ServerUrl + "/get_shot_count");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            int response = Convert.ToInt32(request.downloadHandler.text);
            shotCount = response;
        }
        else
        {
            Debug.LogError("Error querying shot count: " + request.error);
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
        StartCoroutine(SendRequest("/set_bounding_box", "{\"bounding_box\": " + (!drawBoundingBoxes ? "true" : "false") + "}"));
        StartCoroutine(QueryDrawBoundingBoxesState(.1f));
    }

    [ContextMenu("Toggle Pose Annotation")]
    public void SetPoseAnnotation()
    {
        StartCoroutine(SendRequest("/set_pose_annotation", "{\"pose_annotation\": " + (!drawPoseAnnotations ? "true" : "false") + "}"));
        StartCoroutine(QueryDrawPoseAnnotationsState(.1f));
    }

    public void SetShooterRect(Vector2 startPoint, Vector2 endPoint)
    {
        StartCoroutine(SendRequest("/set_shooter_rect", "{\"startPoint\": [" + startPoint.x + ", " + startPoint.y + "], \"endPoint\": [" + endPoint.x + ", " + endPoint.y + "]}"));
    }

    public void SetBasketRect(Vector2 startPoint, Vector2 endPoint)
    {
        StartCoroutine(SendRequest("/set_basket_rect", "{\"startPoint\": [" + startPoint.x + ", " + startPoint.y + "], \"endPoint\": [" + endPoint.x + ", " + endPoint.y + "]}"));
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

}
