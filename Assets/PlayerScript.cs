using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 2f; 
    
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShootRaycast();
        }
    }

    private void ShootRaycast()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.TryGetComponent(out IInteraction interaction))
            {
                interaction.Interact();
            }
        }
    }
}
