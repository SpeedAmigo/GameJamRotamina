using System;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject interactUI;
    [SerializeField] private float interactionDistance = 2f;
    
    public Transform gunSocket;
    public GunScript currentGun;
    
    private IInteraction currentInteraction;
    

    private void Start()
    {
        interactUI.SetActive(false);
    }
    private void Update()
    { 
        ShootRaycast();

        if (Input.GetKeyDown(KeyCode.G) && currentGun != null)
        {
            currentGun.DropGun();
            currentGun = null;
        }

        if (Input.GetMouseButtonDown(0) && currentGun != null)
        {
            currentGun.Shoot();
        }
    }

    private void ShootRaycast()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.TryGetComponent(out IInteraction interaction))
            {
                currentInteraction = interaction;
                interactUI.SetActive(true);
                
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interaction.Interact(this);
                }
            }
            else
            {
                currentInteraction = null;
                interactUI.SetActive(false);
            }
        }
        else
        {
            currentInteraction = null;
            interactUI.SetActive(false);
        }
    }
}
