using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFunctions : MonoBehaviour
{
    protected void Pause(GameObject screen)
    {
        Debug.Log("Pause");
        screen.SetActive(true);
        Time.timeScale = 0;
    }

    protected void Resume(GameObject screen)
    {
        Debug.Log("Resume");
        screen.SetActive(false);
        Time.timeScale = 1;
    }

    protected void ReloadLevel()
    {
        Debug.Log("Reload");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    protected void LoadScene(int index)
    {
        Debug.Log("Loaded " + index);
        SceneManager.LoadScene(index); 
    }

    protected void Quit()
    {
        Application.Quit();
    }
}
