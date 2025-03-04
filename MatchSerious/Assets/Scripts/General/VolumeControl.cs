using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider; // Slider for background music volume
    private float lastVolume = 1.0f;

    private void Start()
    {
        if (volumeSlider != null)
        {
            // Set initial slider value to the current music volume
            volumeSlider.value = BackgroundMusic.instance.GetMusicVolume();
            volumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        else
        {
            Debug.LogError("Volume slider is not assigned.");
        }
    }

    // Called when the music volume slider changes
    private void OnMusicVolumeChanged(float volume)
    {
        // Update only the background music volume
        BackgroundMusic.instance.SetMusicVolume(volume);
        lastVolume = volume;
    }
}
