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

    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        AudioManager.Instance.PlaySfx(portalOpenSound);
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
        if (sceneName == "Level1")
        {
            SceneManager.LoadScene("Level2");
            SceneStateManager.Level1Completed = true;
        }
        else if (sceneName == "Level2")
        {
            SceneManager.LoadScene("Level3_Vertical");
            SceneStateManager.Level2Completed = true;
        }
        else if (sceneName == "Level3_Vertical")
        {
            SceneManager.LoadScene("WinScreen");
            SceneStateManager.Level3Completed = true;
        }
    }

    public void PortalIdle()
    {
        portalCollider.enabled = true;
        animator.SetBool("PortalOpen", false);
        animator.SetBool("PortalIdle", true);
    }

}
