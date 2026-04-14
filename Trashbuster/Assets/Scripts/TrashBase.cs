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
    [SerializeField] private Rigidbody2D rb;
    public bool isCollected = false;

    public void ApplyShootForce(float shootForce, Vector2 shootDir)
    {
        rb.AddForce(shootDir * shootForce, ForceMode2D.Impulse);
    }

    public void ApplyVacuumForce(float vacuumForce, Vector2 forceDir)
    {
        rb.AddForce(forceDir * vacuumForce, ForceMode2D.Force);
    }
}
