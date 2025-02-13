using Firebase;
using Firebase.Database;
using TMPro;
using UnityEngine;
using Firebase.Extensions;

public class DatabaseManager : MonoBehaviour
{
    public string Name;               // Player's name
    public TextMeshProUGUI Score;      // UI Text showing the player's score
    private string userID;             // Unique device ID for the user
    private DatabaseReference dbReference; // Reference to the Firebase database

    private float startTime;           // Time tracking when the game starts
    private float completionTime;      // Time taken to finish the game

    void Start()
    {
        // Initialize Firebase and check for required dependencies
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            // If Firebase is initialized correctly
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase initialized successfully!");

                // Get a reference to the root of the Firebase database
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;

                // Assign the unique device ID to userID (used to identify the player)
                userID = SystemInfo.deviceUniqueIdentifier;
            }
            else
            {
                // Log error if Firebase dependencies could not be resolved
                Debug.LogError($"Could not resolve Firebase dependencies: {task.Result}");
            }
        });

        // Start tracking the time when the game begins
        startTime = Time.time;
    }
}
