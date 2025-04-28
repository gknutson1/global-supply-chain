using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public Canvas cutsceneCanvas;
    public Canvas pauseMenu;
    public Canvas optionsMenu;
    bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused ^= true;
        cutsceneCanvas.enabled = !isPaused;
        pauseMenu.enabled = isPaused;
        optionsMenu.enabled = false;
    }
}
