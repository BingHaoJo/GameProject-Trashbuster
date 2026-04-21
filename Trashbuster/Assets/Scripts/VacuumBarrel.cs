using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class VacuumBarrel : MonoBehaviour
{
    public event Action<TrashBase> onTrashCollected;
    [SerializeField] private VacuumGunController vacuumGun;
    [SerializeField] private CapsuleCollider2D barrelCollider;

    void Update()
    {
        if (InputSystem.actions.FindAction("Suck").IsPressed())
        {
            barrelCollider.enabled = true; // enable collider to trigger collection
        }
        else
        {
            barrelCollider.enabled = false; // disable collider when not sucking
        }
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
