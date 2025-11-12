using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        StartCoroutine(LoadSceneSafe(SceneManager.GetActiveScene().name));
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        StartCoroutine(LoadSceneSafe(mainMenuSceneName));
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        PlayerPrefs.Save();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator LoadSceneSafe(string sceneName)
    {
        // wait one frame to allow UI events to clean up
        yield return null;

        // destroy duplicate EventSystem if any
        var eventSystems = FindObjectsOfType<EventSystem>();
        if (eventSystems.Length > 1)
        {
            for (int i = 1; i < eventSystems.Length; i++)
                Destroy(eventSystems[i].gameObject);
        }

        SceneManager.LoadScene(sceneName);
    }
}
