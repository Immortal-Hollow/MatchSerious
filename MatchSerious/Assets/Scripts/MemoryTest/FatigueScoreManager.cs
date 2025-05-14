using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

public class FatigueScoreManager : MonoBehaviour
{
    // Singleton instance
    public static FatigueScoreManager Instance { get; private set; }

    // Total fatigue score
    private float totalFatigueScore;

    // UI references
    [SerializeField] private TextMeshProUGUI totalFatigueText;

    // Reference to Firebase Database
    private DatabaseReference dbReference;

    private void Awake()
    {
        // Ensure there is only one instance of FatigueScoreManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize Firebase
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase initialized successfully");
            }
            else
            {
                Debug.LogError($"Firebase initialization failed: {task.Result}");
            }
        });
    }

    private void Start()
    {
        // Optionally, load previous score if you want to accumulate it across sessions
        totalFatigueScore = PlayerPrefs.GetFloat("TotalFatigueScore", 0f);
        Debug.Log($"Loaded fatigue score from PlayerPrefs: {totalFatigueScore}");
        UpdateFatigueText();
    }

    // Method to add a fatigue score from another script 
    public void AddFatigueScore(float fatigueScore)
    {
        totalFatigueScore += fatigueScore;

        // Update UI
        UpdateFatigueText();

        // Save total fatigue score to PlayerPrefs (for persistence)
        PlayerPrefs.SetFloat("TotalFatigueScore", totalFatigueScore);
        PlayerPrefs.Save();
        Debug.Log($"Updated total fatigue score: {totalFatigueScore}");

        // Store score to Firebase (async)
        StoreTotalFatigueInFirebase();
    }

    // Method to update the total fatigue score UI
    private void UpdateFatigueText()
    {
        if (totalFatigueText != null)
        {
            totalFatigueText.text = totalFatigueScore.ToString("F2");
            Debug.Log($"UI updated with new score: {totalFatigueScore}");
        }
        else
        {
            Debug.LogWarning("TotalFatigueText is not assigned in the inspector.");
        }
    }

    // Method to store the total fatigue score in Firebase
    public void StoreTotalFatigueInFirebase()
    {
        if (dbReference == null)
        {
            Debug.LogError("Firebase reference is not initialized.");
            return;
        }

        string userID = SystemInfo.deviceUniqueIdentifier;
        UserFatigueData userFatigueData = new UserFatigueData(totalFatigueScore);

        string json = JsonUtility.ToJson(userFatigueData);

        dbReference.Child("users").Child(userID).Child("fatigue").SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Total fatigue score successfully saved to Firebase!");
            }
            else
            {
                Debug.LogError($"Failed to save total fatigue score to Firebase: {task.Exception}");
            }
        });
    }

    // Method to reset the total fatigue score (if needed)
    public void ResetFatigueScore()
    {
        totalFatigueScore = 0f;
        PlayerPrefs.SetFloat("TotalFatigueScore", totalFatigueScore);
        PlayerPrefs.Save();
        UpdateFatigueText();
        StoreTotalFatigueInFirebase(); // Ensure it resets on Firebase as well
    }
}

[System.Serializable]
public class UserFatigueData
{
    public float totalFatigueScore;

    public UserFatigueData(float totalFatigueScore)
    {
        this.totalFatigueScore = totalFatigueScore;
    }
}
