using UnityEngine;

public class GunScript : InteractionAbstract
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject particle;
    [SerializeField] private GameObject rayStart;
    
    [SerializeField] private float distance;
    [SerializeField] private int damage;

    private void Start()
    {
        particle.SetActive(false);
    }
    
    public void Shoot()
    {
        animator.SetTrigger("Shoot");
        particle.SetActive(true);
        
        Ray ray = new Ray(rayStart.transform.position, rayStart.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            Debug.Log(hit.collider.name);
            
            if (hit.collider.TryGetComponent(out SpiritScript damageAble))
            {
                damageAble.TakeDamage(damage);
            }
        }
    }
    
    public override void Interact(PlayerScript player)
    {
        transform.position = player.gunSocket.position;
        transform.rotation = player.gunSocket.rotation;
        player.currentGun = this;
        
        transform.SetParent(player.gunSocket);
    }
}
