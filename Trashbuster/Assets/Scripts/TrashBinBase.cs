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
        switch (binType)
        {
            case BinType.General:
                // Accept all trash types
                trash.gameObject.SetActive(false);
                currentTrashCount++;
                break;
            case BinType.Recycle:
                // Accept all recyclable trash types
                if (trash.trashType == TrashType.Paper || trash.trashType == TrashType.Plastic 
                || trash.trashType == TrashType.Metal || trash.trashType == TrashType.Glass)
                {
                    trash.gameObject.SetActive(false);
                    currentTrashCount++;
                }
                break;
            case BinType.Paper:
                // Accept only paper trash
                if (trash.trashType == TrashType.Paper)
                {
                    trash.gameObject.SetActive(false);
                    currentTrashCount++;
                }
                break;
            case BinType.Plastic:
                // Accept only plastic trash
                if (trash.trashType == TrashType.Plastic)
                {
                    trash.gameObject.SetActive(false);
                    currentTrashCount++;
                }
                break;
            case BinType.Metal:
                // Accept only metal trash
                if (trash.trashType == TrashType.Metal)
                {
                    trash.gameObject.SetActive(false);
                    currentTrashCount++;
                }
                break;
            case BinType.Glass:
                // Accept only glass trash
                if (trash.trashType == TrashType.Glass)
                {
                    trash.gameObject.SetActive(false);
                    currentTrashCount++;
                }
                break;
        }
    }
}
