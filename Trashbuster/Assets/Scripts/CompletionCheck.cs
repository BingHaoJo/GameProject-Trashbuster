using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CompletionCheck : MonoBehaviour
{
    [SerializeField] private PortalTrigger portal;
    [SerializeField] private TrashBinBase[] trashBins;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image trashLeftProgress;
    [SerializeField] private int completionThreshold = 3;
    [SerializeField] private GameObject trash;
    [SerializeField] private bool isManualThreshold;
    private AudioSource bgmSource;
    private int currentTrashCount = 0;

    void OnEnable()
    {
        foreach(TrashBinBase bin in trashBins)
        {
            bin.OnTrashDeposited += OnTrashDeposited;
        }
    }

    void OnDisable()
    {
        foreach(TrashBinBase bin in trashBins)
        {
            bin.OnTrashDeposited -= OnTrashDeposited;
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

        bgmSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update trash score display
        scoreText.text = "Trash Score: " + GlobalVar.trashScore.ToString();

        // Update level display
        levelText.text = SceneStateManager.currentGameStates.ToString();

        // Update trash left
        trashLeftProgress.fillAmount = (float)currentTrashCount / completionThreshold;

        // Check Completion Progress
        if (currentTrashCount >= completionThreshold)
        {
            portal.gameObject.SetActive(true);
            portal.nextLevel = true;
        }
        
        ToggleVolume();

    }

    private void ToggleVolume()
    {
        bgmSource.mute = GlobalVar.bgmMuted;
    }

    private void OnTrashDeposited()
    {
        currentTrashCount++;
        GlobalVar.trashScore++;
    }
}