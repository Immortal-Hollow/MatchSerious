using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    // References to UI elements in the scene
    [SerializeField] private Button loginButton;  // Button for login action
    [SerializeField] private Button logoutButton;  // Button for logout action

    [SerializeField] private TMP_Text userIdText;  // Text field to display user ID
    [SerializeField] private TMP_Text userNameText;  // Text field to display user name

    [SerializeField] private Transform loginPanel;  // Panel to show when the user is not signed in
    [SerializeField] private Transform userPanel;  // Panel to show when the user is signed in

    [SerializeField] private LoginController loginController;  // Reference to the LoginController

    private PlayerProfile playerProfile;  // Stores the current player profile

    // Unity lifecycle method: Called when the script is enabled
    private async void OnEnable()
    {
        // Register button click listeners for login and logout actions
        loginButton.onClick.AddListener(LoginButtonPressed);
        logoutButton.onClick.AddListener(LogoutButtonPressed);

        // Subscribe to events from the LoginController for sign-in and avatar update
        loginController.OnSignedIn += LoginController_OnSignedIn;
        loginController.OnAvatarUpdate += LoginController_OnAvatarUpdate;

        // Check if the player is already signed in
        if (loginController.IsSignedIn)
        {
            // If signed in, initialize the UI with the user's profile
            await InitializeSignedInUser();
        }
        else
        {
            // If not signed in, show the login panel and hide the user panel
            loginPanel.gameObject.SetActive(true);
            userPanel.gameObject.SetActive(false);
        }
    }

    // Unity lifecycle method: Called when the script is disabled
    private void OnDisable()
    {
        // Unregister the button click listeners
        loginButton.onClick.RemoveListener(LoginButtonPressed);
        logoutButton.onClick.RemoveListener(LogoutButtonPressed);

        // Unsubscribe from events to avoid memory leaks
        if (loginController != null)
        {
            loginController.OnSignedIn -= LoginController_OnSignedIn;
            loginController.OnAvatarUpdate -= LoginController_OnAvatarUpdate;
        }
    }

    // Called when the login button is pressed
    private async void LoginButtonPressed()
    {
        // Start the sign-in process using the LoginController
        await loginController.InitSignIn();
    }

    // Called when the logout button is pressed
    private async void LogoutButtonPressed()
    {
        // Log the player out through the LoginController
        await loginController.Logout();

        // After logout, show the login panel and hide the user panel
        loginPanel.gameObject.SetActive(true);
        userPanel.gameObject.SetActive(false);
    }

    // Called when the player has signed in
    private void LoginController_OnSignedIn(PlayerProfile profile)
    {
        // Store the signed-in player profile
        playerProfile = profile;

        // Hide the login panel and show the user panel
        loginPanel.gameObject.SetActive(false);
        userPanel.gameObject.SetActive(true);

        // Display the player's ID and name in the UI
        userIdText.text = $"id_{playerProfile.playerInfo.Id}";
        userNameText.text = profile.Name;
    }

    // Called when the player's avatar is updated
    private void LoginController_OnAvatarUpdate(PlayerProfile profile)
    {
        // Update the player profile with the new avatar data (if needed)
        playerProfile = profile;
    }

    // Initializes the UI for a signed-in user by fetching their profile asynchronously
    private async Task InitializeSignedInUser()
    {
        // Retrieve the player profile from the LoginController
        playerProfile = await loginController.GetPlayerProfileAsync();

        // Call the OnSignedIn method to update the UI with the player's information
        LoginController_OnSignedIn(playerProfile);
    }
}
