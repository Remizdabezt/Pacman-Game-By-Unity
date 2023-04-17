using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public int startGameScene;
    public static int startAction;
    public void NewGame()
    {
        startAction = 0;
        SceneManager.LoadScene(startGameScene);
    }
    public void Continue()
    {
        startAction = 1;
        SceneManager.LoadScene(startGameScene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
