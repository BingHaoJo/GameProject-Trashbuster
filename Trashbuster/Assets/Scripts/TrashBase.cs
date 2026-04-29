using System.Collections;
using UnityEngine;

public enum TrashType
{
    General,
    Plastic,
    Paper,
    Metal,
    Glass
}

public class TrashBase : MonoBehaviour
{
    public TrashType trashType = TrashType.Plastic;
    [SerializeField] private Rigidbody2D rb;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
