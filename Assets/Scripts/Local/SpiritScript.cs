using System.Collections.Generic;
using RaycastPro.Detectors;
using RaycastPro.RaySensors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SpiritScript : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;
    [SerializeField] private SightDetector sight;
    
    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootCooldown = 2f;
    [SerializeField] private float shootForce = 15f;
    
    [SerializeField] private Animator animator;

    private HashSet<Collider> detections;
    private float velocity;
    private float shootTimer;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        detections = sight.DetectedColliders;
        
        velocity  = agent.velocity.magnitude;
        
        animator.SetFloat("Velocity", velocity);
        
        if (detections.Count > 0  && target != null)
        {
            agent.SetDestination(target.position);
            
            shootTimer -= Time.deltaTime;
            
            if (shootTimer <= 0f)
            {
                ShootProjectile();
                shootTimer = shootCooldown;
            }
        }
    }
    
    private void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null || target == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (target.position - firePoint.position).normalized;
            rb.linearVelocity = direction * shootForce;
        }

        // Optional: Add animation or sound
        //animator.SetTrigger("Shoot");
    }

    public void GetTargetRef()
    {
        foreach (var detection in detections)
        {
            if (detection.gameObject.TryGetComponent(out FirstPersonController player))
            {
                target = player.transform;
            }
        }
    }

    public void RemoveTargetRef()
    {
        target = null;
    }
}
