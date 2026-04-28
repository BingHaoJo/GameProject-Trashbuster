using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController player;
    [SerializeField] private CapsuleCollider2D portalCollider;
    [SerializeField] private AudioClip portalOpenSound;
    [SerializeField] private AudioClip portalCloseSound;
    private string sceneName;
    public bool nextLevel = false;

    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        AudioManager.Instance.PlaySfx(portalOpenSound);
        player.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && sceneName != "WinScreen")
        {
            animator.SetBool("PortalClose", true);
            animator.SetBool("PortalIdle", false);
            player.gameObject.SetActive(false);
            AudioManager.Instance.PlaySfx(portalCloseSound);
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
            SceneManager.LoadScene("Level3_Vertical");
            SceneStateManager.Level2Completed = true;
        }
        else if (sceneName == "Level3_Vertical" && nextLevel)
        {
            SceneManager.LoadScene("WinScreen");
            SceneStateManager.Level3Completed = true;
        }

        if (!nextLevel)
        {
            gameObject.SetActive(false);
        }

    }

    public void PortalIdle()
    {
        if (nextLevel || sceneName == "WinScreen")
        {
            portalCollider.enabled = true;
            animator.SetBool("PortalIdle", true);
            player.gameObject.SetActive(true);
        }
        else
        {
            // Close Portal
            player.gameObject.SetActive(true);
            animator.SetBool("PortalClose", true);
            AudioManager.Instance.PlaySfx(portalCloseSound);
        }

    }

    

}
