using System.Collections;
using FMODUnity;
using UnityEngine;

public class DoorScript : InteractionAbstract
{
    [SerializeField] private bool isRotating;
    [SerializeField] private bool doorsOpened = false;
    [SerializeField] private float rotationSpeed;
    
    FMOD.Studio.EventInstance DoorsSound;
    public EventReference DoorsEvent;
    
    public override void Interact(PlayerScript player)
    {
        if (doorsOpened == true)
        {
            StartCoroutine(CloseOverTime());
            PlaySound();
            doorsOpened = false;
            //RoomsSnap();
        }
        else
        {
            StartCoroutine(OpenOverTime());
            PlaySound();
            doorsOpened = true;
            
            //RoomsSnap();
        }
    }
    
    private void PlaySound()
    {
        DoorsSound = FMODUnity.RuntimeManager.CreateInstance(DoorsEvent);
        DoorsSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
        
        if (doorsOpened == true)
        {            
            DoorsSound.setParameterByNameWithLabel("doorParameter", "Close");
        }
        else
        {
            DoorsSound.setParameterByNameWithLabel("doorParameter", "Open");
        }
        
        DoorsSound.start();
        DoorsSound.release();
    }
    
    private IEnumerator CloseOverTime()
    {
        isRotating = true;
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 95, 0) * startRotation; // Rotating around Y-axis by 90 degrees

        while (elapsedTime < 1f) // Rotate over 1 second
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * rotationSpeed / 90f; // Normalizing to 90 degrees rotation
            yield return null;
        }

        // Ensure the rotation is exactly what we want at the end
        transform.rotation = targetRotation;
        isRotating = false;
        //roomEventOpen?.Invoke();
    }
    
    private IEnumerator OpenOverTime()
    {
        isRotating = true;
        float elapsedTime = 0f;
        //roomEventClose?.Invoke();
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, -95, 0) * startRotation; // Rotating around Y-axis by 90 degrees

        while (elapsedTime < 1f) // Rotate over 1 second
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * rotationSpeed / 90f; // Normalizing to 90 degrees rotation
            yield return null;
        }

        // Ensure the rotation is exactly what we want at the end
        transform.rotation = targetRotation;
        isRotating = false;
    }

}
