using UnityEngine;

public class CameraShakeScript : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 5f;
    [SerializeField] private Transform player;

    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;
    

    private void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        
        player.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
        transform.localRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
    }
}