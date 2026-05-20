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
    private Rigidbody2D rb2D;
    private Rigidbody rb;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (GetComponent<Rigidbody2D>() != null)
        {
            rb2D = GetComponent<Rigidbody2D>();
        }
        else if (GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
        }
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
                if (rb2D != null)
                {
                    rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 1f);
                }
                else if (rb != null)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 1f, rb.linearVelocity.z);
                }
            }

            // When trash in wall
            if (transform.position.x < -50f)
            {
                if (rb2D != null)
                {
                    rb2D.linearVelocity = new Vector2(1f, rb2D.linearVelocity.y);
                }
                else if (rb != null)
                {
                    rb.linearVelocity = new Vector3(1f, rb.linearVelocity.y, rb.linearVelocity.z);
                }
            }
            else if (transform.position.x > 54f)
            {
                if (rb2D != null)
                {
                    rb2D.linearVelocity = new Vector2(-1f, rb2D.linearVelocity.y);
                }
                else if (rb != null)
                {
                    rb.linearVelocity = new Vector3(-1f, rb.linearVelocity.y, rb.linearVelocity.z);
                }
            }
        }
        else if (SceneStateManager.currentGameStates == GameStates.Level2)
        {
            // When trash in floor
            if (transform.position.y < -1f)
            {
                if (rb2D != null)
                {
                    rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 1f);
                }
                else if (rb != null)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 1f, rb.linearVelocity.z);
                }
            }

            // When trash in wall
            if (transform.position.x < -25f)
            {
                if (rb2D != null)
                {
                    rb2D.linearVelocity = new Vector2(1f, rb2D.linearVelocity.y);
                }
                else if (rb != null)
                {
                    rb.linearVelocity = new Vector3(1f, rb.linearVelocity.y, rb.linearVelocity.z);
                }
            }
            else if (transform.position.x > 23.5f)
            {
                if (rb2D != null)
                {
                    rb2D.linearVelocity = new Vector2(-1f, rb2D.linearVelocity.y);
                }
                else if (rb != null)
                {
                    rb.linearVelocity = new Vector3(-1f, rb.linearVelocity.y, rb.linearVelocity.z);
                }
            }
        }
        else if (SceneStateManager.currentGameStates == GameStates.Level3)
        {
            // When trash in floor
            if (transform.position.y < -1f)
            {
                if (rb2D != null)
                {
                    rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 1f);
                }
                else if (rb != null)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 1f, rb.linearVelocity.z);
                }
            }

            // When trash in wall
            if (transform.position.x < -99f)
            {
                if (rb2D != null)
                {
                    rb2D.linearVelocity = new Vector2(1f, rb2D.linearVelocity.y);
                }
                else if (rb != null)
                {
                    rb.linearVelocity = new Vector3(1f, rb.linearVelocity.y, rb.linearVelocity.z);
                }
            }
            else if (transform.position.x > 99f)
            {
                if (rb2D != null)
                {
                    rb2D.linearVelocity = new Vector2(-1f, rb2D.linearVelocity.y);
                }
                else if (rb != null)
                {
                    rb.linearVelocity = new Vector3(-1f, rb.linearVelocity.y, rb.linearVelocity.z);
                }
            }
        }
        else if (SceneStateManager.currentGameStates == GameStates.Winscreen)
        {
            // When trash in floor
            if (transform.position.y < -8f)
            {
                if (rb2D != null)
                {
                    rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 1f);
                }
                else if (rb != null)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 1f, rb.linearVelocity.z);
                }
            }

            // When trash in wall
            if (transform.position.x < -17f)
            {
                if (rb2D != null)
                {
                    rb2D.linearVelocity = new Vector2(1f, rb2D.linearVelocity.y);
                }
                else if (rb != null)
                {
                    rb.linearVelocity = new Vector3(1f, rb.linearVelocity.y, rb.linearVelocity.z);
                }
            }
            else if (transform.position.x > 17f)
            {
                if (rb2D != null)
                {
                    rb2D.linearVelocity = new Vector2(-1f, rb2D.linearVelocity.y);
                }
                else if (rb != null)
                {
                    rb.linearVelocity = new Vector3(-1f, rb.linearVelocity.y, rb.linearVelocity.z);
                }
            }
        }
    }

    private float ToPlayerDist()
    {
        return (player.transform.position - transform.position).magnitude;
    }

    public void ApplyShootForce(float shootForce, Vector2 shootDir)
    {
        if (rb2D != null)
        {
            rb2D.AddForce(shootDir * shootForce, ForceMode2D.Impulse);
        }
        else if (rb != null)
        {
            rb.AddForce(shootDir * shootForce, ForceMode.Impulse);
        }
    }

    public void ApplyVacuumForce(float vacuumForce, Vector2 forceDir)
    {
        if (rb2D != null)
        {
            rb2D.AddForce(forceDir * vacuumForce, ForceMode2D.Force);
        }
        else if (rb != null)
        {
            rb.AddForce(forceDir * vacuumForce, ForceMode.Force);
        }
    }
}
