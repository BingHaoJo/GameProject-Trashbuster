using UnityEngine;
using UnityEngine.InputSystem;

public class VacuumGunController : MonoBehaviour
{
    private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {   
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        Vector2 direction = (worldPos - transform.position).normalized;
        float angleRad = Mathf.Atan2(direction.y, direction.x);
        float angleDeg = angleRad * Mathf.Rad2Deg; // Subtract 90 degrees to align with the sprite's orientation

        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
        Debug.DrawLine(transform.position, worldPos, Color.red, Time.deltaTime);
    }
}
