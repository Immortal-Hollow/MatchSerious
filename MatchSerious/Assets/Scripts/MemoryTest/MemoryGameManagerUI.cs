using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class MemoryGameManagerUI : MinigamesBase
{
    public static MemoryGameManagerUI Instance { get; private set; }

    // UI references for managing the game cards
    [SerializeField] private CardGroup cardGroup;
    [SerializeField] private List<CardSingleUI> cardSingleUIList = new List<CardSingleUI>();

    // UI references for displaying the completion time, best time, and fatigue score
    [SerializeField] private TextMeshProUGUI completionTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI fatigueScoreText;  // Fatigue score text reference
    [SerializeField] private TextMeshProUGUI totalFatigueText;   // Total accumulated fatigue score reference

    // Reference for the back button
    [SerializeField] private Button backButton;

    // Reference for the AudioSource to play sound effects
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip victorySound;

    private float gameStartTime;  // Store the time when the game starts

    // Reference to the Firebase Database Manager
    private DatabaseReference dbReference;

    private void Awake()
    {
        Instance = this;  // Ensures there is only one instance of this class (singleton)
    }

    private void Start()
    {
        // Subscribe to card match event
        cardGroup.OnCardMatch += CardGroup_OnCardMatch;

        // Initially hide the back button
        backButton.gameObject.SetActive(false);

        // Set up the back button functionality
        backButton.onClick.AddListener(BackToMenu);

        // Initialize Firebase database reference
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void OnEnable()
    {
        // Initialize the game setup when the UI is enabled
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForSeconds(0.1f);

        // Set up the difficulty button listeners
        DifficultyManager.Instance
            .ResetListeners()  // Reset any previous listeners
            .OnEasyButtonClick(() =>
            {
                DifficultyManager.Instance.Toggle(false);
                ToggleGameArea(true);  // Show game area
                StartGame();  // Start the game
            })
            .OnNormalButtonClick(() =>
            {
                DifficultyManager.Instance.Toggle(false);
                ToggleGameArea(true);
                StartGame();
            })
            .OnHardButtonClick(() =>
            {
                DifficultyManager.Instance.Toggle(false);
                ToggleGameArea(true);
                StartGame();
            });
    }

    private void StartGame()
    {
        // Record the start time when the game begins
        gameStartTime = Time.time;
    }

    // Subscribe to card match events
    public void Subscribe(CardSingleUI cardSingleUI)
    {
        if (cardSingleUIList == null)
        {
            cardSingleUIList = new List<CardSingleUI>();  // Initialize if null
        }

        if (!cardSingleUIList.Contains(cardSingleUI))
        {
            cardSingleUIList.Add(cardSingleUI);  // Add card if not already present
        }
    }

    // Event handler when a match is found between cards
    private void CardGroup_OnCardMatch(object sender, System.EventArgs e)
    {
        // Check if all cards have been matched
        if (cardSingleUIList.All(x => x.GetObjectMatch()))
        {
            StartCoroutine(OnCompleteGame());  // Start the completion process if all cards match
        }
    }

    private IEnumerator OnCompleteGame()
    {
        yield return new WaitForSeconds(0.75f);  // Wait before processing game completion

        // Calculate the time taken to complete the game
        float completionTime = Time.time - gameStartTime;

        // Calculate the fatigue score based on completion time
        float fatigueScore = Mathf.Clamp(100f - (completionTime / 10f), 0f, 100f);

        // Save the best time if it's the player's new record
        float bestTime = PlayerPrefs.GetFloat("BestCompletionTime", float.MaxValue);
        if (completionTime < bestTime)
        {
            PlayerPrefs.SetFloat("BestCompletionTime", completionTime);
            PlayerPrefs.Save();  // Save the new best time
        }

        // Update UI with the completion time and best time
        completionTimeText.text = completionTime.ToString("F2") + " secs";
        bestTime = PlayerPrefs.GetFloat("BestCompletionTime", float.MaxValue);
        bestTimeText.text = bestTime == float.MaxValue ? "N/A" : bestTime.ToString("F2") + " secs";

        // Display the calculated fatigue score
        fatigueScoreText.text = fatigueScore.ToString("F2");

        // Update and display the total accumulated fatigue score
        float totalFatigue = PlayerPrefs.GetFloat("TotalFatigueScore", 0f);
        totalFatigue += fatigueScore;
        PlayerPrefs.SetFloat("TotalFatigueScore", totalFatigue);
        PlayerPrefs.Save();  // Save the updated total fatigue score
        totalFatigueText.text = totalFatigue.ToString("F2");

        // Play victory sound effect if available
        if (audioSource != null && victorySound != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        // Show the back button after the game finishes
        backButton.gameObject.SetActive(true);

        // Store the completion time and fatigue score in Firebase
        StoreScoreInFirebase(completionTime, fatigueScore);
    }

    // Method to store the completion time and fatigue score in Firebase
    private void StoreScoreInFirebase(float time, float fatigueScore)
    {
        string userID = SystemInfo.deviceUniqueIdentifier;  // Get the unique device ID
        User newUser = new User("Player", time, fatigueScore);  // Create a new user object

        string json = JsonUtility.ToJson(newUser);  // Convert the user object to JSON format

        // Store the user data in the Firebase database
        dbReference.Child("users").Child(userID).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Completion time and fatigue score added successfully to Firebase!");
            }
            else
            {
                Debug.LogError($"Failed to add data to Firebase: {task.Exception}");
            }
        });
    }

    // Method to go back to the main menu
    public void BackToMenu()
    {
        SceneManager.LoadScene("SelectGamemode");  // Load the scene for selecting game modes
    }

    // Method to restart the game (clears the card list)
    public void Restart()
    {
        cardSingleUIList.Clear();
    }
}
