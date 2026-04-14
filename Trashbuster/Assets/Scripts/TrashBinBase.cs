using UnityEngine;

public class TrashBinBase : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trash") && collision.GetComponent<TrashBase>().isCollected)
        {
            Destroy(collision.gameObject); // destroy trash on contact
            Debug.Log("Trash Deposited!");
        }
    }
}
