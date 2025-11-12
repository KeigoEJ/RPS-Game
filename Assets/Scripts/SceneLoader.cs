using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        EventSystem.current.SetSelectedGameObject(null);

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        // Save any unsaved settings
        PlayerPrefs.Save();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
}
