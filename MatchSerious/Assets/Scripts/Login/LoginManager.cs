using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    // Event triggered when the player successfully signs in
    public event Action<PlayerProfile> OnSignedIn;

    // Event triggered when the player's avatar is updated
    public event Action<PlayerProfile> OnAvatarUpdate;

    private PlayerInfo playerInfo;  // Stores player info fetched during login
    private PlayerProfile playerProfile;  // Stores player profile data
    public PlayerProfile PlayerProfile => playerProfile;  // Read-only access to player profile

    // Checks if the player is signed in
    public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;

    // Unity lifecycle method: called when the script is initialized
    private async void Awake()
    {
        // Ensure Unity Services are initialized
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }

        // Subscribe to the event triggered when the player signs in
        PlayerAccountService.Instance.SignedIn += SignedIn;

        // Check if the player is already signed in and initialize profile
        if (IsSignedIn)
        {
            await InitializePlayerProfile();
        }
    }

    // Called when the player signs in
    private async void SignedIn()
    {
        try
        {
            // Retrieve the player's access token
            var accessToken = PlayerAccountService.Instance.AccessToken;

            // If access token is not available, initiate the sign-in flow
            if (string.IsNullOrEmpty(accessToken))
            {
                Debug.LogWarning("Access token is null or empty. Initiating login flow.");
                await InitSignIn();
                return;
            }

            // Otherwise, proceed with sign-in using the access token
            await SignInWithUnityAsync(accessToken);
        }
        catch (Exception ex)
        {
            // Log any errors that occur during the sign-in process
            Debug.LogError($"SignedIn error: {ex.Message}");
        }
    }

    // Starts the sign-in process if the player is not signed in
    public async Task InitSignIn()
    {
        // Ensure Unity Services are initialized before proceeding
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }

        try
        {
            // If the player is already signed in, proceed with existing session
            if (PlayerAccountService.Instance.IsSignedIn)
            {
                Debug.Log("Player is already signed in. Proceeding with existing session.");

                // Use the existing access token to sign in
                var accessToken = PlayerAccountService.Instance.AccessToken;
                await SignInWithUnityAsync(accessToken);
            }
            else
            {
                // If the player is not signed in, initiate the sign-in process
                Debug.Log("Starting sign-in process...");
                await PlayerAccountService.Instance.StartSignInAsync();
            }
        }
        catch (Exception ex)
        {
            // Log any errors that occur during the sign-in initiation
            Debug.LogError($"InitSignIn error: {ex.Message}");
        }
    }

    // Signs in with Unity using the provided access token
    private async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            // Attempt to sign in with the provided Unity access token
            Debug.Log("Attempting to sign in with Unity...");
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("Sign-in is successful.");

            // Initialize player profile after successful sign-in
            await InitializePlayerProfile();

            // Invoke the OnSignedIn event with the player profile
            OnSignedIn?.Invoke(playerProfile);
        }
        catch (AuthenticationException ex)
        {
            // Handle authentication-specific errors
            Debug.LogError($"AuthenticationException: {ex.Message}");
        }
        catch (RequestFailedException ex)
        {
            // Handle failed requests during sign-in
            Debug.LogError($"RequestFailedException: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Handle any other errors that occur during the sign-in process
            Debug.LogError($"SignInWithUnityAsync error: {ex.Message}");
        }
    }

    // Initializes the player's profile by fetching their info and name
    private async Task InitializePlayerProfile()
    {
        // Retrieve the player info from the Authentication service
        playerInfo = AuthenticationService.Instance.PlayerInfo;

        // Fetch the player's name asynchronously
        var name = await AuthenticationService.Instance.GetPlayerNameAsync();

        // Create a new player profile with the fetched player info and name
        playerProfile = new PlayerProfile
        {
            playerInfo = playerInfo,
            Name = name
        };
    }

    // Retrieves the player's profile asynchronously
    public async Task<PlayerProfile> GetPlayerProfileAsync()
    {
        // If player profile is not initialized, initialize it first
        if (playerProfile.playerInfo == null)
        {
            await InitializePlayerProfile();
        }
        return playerProfile;
    }

    // Logs the player out and clears their session
    public async Task Logout()
    {
        try
        {
            // Sign out of the authentication service
            AuthenticationService.Instance.SignOut();

            // Clear the session token from PlayerPrefs or through a service method if available
            // PlayerAccountService.Instance.ClearSessionToken(); // Uncomment if available
            PlayerPrefs.DeleteKey("com.unity.services.authentication.playeraccounts.session_token");
            PlayerPrefs.Save();

            // Clear player profile and player info
            playerProfile = new PlayerProfile();
            playerInfo = null;

            Debug.Log("Logout is successful.");
        }
        catch (Exception ex)
        {
            // Log any errors that occur during the logout process
            Debug.LogError($"Logout failed: {ex.Message}");
        }
    }

    // Unity lifecycle method: cleanup when the object is destroyed
    private void OnDestroy()
    {
        // Unsubscribe from the SignedIn event to prevent memory leaks
        PlayerAccountService.Instance.SignedIn -= SignedIn;
    }
}

// Serializable struct to store player profile information
[Serializable]
public struct PlayerProfile
{
    public PlayerInfo playerInfo;  // Stores player info (e.g., ID, access token)
    public string Name;  // Stores the player's name
}
