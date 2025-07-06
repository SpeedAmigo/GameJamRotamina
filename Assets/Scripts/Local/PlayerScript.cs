using DG.Tweening;
using UnityEngine;

public class PlayerScript : MonoBehaviour, IDamageAble
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject interactUI;
    [SerializeField] private float interactionDistance = 2f;
    
    [SerializeField] private float mouseSensitivity = 10f;
    
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
        CameraShake();
        
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
    
    private void CameraShake()
    {
        float sanity = SanityManager.Instance.GetCurrentSanity();
        float intensity;

        if (sanity >= 75)
            intensity = 0f;
        else if (sanity >= 50)
            intensity = 2f;
        else if (sanity >= 25)
            intensity = 5f;
        else
            intensity = 10f;

        Shake(intensity);
    }
    
    private void Shake(float intensity)
    {
        if (intensity <= 0f) return;
        
        playerCamera.transform.DOShakeRotation(0.2f, intensity);
    }
    
    public void TakeDamage(int damage)
    {
        SanityManager.Instance.RemoveSanity(damage);
    }
}
