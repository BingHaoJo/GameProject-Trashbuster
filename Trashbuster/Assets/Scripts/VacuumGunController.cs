using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class VacuumGunController : MonoBehaviour
{
    public event Action<Vector2> mousePositionUpdated;
    private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        Vector2 direction = (worldPos - transform.position).normalized;

        mousePositionUpdated?.Invoke(direction); // signal emitted
        // Debug.Log($"Mouse direction: {direction}");

        float angleDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
        Debug.DrawLine(transform.position, worldPos, Color.red, Time.deltaTime);
    }
}
