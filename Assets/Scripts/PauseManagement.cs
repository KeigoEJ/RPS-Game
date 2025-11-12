using UnityEngine;
using UnityEngine.UI;

public class PauseManagement : MonoBehaviour
{
    [Header("🕹 Pause Settings")]
    public GameObject pausePanel; // Your pause menu UI
    public KeyCode pauseKey = KeyCode.Escape; // Default key to toggle pause

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        // ✅ No time freeze, just mark as paused
        isPaused = true;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        // Optionally stop gameplay (your movement, etc.)
        ToggleGameplay(false);
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Resume gameplay
        ToggleGameplay(true);
    }

    // This will disable any script you assign in the Inspector
    [Header("🎮 Scripts to Disable When Paused")]
    public MonoBehaviour[] scriptsToDisable;

    private void ToggleGameplay(bool enable)
    {
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = enable;
        }
    }

    // Optional: call from UI Button
    public void OnResumeButton()
    {
        ResumeGame();
    }
}
