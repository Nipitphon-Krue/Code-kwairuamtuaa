using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public void StartGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Credit()
    {
        SceneManager.LoadScene("Credit");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}