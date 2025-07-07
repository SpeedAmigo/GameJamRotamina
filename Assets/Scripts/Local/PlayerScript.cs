using DG.Tweening;
using UnityEngine;

public class PlayerScript : MonoBehaviour, IDamageAble
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject interactUI;
    [SerializeField] private float interactionDistance = 2f;

    [SerializeField] private float mouseSensitivity = 10f;
    
    [SerializeField] private FMODUnity.EventReference sanityEffectEvent;

    private float sanitySoundTimer = 0f;
    private float currentSanitySoundInterval = 5f;

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
        
        sanitySoundTimer += Time.deltaTime;
        UpdateSanitySoundInterval(); // Calculate interval based on current sanity

        if (sanitySoundTimer >= currentSanitySoundInterval)
        {
            FMODUnity.RuntimeManager.PlayOneShot(sanityEffectEvent, transform.position);
            sanitySoundTimer = 0f;
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
            // Debug co trafiłeś
            Debug.Log($"[PlayerScript] Raycast hit: {hit.collider.name}");

            if (hit.collider.TryGetComponent(out IInteraction interaction))
            {
                Debug.Log($"[PlayerScript] Found interaction component: {interaction.GetType()}");
                currentInteraction = interaction;
                interactUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("[PlayerScript] E pressed - calling interact!");
                    interaction.Interact(this);
                }
            }
            else
            {
                Debug.Log("[PlayerScript] No interaction component found");
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
            intensity = 0.5f;
        else if (sanity >= 25)
            intensity = 0.8f;
        else
            intensity = 1f;

        Shake(intensity);
    }

    private void Shake(float intensity)
    {
        if (intensity <= 0f) return;

        playerCamera.transform.DOShakeRotation(0.2f, intensity, 10);
    }
    
    private void UpdateSanitySoundInterval()
    {
        float sanity = SanityManager.Instance.GetCurrentSanity();
        // Map sanity (0–75) to interval (1s–10s)
        float clamped = Mathf.Clamp(sanity, 0f, 75f);
        currentSanitySoundInterval = Mathf.Lerp(2f, 10f, clamped / 75f);
    }

    public void TakeDamage(int damage)
    {
        SanityManager.Instance.RemoveSanity(damage);
    }
}
