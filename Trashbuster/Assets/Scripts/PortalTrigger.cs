using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController player;
    [SerializeField] private CapsuleCollider2D portalCollider;
    [SerializeField] private AudioSource portalOpenSound;
    [SerializeField] private AudioSource portalCloseSound;
    private string sceneName;
    public bool nextLevel = false;

    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        PortalOpen();
        player.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        PortalOpen();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && sceneName != "WinScreen")
        {
            PortalClose();
            player.gameObject.SetActive(false);
        }
    }

    public void ChangeScene()
    {
        if (sceneName == "Level1" && nextLevel)
        {
            SceneManager.LoadScene("Level2");
            SceneStateManager.Level1Completed = true;
        }
        else if (sceneName == "Level2" && nextLevel)
        {
            SceneStateManager.Level2Completed = true;
            SceneStateManager.currentGameStates = GameStates.Level3;
            SceneManager.LoadScene("Level3_Vertical");
        }
        else if (sceneName == "Level3_Vertical" && nextLevel)
        {
            SceneManager.LoadScene("WinScreen");
            SceneStateManager.Level3Completed = true;
        }

        if (!nextLevel)// Deactivate portal after layer enter scene
        {
            gameObject.SetActive(false);
        }

    }

    public void PortalIdle()
    {
        if (nextLevel || sceneName == "WinScreen")// Portal idle ready to next level or idle in winsceen
        {
            portalCollider.enabled = true;
            animator.SetBool("PortalIdle", true);
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
        animator.SetBool("PortalClose", false);
        animator.SetBool("PortalIdle", false);
        portalOpenSound.Play();
    }

    public void PortalClose()
    {
        animator.SetBool("PortalClose", true);
        animator.SetBool("PortalIdle", false);
        portalCloseSound.Play();
    }
}
