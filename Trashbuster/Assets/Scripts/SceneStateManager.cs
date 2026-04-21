using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneStateManager", menuName = "Scriptable Objects/SceneStateManager")]
public class SceneStateManager : ScriptableObject
{
    public static bool Level1Completed = false;
    public static bool Level2Completed = false;
    public static bool Level3Completed = false;
    
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
