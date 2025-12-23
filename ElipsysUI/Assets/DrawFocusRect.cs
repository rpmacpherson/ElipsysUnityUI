using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class DrawFocusRect : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RawImage rawImage; // Reference to the UI Image representing the image frame
    public RectTransform boundingBoxPrefab; // Reference to the bounding box prefab

    private RectTransform boundingBox; // Reference to the currently drawn bounding box
    private Vector2 startPoint; // Start point of the bounding box
    private Vector2 endPoint; // End point of the bounding box

    [SerializeField]
    public bool boundingBoxMode = false; // Flag to indicate if bounding box mode is enabled

    [SerializeField]
    private bool inPersonMode = true;

    [SerializeField]
    private float serverUpdateInterval = .1f;
    
    private float timeSinceLastServerUpdate = 0;

    public void SetPersonMode()
    {
        inPersonMode = true;
    }
    public void SetGoalMode()
    {
        inPersonMode = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Check if bounding box mode is enabled
        if (!boundingBoxMode)
            return;

        // Set the start point of the bounding box
        startPoint = eventData.position;

        // Create a new bounding box instance
        //boundingBox = Instantiate(boundingBoxPrefab, rawImage.transform);
        //boundingBox.anchoredPosition = startPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        // Check if bounding box mode is enabled
        if (!boundingBoxMode)
            return;

        timeSinceLastServerUpdate += Time.deltaTime;
        // Update the end point of the bounding box while dragging
        endPoint = eventData.position;

        if (timeSinceLastServerUpdate > serverUpdateInterval)
        {
            timeSinceLastServerUpdate = 0;
            SendBoundingBoxToServer();
        }

        // Calculate the size of the bounding box
        //Vector2 size = endPoint - startPoint;

        // Update the size of the bounding box RectTransform
        //boundingBox.sizeDelta = size;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Check if bounding box mode is enabled
        if (!boundingBoxMode)
            return;
        endPoint = eventData.position;
        // Send the bounding box coordinates to the server
        SendBoundingBoxToServer();
        timeSinceLastServerUpdate = 0;
    }

    Vector2 AdjustBoundingBoxCoordinates(Vector2 startPoint, Vector2 endPoint, RectTransform rawImageRectTransform)
    {
        // Calculate the inverse scale factor
        Vector2 inverseScale = new Vector2(1f / rawImageRectTransform.localScale.x, 1f / rawImageRectTransform.localScale.y);

        // Adjust the bounding box coordinates based on the inverse scale factor
        Vector2 adjustedStartPoint = Vector2.Scale(startPoint, inverseScale);
        Vector2 adjustedEndPoint = Vector2.Scale(endPoint, inverseScale);

        return adjustedStartPoint;
    }




    private void SendBoundingBoxToServer()
    {
        // Convert the bounding box coordinates to pixel values
        float x1 = startPoint.x / Screen.width * 1920;
        float y1 = startPoint.y / Screen.height * 1080;
        float x2 = endPoint.x / Screen.width * 1920;
        float y2 = endPoint.y / Screen.height * 1080;

        // Send a POST request to the server with the bounding box coordinates
        StartCoroutine(SendBoundingBoxRequest(x1, y1, x2, y2));
    }

    private IEnumerator SendBoundingBoxRequest(float x1, float y1, float x2, float y2)
    {
        // Construct the JSON data with bounding box coordinates
        
        
        // Create a UnityWebRequest with the JSON data
        if (inPersonMode)
        {
            string json = "{\"shooterRect\": [" + x1 + ", " + (1080 - y1) + ", " + x2 + ", " + (1080 - y2) + "]}";
            print(json);
            UnityWebRequest request = new UnityWebRequest("http://localhost:8080/set_shooter_rect", "POST");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request and wait for the response
            yield return request.SendWebRequest();

            // Check for errors
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error sending bounding box request: " + request.error);
            }
            else
            {
                Debug.Log("Bounding box sent successfully.");
            }
        }
        else
        {
            string json = "{\"basketRect\": [" + x1 + ", " + (1080 - y1) + ", " + x2 + ", " + (1080 - y2) + "]}";
            print(json);
            UnityWebRequest request = new UnityWebRequest("http://localhost:8080/set_basket_rect", "POST");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request and wait for the response
            yield return request.SendWebRequest();

            // Check for errors
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error sending bounding box request: " + request.error);
            }
            else
            {
                Debug.Log("Bounding box sent successfully.");
            }
        }
        
    }

    // Function to enable/disable bounding box mode
    public void ToggleBoundingBoxMode(bool enabled)
    {
        boundingBoxMode = enabled;
    }
}
