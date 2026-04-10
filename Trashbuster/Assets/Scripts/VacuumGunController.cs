using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class VacuumGunController : MonoBehaviour
{
    [SerializeField] float vacuumForce = 7f;
    [SerializeField] float maxRaycastDist = 8f;
    public event Action<Vector2> mousePositionUpdated;
    private Camera mainCamera;

    private GameObject[] trashObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        #region Gun look at mouse & signal emission
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        Vector2 direction = (worldPos - transform.position).normalized;
        mousePositionUpdated?.Invoke(worldPos); // signal emitted
        float angleDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
        // Debug.DrawLine(transform.position, worldPos, Color.red, Time.deltaTime);
        #endregion
        
        // CheckRaycast();

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Trash") && InputSystem.actions.FindAction("Suck").IsPressed())
        {
            TrashBase trash = collision.GetComponent<TrashBase>();
            Vector2 forceDir = ((Vector2)transform.position - (Vector2)collision.transform.position).normalized;
            trash.ApplyVacuumForce(vacuumForce, forceDir);
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
