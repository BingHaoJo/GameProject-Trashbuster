using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class VacuumBarrel : MonoBehaviour
{
    public event Action<TrashBase> onTrashCollected;
    [SerializeField] private VacuumGunController vacuumGun;
    private CapsuleCollider2D barrelCollider2D;
    private CapsuleCollider barrelCollider3D;

    void Start()
    {
        if (GetComponent<CapsuleCollider2D>() != null)
        {
            barrelCollider2D = GetComponent<CapsuleCollider2D>();
        }
        else if (GetComponent<CapsuleCollider>() != null)
        {
            barrelCollider3D = GetComponent<CapsuleCollider>();
        }
    }

    void Update()
    {
        if (InputSystem.actions.FindAction("Suck").IsPressed())
        {
            if (barrelCollider2D != null)
            {
                barrelCollider2D.enabled = true; // enable collider to trigger collection
            }
            else if (barrelCollider3D != null)
            {
                barrelCollider3D.enabled = true; // enable collider to trigger collection
            }
        }
        else
        {
            if (barrelCollider2D != null)
            {
                barrelCollider2D.enabled = false; // disable collider when not sucking
            }
            else if (barrelCollider3D != null)
            {
                barrelCollider3D.enabled = false; // disable collider when not sucking
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision) // for 2D
    {
        if (collision.CompareTag("Trash"))
        {
            TrashBase trash = collision.GetComponent<TrashBase>();
            onTrashCollected?.Invoke(trash); // signal emitted
        }
    }

    void OnTriggerEnter(Collider collision) // for 3D
    {
        if (collision.CompareTag("Trash"))
        {
            TrashBase trash = collision.GetComponent<TrashBase>();
            onTrashCollected?.Invoke(trash); // signal emitted
        }
    }
}
