using FMODUnity;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private int damageValue;
    
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject projectileEnd;
    
    [SerializeField] private FMODUnity.EventReference fireEvent;

    private void Start()
    {
        RuntimeManager.PlayOneShot(fireEvent, transform.position);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageAble damageAble))
        {
            damageAble.TakeDamage(damageValue);
            Destroy(gameObject);
        }
        else
        {
            projectile.SetActive(false);
            projectileEnd.SetActive(true);
            Destroy(gameObject, 1f); 
        }
    }
}
