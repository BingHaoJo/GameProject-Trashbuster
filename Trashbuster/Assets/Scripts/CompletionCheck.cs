using UnityEngine;

public class CompletionCheck : MonoBehaviour
{
    [SerializeField] private PortalTrigger portal;
    [SerializeField] private TrashBinBase[] trashBins;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
