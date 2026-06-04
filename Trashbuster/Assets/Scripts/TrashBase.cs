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
    [SerializeField] public Sprite canImage;

    void Start()
    {
        player = GameObject.FindWithTag("Player");

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

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // When player collides with trash, move the trash away from the player
            if (transform.position.y < collision.transform.position.y - 2f)
            {
                if (transform.position.x < collision.transform.position.x)
                {
                    rb2D.linearVelocity = Vector2.left * 5f;
                }
                else
                {
                    rb2D.linearVelocity = Vector2.right * 5f;
                }
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // When player collides with trash, move the trash away from the player
            if (transform.position.y < collision.transform.position.y - 2f)
            {
                if (transform.position.x < collision.transform.position.x)
                {
                    rb.linearVelocity = Vector3.left * 5f;
                }
                else
                {
                    rb.linearVelocity = Vector3.right * 5f;
                }
            }
        }
    }

    private void FloatTrashInGround()
    {
        if (SceneStateManager.currentGameStates == GameStates.Level1)
        {
            // When trash in floor
            if (transform.position.y < -2f)
            {
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 1f);
            }

            // When trash in wall
            if (transform.position.x < -50f)
            {
                rb2D.linearVelocity = new Vector2(1f, rb2D.linearVelocity.y);
            }
            else if (transform.position.x > 54f)
            {
                rb2D.linearVelocity = new Vector2(-1f, rb2D.linearVelocity.y);
            }
        }
        else if (SceneStateManager.currentGameStates == GameStates.Level2)
        {
            // When trash in floor
            if (transform.position.y < -1f)
            {
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 1f);
            }

            // When trash in wall
            if (transform.position.x < -25f)
            {
                rb2D.linearVelocity = new Vector2(1f, rb2D.linearVelocity.y);
            }
            else if (transform.position.x > 23.5f)
            {

                rb2D.linearVelocity = new Vector2(-1f, rb2D.linearVelocity.y);
            }
        }
        else if (SceneStateManager.currentGameStates == GameStates.Level3)
        {
            // When trash in floor
            if (transform.position.y < -1f)
            {
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 1f);
            }

            // When trash in wall
            if (transform.position.x < -99f)
            {
                rb2D.linearVelocity = new Vector2(1f, rb2D.linearVelocity.y);
            }
            else if (transform.position.x > 99f)
            {
                rb2D.linearVelocity = new Vector2(-1f, rb2D.linearVelocity.y);
            }
        }
        else if (SceneStateManager.currentGameStates == GameStates.Winscreen)
        {
            // When trash in floor
            if (transform.position.y < -8f)
            {
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 1f);
            }

            // When trash in wall
            if (transform.position.x < -17f)
            {
                rb2D.linearVelocity = new Vector2(1f, rb2D.linearVelocity.y);
            }
            else if (transform.position.x > 17f)
            {
                rb2D.linearVelocity = new Vector2(-1f, rb2D.linearVelocity.y);
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
            // print(new Vector3(1, 1, 0f) * shootForce);
            // rb.AddForce(new Vector3(1, 1, 0f) * shootForce, ForceMode.Impulse);
            rb.linearVelocity = new Vector3(shootDir.x, shootDir.y, 0f) * (shootForce+10f);
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
