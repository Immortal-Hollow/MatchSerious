using UnityEngine;

public class LinkOpener : MonoBehaviour
{
    // URL to open (this can be modified to any URL you want)
    public string url = "https://docs.google.com/forms/d/e/1FAIpQLScefyFLOZfwQigNcNOS4SKgJkKW7B02cx6eQ_daCeupLDfo3g/viewform";

    // Method to open the URL when called
    // This method uses Unity's Application class to open the given URL in the default web browser
    public void OpenURL()
    {
        // Opens the URL specified in the 'url' variable
        Application.OpenURL(url);
    }
}
