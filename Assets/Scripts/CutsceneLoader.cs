using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

public class CutsceneLoader : MonoBehaviour
{
    [SerializeField] private float waitTime = 5f; // how long before auto load
    private bool isLoading = false;

    private void Start()
    {
        // Start the countdown coroutine
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private void Update()
    {
        // Check if player presses ESC to skip
        if (Input.GetKeyDown(KeyCode.Escape) && !isLoading)
        {
            LoadNextScene();
        }
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(waitTime);
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        isLoading = true; // Prevent double loading
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next scene found! Maybe this is the last one?");
        }
    }
}
