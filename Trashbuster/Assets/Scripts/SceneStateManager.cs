using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneStateManager", menuName = "Scriptable Objects/SceneStateManager")]
public class SceneStateManager : ScriptableObject
{
    public static bool Level1Completed = false;
    public static bool Level2Completed = false;
    public static bool Level3Completed = false;
    public static bool InLevelScene = true;
    public static GameStates currentGameStates = GameStates.MainMenu;

    public static void CheckInLevelScene(Scene scene)
    {
        if (scene.name == "Level1" || scene.name == "Level2" || scene.name == "Level3")
        {
            InLevelScene = true;
        }
        else
        {
            InLevelScene = false;
        }
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
