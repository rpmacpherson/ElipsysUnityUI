using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class FrameReceiver : MonoBehaviour
{
    public RawImage rawImage1, rawImage2;
    public string serverUrl = "http://localhost:8080/video_feed"; // Update with your server URL

    private Coroutine fetchFrameCoroutine;

    void Start()
    {
        // Start retrieving frames from the server
        fetchFrameCoroutine = StartCoroutine(GetFrame());
    }

    void OnApplicationQuit()
    {
        // Stop the coroutine when the application quits
        if (fetchFrameCoroutine != null)
            StopCoroutine(fetchFrameCoroutine);
    }

    IEnumerator GetFrame()
    {
        while (true)
        {
            // Debug message for sending request
            Debug.Log("Sending request to: " + serverUrl);

            // Create a UnityWebRequest to make a GET request to the server
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(serverUrl))
            {
                // Send the request and wait for a response
                yield return www.SendWebRequest();

                // Check for errors
                if (www.result == UnityWebRequest.Result.Success)
                {
                    // Check the content type of the response
                    string contentType = www.GetResponseHeader("Content-Type");
                    if (contentType.StartsWith("image"))
                    {
                        // Get the downloaded texture from the web request
                        Texture2D texture = DownloadHandlerTexture.GetContent(www);

                        // Apply the texture to the RawImage component on the main thread
                        ApplyTextureToRawImage(texture);
                    }
                    else
                    {
                        Debug.LogError("Invalid content type received: " + contentType);
                    }
                }
                else
                {
                    Debug.LogError("Error fetching frame from " + serverUrl + ": " + www.error);
                }
            }

            // Pause for a short duration before fetching the next frame
            yield return new WaitForSeconds(1f/30f);
        }
    }

    void ApplyTextureToRawImage(Texture2D texture)
    {
        // Ensure that the texture is applied on the main thread
        if (rawImage1 != null)
        {
            // Set the texture to the RawImage component
            rawImage1.texture = texture;
        }
        if (rawImage2 != null)
        {
            // Set the texture to the RawImage component
            rawImage2.texture = texture;
        }
    }
}
