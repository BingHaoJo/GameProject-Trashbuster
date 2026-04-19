using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController player;
    [SerializeField] private CapsuleCollider2D portalCollider;
    private string sceneName;

    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetBool("PortalClose", true);
            animator.SetBool("PortalIdle", false);
            player.gameObject.SetActive(false);
        }
    }

    public void ChangeScene()
    {
        if (sceneName == "Level1")
        {
            SceneManager.LoadScene("Level2");
        }
        else if (sceneName == "Level2")
        {
            SceneManager.LoadScene("Level3_Vertical");
        }
    }

    public void PortalIdle()
    {
        portalCollider.enabled = true;
        animator.SetBool("PortalOpen", false);
        animator.SetBool("PortalIdle", true);
    }

}
