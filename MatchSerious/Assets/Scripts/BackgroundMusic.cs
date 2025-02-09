using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    // Static instance variable to ensure there's only one instance of BackgroundMusic
    public static BackgroundMusic instance = null;

    // Reference to the AudioSource component that will play the music
    public AudioSource audioSource;

    // Called when the script is first loaded or the object is instantiated
    private void Awake()
    {
        // Check if an instance of BackgroundMusic already exists
        if (instance == null)
        {
            // If no instance exists, set this instance as the one and keep it across scenes
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevent the music manager from being destroyed when changing scenes
        }
        else if (instance != this)
        {
            // If another instance already exists, destroy this duplicate object
            Destroy(gameObject); // This ensures only one BackgroundMusic object persists in the game
        }
    }

    // Called when the script is enabled
    private void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Check if the AudioSource component is attached and valid
        if (audioSource != null)
        {
            // If the audio source is not already playing, start playing the music
            if (!audioSource.isPlaying)
            {
                audioSource.Play(); // Start the background music
            }
        }
        else
        {
            // If the AudioSource is missing, log an error
            Debug.LogError("AudioSource component missing on MusicManager.");
        }
    }

    // Method to adjust the music volume
    public void SetMusicVolume(float volume)
    {
        // If the AudioSource is assigned, change its volume
        if (audioSource != null)
        {
            audioSource.volume = volume; // Set the volume of the audio source
        }
    }
}
