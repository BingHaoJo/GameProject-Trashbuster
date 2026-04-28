using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private AudioClip pressSound;
    private GameObject MainMenuUI;
    private GameObject LevelSelectUI;
    
    void Start()
    {
        if (GameObject.Find("MainMenuUI") != null)
        {
            MainMenuUI = GameObject.Find("MainMenuUI");
        }

        if (GameObject.Find("LevelSelectUI") != null)
        {
            LevelSelectUI = GameObject.Find("LevelSelectUI");
            print(LevelSelectUI);
            LevelSelectUI.SetActive(false);
        }
    }
    
    void Update()
    {
        if (LevelSelectUI != null)
        {
            if (LevelSelectUI.activeSelf)
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
        
    }

    public void PlayButtonFunction()
    {
        AudioManager.Instance.PlaySfx(pressSound);
        MainMenuUI.SetActive(false);
        LevelSelectUI.SetActive(true);
    }

    public void CreditsButtonFunction()
    {
        AudioManager.Instance.PlaySfx(pressSound);
        SceneStateManager.LoadScene("Credits");
    }
    
    public void QuitButtonFunction()
    {
        AudioManager.Instance.PlaySfx(pressSound);
        Application.Quit();
    }

    public void MainMenuButtonFunction()
    {
        AudioManager.Instance.PlaySfx(pressSound);
        if (SceneManager.GetActiveScene().name == "WinScreen")
        {
            SceneStateManager.LoadScene("MainMenu");
        }
        else if (SceneManager.GetActiveScene().name == "MainMenu" && LevelSelectUI.activeSelf)
        {
            MainMenuUI.SetActive(true);
            LevelSelectUI.SetActive(false);
        }
    }

    public void Level1ButtonFunction()
    {
        AudioManager.Instance.PlaySfx(pressSound);
        SceneStateManager.LoadScene("Level1");
    }

    public void Level2ButtonFunction()
    {
        AudioManager.Instance.PlaySfx(pressSound);
        SceneStateManager.LoadScene("Level2");
    }

    public void Level3ButtonFunction()
    {
        AudioManager.Instance.PlaySfx(pressSound);
        SceneStateManager.LoadScene("Level3");
    }
}
