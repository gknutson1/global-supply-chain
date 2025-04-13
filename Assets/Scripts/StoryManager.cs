using TMPro;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public Canvas storyTextCanvas;
    public StoryText storyText;
    public Canvas pauseMenu;
    bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused ^= true;
        storyText.Pause(isPaused);
        storyTextCanvas.enabled = !isPaused;
        pauseMenu.enabled = isPaused;
    }
}
