using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "LevelSelect")
        {
            if (SceneStateManager.Level2Completed)
            {
                // Unlock Level 3 button
                GameObject.Find("Level2Button").GetComponent<UnityEngine.UI.Button>().interactable = true;
            }

            if (SceneStateManager.Level3Completed)
            {
                // Unlock Level 2 button
                GameObject.Find("Level3Button").GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
        }
        
    }

    public void PlayButtonFunction()
    {
        SceneStateManager.LoadScene("LevelSelect");
    }

    public void CreditsButtonFunction()
    {
        SceneStateManager.LoadScene("Credits");
    }
    
    public void QuitButtonFunction()
    {
        Application.Quit();
    }

    public void MainMenuButtonFunction()
    {
        SceneStateManager.LoadScene("MainMenu");
    }

    public void Level1ButtonFunction()
    {
        SceneStateManager.LoadScene("Level1");
    }

    public void Level2ButtonFunction()
    {
        SceneStateManager.LoadScene("Level2");
    }

    public void Level3ButtonFunction()
    {
        SceneStateManager.LoadScene("Level3");
    }
}
