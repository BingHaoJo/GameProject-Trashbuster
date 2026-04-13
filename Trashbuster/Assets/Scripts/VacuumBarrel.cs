using System;
using UnityEngine;

public class VacuumBarrel : MonoBehaviour
{
    public event Action<TrashBase> onTrashCollected;
    [SerializeField] private VacuumGunController vacuumGun;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trash"))
        {
            TrashBase trash = collision.GetComponent<TrashBase>();
            onTrashCollected?.Invoke(trash); // signal emitted
        }
    }
}
