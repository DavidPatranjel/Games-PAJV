using UnityEngine;
using UnityEngine.Networking;
using TMPro;  // Add this for TextMeshPro
using System;
using System.Collections;

public class HandleWebReq : MonoBehaviour
{
    // The URL of the API endpoint
    private string apiUrl = "https://your-api-endpoint.com/getUserName";

    // Reference to the TMP_Text component to display the username
    public TMP_Text userNameText;

    void Start()
    {
        // Check for command-line arguments
        string[] args = Environment.GetCommandLineArgs();

        if (args.Length > 1)
        {
            string userId = args[1];  // Assuming the user ID is passed as the first argument
            Debug.Log("User ID: " + userId);

            // Make the HTTP request to fetch the authenticated user's name
            StartCoroutine(FetchUserName(userId));
        }
        else
        {
            Debug.LogError("No user ID found in the command line arguments.");
        }
    }

    // Coroutine to fetch the user's name
    private IEnumerator FetchUserName(string userId)
    {
        // Create the URL with the user ID (e.g., https://your-api-endpoint.com/getUserName?userId=123)
        string urlWithParams = apiUrl + "?userId=" + userId;

        UnityWebRequest request = UnityWebRequest.Get(urlWithParams);

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // If the request is successful, get the user name from the response
            string userName = request.downloadHandler.text;
            Debug.Log("Authenticated user name: " + userName);

            // Update the UI text with the username
            if (userNameText != null)
            {
                userNameText.text = "Hello, " + userName + "!";
            }
        }
        else
        {
            // If there is an error, log the message
            Debug.LogError("Request failed: " + request.error);
        }
    }
}
