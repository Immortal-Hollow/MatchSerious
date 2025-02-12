using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This is an abstract base class for minigames, containing common functionality and UI interactions.
public abstract class MinigamesBase : MonoBehaviour
{
    // UI Elements for the minigame
    [Header("Minigame base")]
    [SerializeField] protected GameObject gameArea;  // Game Area UI element (the area where the minigame takes place)

    // Buttons for player interaction (e.g., to confirm or cancel actions)
    [SerializeField] private Button negativeButton;  // Button for negative/exit action
    [SerializeField] private Button positiveButton;  // Button for positive/accept action

    // Toggles the visibility of the entire Minigame object
    protected void Toggle(bool toggle)
    {
        gameObject.SetActive(toggle);  // Enables or disables the whole Minigame object
    }

    // Toggles the visibility of the Game Area
    protected void ToggleGameArea(bool toggle)
    {
        gameArea.SetActive(toggle);  // Enables or disables the game area UI element
    }

    // Handles actions when the player wins the minigame
    protected virtual void GameWin(int currencyValue, string titleText, string descriptionText, System.Action onNegative = null, System.Action onPositive = null)
    {
        // Logs the win event with relevant details
        Debug.Log($"{titleText} {descriptionText} {currencyValue}");

        // Adds a listener for the negative button (exit or cancel)
        negativeButton?.onClick.AddListener(() =>
        {
            onNegative?.Invoke();  // Invoke the negative action callback if provided
            Toggle(true);  // Show the base minigame object again
            ToggleGameArea(false);  // Hide the game area
            DifficultyManager.Instance.Toggle(true);  // Enable difficulty management or show some other UI elements
        });

        // Adds a listener for the positive button (e.g., confirm or continue)
        positiveButton?.onClick.AddListener(() =>
        {
            onPositive?.Invoke();  // Invoke the positive action callback if provided
            Toggle(false);  // Hide the base minigame object
            ToggleGameArea(false);  // Hide the game area
            DifficultyManager.Instance.HideCloseButton();  // Hide the close button (perhaps after completing the minigame)
        });
    }

    // Handles actions when the player loses the minigame
    public virtual void GameLose(string titleText, string descriptionText, System.Action onNegative = null, System.Action onPositive = null)
    {
        // Logs the lose event with relevant details
        Debug.Log($"{titleText} {descriptionText}");

        // Adds a listener for the negative button (exit or cancel)
        negativeButton?.onClick.AddListener(() =>
        {
            onNegative?.Invoke();  // Invoke the negative action callback if provided
            Toggle(true);  // Show the base minigame object again
            ToggleGameArea(false);  // Hide the game area
            DifficultyManager.Instance.Toggle(true);  // Enable difficulty management or show some other UI elements
        });

        // Adds a listener for the positive button (e.g., retry or continue)
        positiveButton?.onClick.AddListener(() =>
        {
            onPositive?.Invoke();  // Invoke the positive action callback if provided
            Toggle(false);  // Hide the base minigame object
            ToggleGameArea(false);  // Hide the game area
            DifficultyManager.Instance.HideCloseButton();  // Hide the close button (perhaps after completing the minigame)
        });
    }

    // Gets the current difficulty level from the DifficultyManager
    public DifficultyEnum GetDifficulty()
    {
        return DifficultyManager.Instance.GetDifficulty();  // Returns the current difficulty level
    }
}
