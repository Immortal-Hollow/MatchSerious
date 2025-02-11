using UnityEngine;
using UnityEngine.SceneManagement; // Imports the SceneManager library to handle scene transitions

public class SceneChanger : MonoBehaviour
{
    // This method is called to change to the selected scene
    // It takes the name of the scene as a string parameter (SelectGamemode)
    public void ChangeScene(string SelectGamemode)
    {
        // Loads the scene specified by the SelectGamemode parameter
        SceneManager.LoadScene(SelectGamemode);
    }
}