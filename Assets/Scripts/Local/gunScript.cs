using UnityEngine;
using System.Collections;

public class GunScript : InteractionAbstract
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject particle;
    [SerializeField] private GameObject rayStart;
    [SerializeField] private float distance;
    [SerializeField] private int damage;
    [SerializeField] private float reloadTime = 2f;
    
    [SerializeField] private FMODUnity.EventReference shootSoundEvent;

    private bool isReloading = false;

    private void Start()
    {
        particle.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public void Shoot()
    {
        if (isReloading)
        {
            Debug.Log("[GunScript] Cannot shoot - reloading!");
            return;
        }

        if (AmmoManager.Instance == null)
        {
            Debug.LogError("[GunScript] AmmoManager not found!");
            return;
        }

        if (AmmoManager.Instance.UseAmmo())
        {
            // Strzel
            animator.SetTrigger("Shoot");
            particle.SetActive(true);
            
            FMODUnity.RuntimeManager.PlayOneShot(shootSoundEvent, transform.position);

            Ray ray = new Ray(rayStart.transform.position, rayStart.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, distance))
            {
                Debug.Log($"[GunScript] Hit: {hit.collider.name}");

                if (hit.collider.TryGetComponent(out SpiritScript damageAble))
                {
                    damageAble.TakeDamage(damage);
                }
            }
        }
        else
        {
            Debug.Log("[GunScript] Out of ammo! Press 'R' to reload.");

        }
    }

    public void Reload()
    {
        if (isReloading) return;

        if (AmmoManager.Instance != null && AmmoManager.Instance.Reload())
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        if (animator != null)
            animator.SetTrigger("Reload");

        yield return new WaitForSeconds(reloadTime);

        isReloading = false;
    }

    public override void Interact(PlayerScript player)
    {
        transform.position = player.gunSocket.position;
        transform.rotation = player.gunSocket.rotation;
        player.currentGun = this;

        transform.SetParent(player.gunSocket);
    }
}