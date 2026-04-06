using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private int y_offset = 3;

    private void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + y_offset, transform.position.z);
    }
}
