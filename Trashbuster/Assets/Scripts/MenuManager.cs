using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private AudioClip pressSound;
    private GameObject MainMenuUI;
    private GameObject LevelSelectUI;
    private GameObject CreditsUI;
    private AudioSource bgmSource;
    
    void Start()
    {
        foreach(GameStates state in Enum.GetValues(typeof(GameStates)))// Recorrect currentGameStates
        {
            if (SceneManager.GetActiveScene().name == state.ToString())
            {
                SceneStateManager.currentGameStates = state;
                break;
            }
        }

        bgmSource = GetComponent<AudioSource>();

        if (GameObject.Find("MainMenuUI") != null)
        {
            MainMenuUI = GameObject.Find("MainMenuUI");
        }

        if (GameObject.Find("LevelSelectUI") != null)
        {
            LevelSelectUI = GameObject.Find("LevelSelectUI");
            LevelSelectUI.SetActive(false);
        }
        
        if (GameObject.Find("CreditsUI") != null)
        {
            CreditsUI = GameObject.Find("CreditsUI");
            CreditsUI.SetActive(false);
        }
    }
    
    void Update()
    {
        if (SceneStateManager.currentGameStates == GameStates.LevelSelect)
        {
            if (SceneStateManager.Level2Completed)
            {
                // Unlock Level 2 button
                GameObject.Find("Level2Button").GetComponent<UnityEngine.UI.Button>().interactable = true;
            }

            if (SceneStateManager.Level3Completed)
            {
                // Unlock Level 3 button
                GameObject.Find("Level3Button").GetComponent<UnityEngine.UI.Button>().interactable = true;
            }

            if (SceneStateManager.Level4Completed)
            {
                // Unlock Level 4 button
                GameObject.Find("Level4Button").GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
        }

        ToggleVolume();
    }

    private void ToggleVolume()
    {
        bgmSource.mute = GlobalVar.bgmMuted;
    }

    public void PlayButtonFunction()
    {
        AudioManager.Instance.PlaySfx(pressSound);
        MainMenuUI.SetActive(false);
        LevelSelectUI.SetActive(true);
        SceneStateManager.currentGameStates = GameStates.LevelSelect;
    }

    public void CreditsButtonFunction()
    {
        AudioManager.Instance.PlaySfx(pressSound);
        MainMenuUI.SetActive(false);
        CreditsUI.SetActive(true);
        SceneStateManager.currentGameStates = GameStates.Credits;
    }

    public void MainMenuButtonFunction()
    {
        AudioManager.Instance.PlaySfx(pressSound);
        if (SceneStateManager.currentGameStates == GameStates.Winscreen)
        {
            SceneStateManager.currentGameStates = GameStates.MainMenu;
            SceneManager.LoadScene("MainMenu");
        }
        else if (SceneStateManager.currentGameStates == GameStates.LevelSelect || SceneStateManager.currentGameStates == GameStates.Credits)
        {
            MainMenuUI.SetActive(true);
            LevelSelectUI.SetActive(false);
            CreditsUI.SetActive(false);
            SceneStateManager.currentGameStates = GameStates.MainMenu;
        }
    }

    public void Level1ButtonFunction()
    {
        SceneStateManager.currentGameStates = GameStates.Level1;
        AudioManager.Instance.PlaySfx(pressSound);
        SceneManager.LoadScene("Level1");
    }

    public void Level2ButtonFunction()
    {
        SceneStateManager.currentGameStates = GameStates.Level2;
        AudioManager.Instance.PlaySfx(pressSound);
        SceneManager.LoadScene("Level2");
    }

    public void Level3ButtonFunction()
    {
        SceneStateManager.currentGameStates = GameStates.Level3;
        AudioManager.Instance.PlaySfx(pressSound);
        SceneManager.LoadScene("Level3");
    }

    public void Level4ButtonFunction()
    {
        SceneStateManager.currentGameStates = GameStates.Level4;
        AudioManager.Instance.PlaySfx(pressSound);
        SceneManager.LoadScene("Level4");
    }
}