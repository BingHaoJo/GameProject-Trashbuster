using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletionCheck : MonoBehaviour
{
    [SerializeField] private PortalTrigger portal;
    [SerializeField] private TrashBinBase[] trashBins;
    [SerializeField] private TMP_Text scoreText;
    public static int trashScore = 0;

    // Update is called once per frame
    void Update()
    {
        // Update trash score display
        scoreText.text = "Trash Score: " + trashScore.ToString();

        // Manage portal active
        if (trashBins.Length == 1)
        {
            if (trashBins[0].isCompleted)
            {
                portal.gameObject.SetActive(true);
            }
        }
        else if (trashBins.Length == 2)
        {
            if (trashBins[0].isCompleted && trashBins[1].isCompleted)
            {
                portal.gameObject.SetActive(true);
            }
        }
        else if (trashBins.Length == 4)
        {
            if (trashBins[0].isCompleted && trashBins[1].isCompleted && trashBins[2].isCompleted && trashBins[3].isCompleted)
            {
                portal.gameObject.SetActive(true);
            }
        }

    }
}
