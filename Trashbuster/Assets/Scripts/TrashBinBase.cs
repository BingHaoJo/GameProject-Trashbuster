using System;
using System.Collections.Generic;
using UnityEngine;

enum BinType
{
    General,
    Recycle,
    Paper,
    Plastic,
    Metal,
    Glass
}

public class TrashBinBase : MonoBehaviour
{
    [SerializeField] private GameObject trash;
    [SerializeField] private BinType binType = BinType.General;
    [SerializeField] private int completionThreshold = 3;
    private int currentTrashCount = 0;
    public bool isCompleted = false;

    void Start()
    {
        // completionThreshold = trash.transform.childCount;
        trash = GameObject.Find("Trash");
        print(BinType.General);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trash"))
        {
            TrashOrganizer(collision.GetComponent<TrashBase>());

            if (currentTrashCount >= completionThreshold)
            {
                isCompleted = true;
            }
        }
    }

    private void TrashOrganizer(TrashBase trash)
    {
        if (binType.ToString() == trash.trashType.ToString()) // if it's the same type, accept
        {
            trash.gameObject.SetActive(false);
            currentTrashCount++;
            CompletionCheck.trashScore++;
        }
        else if (binType == BinType.Recycle && trash.trashType != TrashType.General) // If it's recyclable, accept
        {
            trash.gameObject.SetActive(false);
            currentTrashCount++;
            CompletionCheck.trashScore++;
        }
        else if (binType == BinType.General) // If it's general, accept
        {
            trash.gameObject.SetActive(false);
            currentTrashCount++;
            CompletionCheck.trashScore++;
        }
        else // if it's the wrong type, reject
        {
            trash.ApplyShootForce(5f, (trash.transform.position - transform.position).normalized);
        }
    }
}
