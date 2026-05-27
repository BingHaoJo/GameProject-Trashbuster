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
            animator = gameObject.GetComponent<Animator>();
            is3D = false;
        }
        else if (gameObject.GetComponent<CapsuleCollider>() != null)
        {
            portalCollider = gameObject.GetComponent<CapsuleCollider>();
            portalMesh = transform.GetChild(0).gameObject;
            portalEffect = transform.GetChild(1).GetComponent<ParticleSystem>();
            is3D = true;
        }

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
            player.gameObject.SetActive(false);
            StartCoroutine(LoadNextLevelDelay());
        }
    }

    IEnumerator LoadNextLevelDelay()
    {
        yield return new WaitForSeconds(0.3f);
        PortalClose();
        ChangeScene();
    }
    
    IEnumerator PortalOpenDelay()
    {
        yield return new WaitForSeconds(0.3f);
        PortalIdle();
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
                animator.SetBool("PortalIdle", true);
            }
            else
            {
                portalCollider.enabled = true;
            }
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
        if (!is3D)
        {
            animator.SetBool("PortalClose", false);
            animator.SetBool("PortalIdle", false);
        }
        else
        {
            portalMesh.SetActive(true);
            portalEffect.Play();
            StartCoroutine(PortalOpenDelay());
        }
        portalOpenSound.Play();
    }

    public void PortalClose()
    {
        if (!is3D)
        {
            animator.SetBool("PortalClose", true);
            animator.SetBool("PortalIdle", false);  
        }
        else
        {
            portalMesh.SetActive(false);
            portalEffect.Stop();
        }
        portalCloseSound.Play();
    }

    public void OpenPortal3D()
    {
        if (!portalMesh.activeSelf)
        {
            PortalOpen();
        }
    }
}