using System;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(0)] // Ensures that this script executes first in the Unity execution order
public class DifficultyManager : MonoBehaviour
{
    // Singleton instance of DifficultyManager
    public static DifficultyManager Instance { get; private set; }

    [SerializeField] private GameObject difficultySelector; // UI panel for difficulty selection
    [SerializeField] private Button closeMinigameButton; // Close button to close the difficulty selector panel

    // Buttons for selecting difficulty
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;

    private DifficultyEnum difficulty; // Holds the current difficulty selection
    private bool isActive; // Tracks whether the difficulty selector is active or not

    private void Awake()
    {
        // Ensure that the singleton instance is set to this instance of DifficultyManager
        Instance = this;
    }

    private void Start()
    {
        // Optionally, you can initialize any values here
    }

    // Registers the action to be called when the close button is clicked
    public DifficultyManager OnCloseButtonClick(Action onClick)
    {
        closeMinigameButton.onClick.AddListener(() =>
        {
            // Show close button animation before invoking the callback
            closeMinigameButton.GetComponent<TweenerUI>()
                .Show()  // Assuming this is an animation
                .OnComplete(() =>
                {
                    onClick?.Invoke();  // Trigger callback after the animation completes
                });
        });

        return this;
    }

    // Registers the action to be called when the Easy button is clicked
    public DifficultyManager OnEasyButtonClick(Action onClick)
    {
        easyButton.onClick.AddListener(() =>
        {
            easyButton.GetComponent<TweenerUI>()
                .Show()  // Assuming this is an animation
                .OnComplete(() =>
                {
                    difficulty = DifficultyEnum.Easy; // Set difficulty to Easy
                    onClick?.Invoke(); // Trigger callback after the animation completes
                });
        });

        return this;
    }

    // Registers the action to be called when the Normal button is clicked
    public DifficultyManager OnNormalButtonClick(Action onClick)
    {
        normalButton.onClick.AddListener(() =>
        {
            normalButton.GetComponent<TweenerUI>()
                .Show()  // Assuming this is an animation
                .OnComplete(() =>
                {
                    difficulty = DifficultyEnum.Normal; // Set difficulty to Normal
                    onClick?.Invoke(); // Trigger callback after the animation completes
                });
        });

        return this;
    }

    // Registers the action to be called when the Hard button is clicked
    public DifficultyManager OnHardButtonClick(Action onClick)
    {
        hardButton.onClick.AddListener(() =>
        {
            hardButton.GetComponent<TweenerUI>()
                .Show()  // Assuming this is an animation
                .OnComplete(() =>
                {
                    difficulty = DifficultyEnum.Hard; // Set difficulty to Hard
                    onClick?.Invoke(); // Trigger callback after the animation completes
                });
        });

        return this;
    }

    // Resets all button listeners (removes any previously registered actions)
    public DifficultyManager ResetListeners()
    {
        easyButton.onClick.RemoveAllListeners(); // Remove listeners from Easy button
        normalButton.onClick.RemoveAllListeners(); // Remove listeners from Normal button
        hardButton.onClick.RemoveAllListeners(); // Remove listeners from Hard button
        // Optionally, you can reset listeners on the close button as well

        return this;
    }

    // Hides the close button (used when you want to hide the difficulty selector UI)
    public void HideCloseButton()
    {
        closeMinigameButton.gameObject.SetActive(false); // Disable the button
    }

    // Shows the close button (used when you want to show the difficulty selector UI)
    public void ShowCloseButton()
    {
        closeMinigameButton.gameObject.SetActive(true); // Enable the button
    }

    // Sets whether the close button is interactable or not
    public void InteractableCloseButton(bool interactable)
    {
        closeMinigameButton.interactable = interactable; // Set button interactability
    }

    // Retrieves the current selected difficulty
    public DifficultyEnum GetDifficulty() => difficulty;

    // Toggles the visibility of the difficulty selector UI
    public void Toggle(bool toggle)
    {
        isActive = toggle; // Set the active state of the difficulty selector
        difficultySelector.SetActive(isActive); // Show or hide the difficulty selector panel
    }
}
