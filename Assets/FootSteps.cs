using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class Footsteps : MonoBehaviour
{
    FMOD.Studio.EventInstance FootstepsSound;

    public EventReference footstepsEvent;

    private float lastFootstepTime = 0f;
    private float distToGround;
    private bool isGrounded = true;
    private bool isJumping = false;
    
    private void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }
    
    private void FixedUpdate()
    {
        // Footsteps
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            if (IsGrounded() && Time.time - lastFootstepTime > 0.5f)
            {
                lastFootstepTime = Time.time;
                PlayFootsteps();
            }
        }         
        
        // Running
        if((Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") != 0) || (Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") != 0))
        {
            if (IsGrounded() && Time.time - lastFootstepTime > 0.4f)
            {
                lastFootstepTime = Time.time;
                PlayFootsteps();
            }
        }
    }

    private void PlayFootsteps()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distToGround + 0.5f))
        {
            if (hit.collider.CompareTag("Stone"))
            {
                FootstepsSound = FMODUnity.RuntimeManager.CreateInstance(footstepsEvent);
                FootstepsSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
                FootstepsSound.setParameterByNameWithLabel("FootSwitcher", "Stone");
                FootstepsSound.start();
                FootstepsSound.release();
            }
        }       
    }
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.5f);
    }  
}