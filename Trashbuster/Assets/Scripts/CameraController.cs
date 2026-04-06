using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private void LateUpdate()
    {
        print(transform.position);
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }
}
