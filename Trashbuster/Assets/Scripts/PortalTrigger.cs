using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTrigger : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private AudioSource portalOpenSound;
    [SerializeField] private AudioSource portalCloseSound;
    private ParticleSystem portalEffect;
    private CapsuleCollider2D portalCollider2D;
    private CapsuleCollider portalCollider;
    private GameObject portalMesh;
    private Animator animator;
    public bool is3D = false;
    public bool nextLevel = false;

    void Start()
    {
        if (gameObject.GetComponent<CapsuleCollider2D>() != null)
        {
            portalCollider2D = gameObject.GetComponent<CapsuleCollider2D>();
            is3D = false;
        }
        else if (gameObject.GetComponent<CapsuleCollider>() != null)
        {
            portalCollider = gameObject.GetComponent<CapsuleCollider>();
            portalEffect = transform.GetChild(1).GetComponent<ParticleSystem>();
            is3D = true;
        }
        animator = gameObject.GetComponent<Animator>();

        PortalOpen();
        player.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && SceneStateManager.currentGameStates != GameStates.Winscreen)
        {
            PortalClose();
            player.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && SceneStateManager.currentGameStates != GameStates.Winscreen)
        {
            PortalClose();
            player.gameObject.SetActive(false);
        }
    }

    public void ChangeScene()
    {
        if (SceneStateManager.currentGameStates == GameStates.Level1 && nextLevel)
        {
            SceneStateManager.Level1Completed = true;
            SceneStateManager.currentGameStates = GameStates.Level2;
            SceneManager.LoadScene("Level2");
        }
        else if (SceneStateManager.currentGameStates == GameStates.Level2 && nextLevel)
        {
            SceneStateManager.Level2Completed = true;
            SceneStateManager.currentGameStates = GameStates.Level3;
            SceneManager.LoadScene("Level3");
        }
        else if (SceneStateManager.currentGameStates == GameStates.Level3 && nextLevel)
        {
            SceneStateManager.Level4Completed = true;
            SceneStateManager.currentGameStates = GameStates.Level4;
            SceneManager.LoadScene("Level4");
        }
        else if (SceneStateManager.currentGameStates == GameStates.Level4 && nextLevel)
        {
            SceneStateManager.Level4Completed = true;
            SceneStateManager.currentGameStates = GameStates.Winscreen;
            SceneManager.LoadScene("WinScreen");
        }

        if (!nextLevel)// Deactivate portal after layer enter scene
        {
            gameObject.SetActive(false);
        }

    }

    public void PortalIdle()
    {
        if (nextLevel || SceneStateManager.currentGameStates == GameStates.Winscreen)// Portal idle ready to next level or idle in winsceen
        {
            if (!is3D)
            {
                portalCollider2D.enabled = true;
            }
            else
            {
                portalCollider.enabled = true;
            }
            animator.SetBool("PortalIdle", true);
            animator.SetBool("PortalClose", false);
            player.gameObject.SetActive(true);
        }
        else
        {
            // Close Portal after player enter scene
            player.gameObject.SetActive(true);
            PortalClose();
        }

    }

    public void PortalOpen()
    {
        if (is3D)
        {
            portalEffect.Play();
        }
        animator.SetBool("PortalClose", false);
        animator.SetBool("PortalIdle", false);
        portalOpenSound.Play();
    }

    public void PortalClose()
    {
        if (is3D)
        {
            portalEffect.Stop();
        }
        animator.SetBool("PortalClose", true);
        animator.SetBool("PortalIdle", false);  
        portalCloseSound.Play();
    }
}