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
    
    [SerializeField] private Animator animator;

    private HashSet<Collider> detections;
    private float velocity;
    
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
        }
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
