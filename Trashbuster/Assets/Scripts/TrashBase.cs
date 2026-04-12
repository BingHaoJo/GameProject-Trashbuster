using UnityEngine;

enum TrashType
{
    Plastic,
    Paper,
    Metal,
    Glass
}

public class TrashBase : MonoBehaviour
{
    [SerializeField] private TrashType trashType = TrashType.Plastic;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyVacuumForce(float vacuumForce, Vector2 forceDir)
    {
        rb.AddForce(forceDir * vacuumForce, ForceMode2D.Force);
    }
}
