using UnityEngine;

enum TrashType
{
    Plastic,
    Paper,
    Metal
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyVacuumForce(float vacuumForce, Vector2 forceDir)
    {
        rb.AddForce(forceDir * vacuumForce, ForceMode2D.Force);
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("VacuumGun"))
    //     {
    //         Vector2 forceDir = (transform.position - other.transform.position).normalized;
    //         rb.AddForce(forceDir * 2f, ForceMode2D.Force);
    //     }
    // }
}
