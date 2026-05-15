using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string gameSceneName = "Game";

    public void StartGame()
    {
        MenuStartData.startLevelIndex = 0;
        SceneManager.LoadScene(gameSceneName);
    }

    public void StartLevel1()
    {
        MenuStartData.startLevelIndex = 0;
        SceneManager.LoadScene(gameSceneName);
    }

    public void StartLevel2()
    {
        MenuStartData.startLevelIndex = 1;
        SceneManager.LoadScene(gameSceneName);
    }

    public void StartLevel3()
    {
        MenuStartData.startLevelIndex = 2;
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}