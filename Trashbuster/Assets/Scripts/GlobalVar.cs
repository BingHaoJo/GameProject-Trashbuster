using UnityEngine;

[CreateAssetMenu(fileName = "GlobalVar", menuName = "Scriptable Objects/GlobalVar")]
public class GlobalVar : ScriptableObject
{
    public static int trashScore = 0;
    public static bool bgmMuted = false;
}
