using System.Collections;
using UnityEngine;

public enum TrashType
{
    Food,
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

    void Update()
    {
        // Check if trash is in the ground or wall and apply force to push the trash out
        FloatTrashInGround();
    }

    private void FloatTrashInGround()
    {
        if (SceneStateManager.currentGameStates == GameStates.Level1)
        {
            // When trash in floor
            if (transform.position.y < -2f)
            {
                rb.linearVelocityY = 1f;
            }

            // When trash in wall
            if (transform.position.x < -50f)
            {
                rb.linearVelocityX = 1f;
            }
            else if (transform.position.x > 54f)
            {
                rb.linearVelocityX = -1f;
            }
        }
        else if (SceneStateManager.currentGameStates == GameStates.Level2)
        {
            // When trash in floor
            if (transform.position.y < -1f)
            {
                rb.linearVelocityY = 1f;
            }

            // When trash in wall
            if (transform.position.x < -25f)
            {
                rb.linearVelocityX = 1f;
            }
            else if (transform.position.x > 23.5f)
            {
                rb.linearVelocityX = -1f;
            }
        }
        else if (SceneStateManager.currentGameStates == GameStates.Level3)
        {
            // When trash in floor
            if (transform.position.y < -1f)
            {
                rb.linearVelocityY = 1f;
            }

            // When trash in wall
            if (transform.position.x < -99f)
            {
                rb.linearVelocityX = 1f;
            }
            else if (transform.position.x > 99f)
            {
                rb.linearVelocityX = -1f;
            }
        }
        else if (SceneStateManager.currentGameStates == GameStates.Winscreen)
        {
            // When trash in floor
            if (transform.position.y < -8f)
            {
                rb.linearVelocityY = 1f;
            }

            // When trash in wall
            if (transform.position.x < -17f)
            {
                rb.linearVelocityX = 1f;
            }
            else if (transform.position.x > 17f)
            {
                rb.linearVelocityX = -1f;
            }
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
