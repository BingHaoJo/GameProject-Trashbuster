using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class VacuumBarrel : MonoBehaviour
{
    public event Action<GameObject> onTrashCollected;
    [SerializeField] private VacuumGunController vacuumGun;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trash"))
        {
            GameObject trash = collision.gameObject;
            onTrashCollected?.Invoke(trash); // signal emitted
        }
    }
}
