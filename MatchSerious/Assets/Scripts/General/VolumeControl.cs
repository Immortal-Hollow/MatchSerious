using UnityEngine;
using UnityEngine.UI;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider; // Reference to the UI Slider for controlling volume
    private float lastVolume = 1.0f; // Variable to store the volume level before muting

    private void Start()
    {
        // Check if volumeSlider is assigned in the Inspector
        if (volumeSlider != null)
        {
            // Set the slider's value to match the current music volume
            volumeSlider.value = BackgroundMusic.instance.audioSource.volume;

            // Add a listener to the slider to call OnVolumeChanged when the slider value changes
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        else
        {
            Debug.LogError("Volume slider is not assigned."); // Log error if slider is missing
        }
    }

    // This method is called when the volume slider value changes
    private void OnVolumeChanged(float volume)
    {
        Debug.Log("Volume changed: " + volume); // Log the new volume value for debugging

        // Set the volume in the BackgroundMusic script
        BackgroundMusic.instance.SetMusicVolume(volume);
        lastVolume = volume;
    }

    // For debugging purposes: Returns a string that represents this class in the debugger
    private string GetDebuggerDisplay()
    {
        return ToString(); // Return a string representation of the current state
    }
}
