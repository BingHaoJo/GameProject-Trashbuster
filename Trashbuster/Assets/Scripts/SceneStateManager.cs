using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStates
{
    MainMenu,
    LevelSelect,
    Credits,
    Level1,
    Level2,
    Level3,
    Winscreen
}

[CreateAssetMenu(fileName = "SceneStateManager", menuName = "Scriptable Objects/SceneStateManager")]
public class SceneStateManager : ScriptableObject
{
    public static bool Level1Completed = false;
    public static bool Level2Completed = false;
    public static bool Level3Completed = false;
    public static bool InLevelScene = true;
    public static GameStates currentGameStates = GameStates.MainMenu;

    public static void CheckInLevelScene()
    {
        if (currentGameStates == GameStates.Level1 || currentGameStates == GameStates.Level2 || currentGameStates == GameStates.Level3)
        {
            InLevelScene = true;
        }
        else
        {
            InLevelScene = false;
        }
    }
}
