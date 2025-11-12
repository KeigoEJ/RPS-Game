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
        // ✅ Freeze gameplay but NOT UI animations
        Time.timeScale = 0f;

        // Activate the pause UI
        if (pausePanel != null)
            pausePanel.SetActive(true);

        isPaused = true;
    }

    public void ResumeGame()
    {
        // 🕐 Unfreeze gameplay
        Time.timeScale = 1f;

        // Hide pause menu
        if (pausePanel != null)
            pausePanel.SetActive(false);

        isPaused = false;
    }

    // Optional: if you want to call from UI Button
    public void OnResumeButton()
    {
        ResumeGame();
    }
}
