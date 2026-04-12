using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VacuumGunController : MonoBehaviour
{
    [SerializeField] float vacuumForce = 7f;
    [SerializeField] float pushForce = -30f;
    public event Action<Vector2> mousePositionUpdated;
    private Camera mainCamera;
    private PlayerController player;
    private Queue<GameObject> trashQueue = new Queue<GameObject>();
    private VacuumBarrel vacuumBarrel;
    private Vector2 mousePos;
    private Vector3 worldPos;
    private Vector2 gunDir;
    private bool canPush = true;
    private float pushCooldown = 2f;

    private void OnEnable()
    {
        vacuumBarrel = GetComponentInChildren<VacuumBarrel>();
        if (vacuumBarrel != null)
        {
            vacuumBarrel.onTrashCollected += OnTrashCollected; // signal connected
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    
        player = transform.parent.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        // Get mouse position in world space
        mousePos = Mouse.current.position.ReadValue();
        worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        gunDir = (worldPos - transform.position).normalized;

        // Rotate gun to look at mouse
        float angleDeg = Mathf.Atan2(gunDir.y, gunDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);

        mousePositionUpdated?.Invoke(worldPos); // signal emitted

        if (InputSystem.actions.FindAction("Push").IsPressed() && canPush)
        {            
            Vector2 pushDir = (worldPos - player.transform.position).normalized;
            player.ApplyPushForce(pushForce, pushDir);
            canPush = false;
            StartCoroutine(PushCooldown());
        }
        Debug.Log(gunDir);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TrashBase trash = collision.GetComponent<TrashBase>();
        Vector2 forceDir = ((Vector2)transform.position - (Vector2)collision.transform.position).normalized;

        // Apply Vacuum Force on trash
        if (collision.CompareTag("Trash") && InputSystem.actions.FindAction("Suck").IsPressed())
        {
            trash.ApplyVacuumForce(vacuumForce, forceDir);
        }

        // Apply Push Force on trash
        if (collision.CompareTag("Trash") && InputSystem.actions.FindAction("Push").IsPressed())
        {
            trash.ApplyVacuumForce(-vacuumForce, forceDir);
        }


    }

    private void OnTrashCollected(TrashBase trash)
    {
        // Object Pooling Trash
        print("Trash collected - signal works!");
        trash.gameObject.SetActive(false);
        trashQueue.Enqueue(trash.gameObject);
    }

    private IEnumerator PushCooldown()
    {
        yield return new WaitForSeconds(pushCooldown);
        print("Push ready!");
        canPush = true;
    }

    private void OnDrawGizmos()
    {
        // Debug.DrawLine(transform.position, worldPos, Color.red, Time.deltaTime);
    }

    private void OnDisable()
    {
        if (vacuumBarrel != null)
        {
            vacuumBarrel.onTrashCollected -= OnTrashCollected; // signal disconnected
        }
    }
    // private void CheckRaycast()
    // {
    //     Vector2 origin = transform.position;
    //     Vector2 aimDir = transform.right;         // direction gun is facing
    //     Vector2 perpDir = transform.up;           // local up — perpendicular to aim

    //     Vector2 raycastDir1 = aimDir + perpDir * 0.2f;
    //     Vector2 raycastDir2 = aimDir + perpDir * 0.1f;
    //     Vector2 raycastDir3 = aimDir;
    //     Vector2 raycastDir4 = aimDir - perpDir * 0.1f;
    //     Vector2 raycastDir5 = aimDir - perpDir * 0.2f;

    //     RaycastHit2D hit1 = Physics2D.Raycast(origin, raycastDir1, maxRaycastDist, LayerMask.GetMask("Trash"), minDepth: -1f, maxDepth: 1f);
    //     RaycastHit2D hit2 = Physics2D.Raycast(origin, raycastDir2, maxRaycastDist, LayerMask.GetMask("Trash"), minDepth: -1f, maxDepth: 1f);
    //     RaycastHit2D hit3 = Physics2D.Raycast(origin, raycastDir3, maxRaycastDist, LayerMask.GetMask("Trash"), minDepth: -1f, maxDepth: 1f);
    //     RaycastHit2D hit4 = Physics2D.Raycast(origin, raycastDir4, maxRaycastDist, LayerMask.GetMask("Trash"), minDepth: -1f, maxDepth: 1f);
    //     RaycastHit2D hit5 = Physics2D.Raycast(origin, raycastDir5, maxRaycastDist, LayerMask.GetMask("Trash"), minDepth: -1f, maxDepth: 1f);

    //     Debug.DrawRay(origin, raycastDir1 * maxRaycastDist, Color.green, Time.deltaTime);
    //     Debug.DrawRay(origin, raycastDir2 * maxRaycastDist, Color.green, Time.deltaTime);
    //     Debug.DrawRay(origin, raycastDir3 * maxRaycastDist, Color.red, Time.deltaTime);
    //     Debug.DrawRay(origin, raycastDir4 * maxRaycastDist, Color.blue, Time.deltaTime);
    //     Debug.DrawRay(origin, raycastDir5 * maxRaycastDist, Color.blue, Time.deltaTime);

    //     if ((hit1 || hit2 || hit3 || hit4 || hit5) && InputSystem.actions.FindAction("Suck").IsPressed())
    //     {
    //         RaycastHit2D hit = hit1 ? hit1 : (hit2 ? hit2 : (hit3 ? hit3 : (hit4 ? hit4 : hit5)));
    //         Vector2 forceDir = ((Vector2)transform.position - hit.point).normalized;
    //         hit.collider.gameObject.GetComponent<TrashBase>()?.ApplyVacuumForce(vacuumForce, forceDir);
    //     }
    // }
}
