using System.Collections;
using UnityEngine;

public enum TrashType
{
    Plastic,
    Paper,
    Metal,
    Glass
}

public class TrashBase : MonoBehaviour
{
    public TrashType trashType = TrashType.Plastic;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerController player;

    void Awake()
    {
        if (ToPlayerDist() > 12f)
        {
            gameObject.SetActive(false);
        }
    }

    private float ToPlayerDist()
    {
        return (player.transform.position - transform.position).magnitude;
    }

    public void ApplyShootForce(float shootForce, Vector2 shootDir)
    {
        rb.AddForce(shootDir * shootForce, ForceMode2D.Impulse);
    }

    public void ApplyVacuumForce(float vacuumForce, Vector2 forceDir)
    {
        rb.AddForce(forceDir * vacuumForce, ForceMode2D.Force);
    }
}
