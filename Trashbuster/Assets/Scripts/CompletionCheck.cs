using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CompletionCheck : MonoBehaviour
{
    [SerializeField] private PortalTrigger portal;
    [SerializeField] private TrashBinBase[] trashBins;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image trashLeftProgress;
    [SerializeField] private int completionThreshold = 3;
    [SerializeField] private GameObject trash;
    [SerializeField] private bool isManualThreshold;
    private int currentTrashCount = 0;

    void OnEnable()
    {
        foreach(TrashBinBase bin in trashBins)
        {
            bin.OnTrashDeposited += OnTrashDeposited;
        }
    }

    void Start()
    {
        if (!isManualThreshold)
        {
            completionThreshold = trash.transform.childCount;
        }
        foreach(GameStates state in Enum.GetValues(typeof(GameStates)))
        {
            if (SceneManager.GetActiveScene().name == state.ToString())
            {
                SceneStateManager.currentGameStates = state;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update trash score display
        scoreText.text = "Trash Score: " + GlobalVar.trashScore.ToString();

        // Update trash left
        trashLeftProgress.fillAmount = (float)currentTrashCount / completionThreshold;

        // Check Completion Progress
        if (currentTrashCount >= completionThreshold)
        {
            portal.gameObject.SetActive(true);
            portal.nextLevel = true;
        }

    }

    private void OnTrashDeposited()
    {
        currentTrashCount++;
        GlobalVar.trashScore++;
        
    }
}