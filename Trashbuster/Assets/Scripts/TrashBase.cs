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
        rb.AddForce(new Vector2(1, 1), ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyVacuumForce(float vacuumForce, Vector2 forceDir)
    {
        rb.AddForce(forceDir * vacuumForce, ForceMode2D.Force);
    }
}
