using System.Collections.Generic;
using RaycastPro.Detectors;
using UnityEngine;
using UnityEngine.AI;

public class SpiritScript : MonoBehaviour, IDamageAble
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;
    [SerializeField] private SightDetector sight;
    
    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootCooldown = 2f;
    [SerializeField] private float shootForce = 15f;
    
    [Header("Roaming Settings")]
    [SerializeField] private float roamRadius = 10f;
    [SerializeField] private float roamInterval = 5f;

    [SerializeField] private int health;

    [SerializeField] private GameObject dieParticle;

    private float roamTimer;
    private Vector3 startPosition;
    
    [SerializeField] private Animator animator;

    private HashSet<Collider> detections;
    private float velocity;
    private float shootTimer;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
        roamTimer = roamInterval;
        dieParticle.gameObject.SetActive(false);
    }

    private void Update()
    {
        detections = sight.DetectedColliders;
        
        velocity  = agent.velocity.magnitude;
        
        animator.SetFloat("Velocity", velocity);
        
        if (detections.Count > 0  && target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget > agent.stoppingDistance)
            {
                agent.SetDestination(target.position);
            }
            else
            {
                agent.ResetPath(); // Stop moving
                FaceTarget();      // Face the player before shooting

                shootTimer -= Time.deltaTime;

                if (shootTimer <= 0f)
                {
                    ShootProjectile();
                    shootTimer = shootCooldown;
                }
            }
            
            shootTimer -= Time.deltaTime;
            
            if (shootTimer <= 0f)
            {
                ShootProjectile();
                shootTimer = shootCooldown;
            }
            
        }
        else
        {
            Roam();
        }
    }
    
    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // Keep rotation flat on the Y axis
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
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
    
    private void Roam()
    {
        roamTimer -= Time.deltaTime;

        if (roamTimer <= 0f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection += startPosition;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, roamRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(navHit.position);
            }

            roamTimer = roamInterval;
        }
    }

    public void RemoveTargetRef()
    {
        target = null;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        dieParticle.SetActive(true);
        Destroy(gameObject, 0.2f);
    }
}
