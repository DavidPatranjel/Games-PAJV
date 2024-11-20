using UnityEngine;
using UnityEngine.Networking;
using TMPro;  // Add this for TextMeshPro
using System;
using System.Collections;

public class HandleWebReq : MonoBehaviour
{
    private const string URL = "https://parseapi.back4app.com/users/me";
    private const string AppId = "";
    private const string ApiKey = "";

    private string sessionToken;

    // Reference to the TMP_Text component to display the username
    public TMP_Text userNameText;

    void Start()
    {
        // Retrieve session token from command-line arguments
        sessionToken = GetSessionTokenFromArgs();

        if (string.IsNullOrEmpty(sessionToken))
        {
            Debug.LogError("Session token not provided. Please pass it via command-line arguments.");
            return;
        }

        // Start the coroutine to fetch user data
        StartCoroutine(GetUserData());
    }

    private string GetSessionTokenFromArgs()
    {
        // Fetch command-line arguments
        string[] args = System.Environment.GetCommandLineArgs();

        // Look for a specific argument format, e.g., --sessionToken=value
        foreach (string arg in args)
        {
            if (arg.StartsWith("--sessionToken="))
            {
                return arg.Substring("--sessionToken=".Length);
            }
        }

        return null;
    }

    private IEnumerator GetUserData()
    {
        UnityWebRequest request = UnityWebRequest.Get(URL);

        // Add necessary headers
        request.SetRequestHeader("X-Parse-Application-Id", AppId);
        request.SetRequestHeader("X-Parse-REST-API-Key", ApiKey);
        request.SetRequestHeader("X-Parse-Session-Token", sessionToken);

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // Parse the JSON response
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("User Data: " + jsonResponse);

            try
            {
                // Deserialize the JSON response to a UserData object
                var userData = JsonUtility.FromJson<UserData>(jsonResponse);

                if (userData != null && !string.IsNullOrEmpty(userData.username))
                {
                    // Set the username in the TMP_Text component
                    userNameText.text = $"Welcome, {userData.username}!";
                }
                else
                {
                    Debug.LogError("Username not found in the response.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to parse JSON response: {ex.Message}");
            }
        }
    }


}


[System.Serializable]
public class UserData
{
    public string username;       // Maps the "username" field
    public string myCustomKeyName; // Maps the "myCustomKeyName" field
    public string createdAt;      // Maps the "createdAt" field
    public string updatedAt;      // Maps the "updatedAt" field
    public string objectId;       // Maps the "objectId" field
    public string sessionToken;   // Maps the "sessionToken" field
}
