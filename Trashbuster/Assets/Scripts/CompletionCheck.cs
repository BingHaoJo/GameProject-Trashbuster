using TMPro;
using UnityEngine;

public class CompletionCheck : MonoBehaviour
{
    [SerializeField] private PortalTrigger portal;
    [SerializeField] private TrashBinBase[] trashBins;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private int completionThreshold = 3;
    [SerializeField] private GameObject trash;
    private int currentTrashCount = 0;
    public static int trashScore = 0;

    void OnEnable()
    {
        foreach(TrashBinBase bin in trashBins)
        {
            bin.OnTrashDeposited += OnTrashDeposited;
        }
    }

    void Start()
    {
        completionThreshold = trash.transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        // Update trash score display
        scoreText.text = "Trash Score: " + trashScore.ToString();

        // Check Completion Progress
        if (currentTrashCount >= completionThreshold)
        {
            portal.gameObject.SetActive(true);
        }

    }

    private void OnTrashDeposited()
    {
        currentTrashCount++;
        trashScore++;
    }



}
