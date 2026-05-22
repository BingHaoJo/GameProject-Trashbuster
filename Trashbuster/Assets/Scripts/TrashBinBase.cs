using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

enum BinType
{
    General,
    Food,
    Recycle,
    Paper,
    Plastic,
    Metal,
    Glass
}

public class TrashBinBase : MonoBehaviour
{
    [SerializeField] private BinType binType = BinType.General;
    [SerializeField] private TMP_Text binText;
    [SerializeField] private AudioSource depositedAudio;
    public event Action OnTrashDeposited;

    void Start()
    {
        binText.text = binType.ToString();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trash"))
        {
            TrashOrganizer(collision.GetComponent<TrashBase>());
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Trash"))
        {
            TrashOrganizer(collision.GetComponent<TrashBase>());
        }
    }

    private void TrashOrganizer(TrashBase trash)
    {
        if (binType.ToString() == trash.trashType.ToString()) // if it's the same type, accept
        {
            TrashDeposited(trash);
        }
        else if (binType == BinType.Recycle && trash.trashType != TrashType.Food) // If it's recyclable, accept
        {
            TrashDeposited(trash);
        }
        else if (binType == BinType.General) // If it's general, accept
        {
            TrashDeposited(trash);
        }
    }

    private void TrashDeposited(TrashBase trash)
    {
        trash.gameObject.SetActive(false);
        depositedAudio.Play();
        OnTrashDeposited.Invoke();
    }
}
