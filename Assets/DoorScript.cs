using System.Collections;
using UnityEngine;

public class DoorScript : InteractionAbstract
{
    [SerializeField] private bool isRotating;
    [SerializeField] private bool doorsOpened;
    [SerializeField] private float rotationSpeed;
    
    public override void Interact(PlayerScript player)
    {
        if (doorsOpened)
        {
            StartCoroutine(CloseOverTime());
            //PlaySound();
            doorsOpened = false;
            //RoomsSnap();
        }
        else
        {
            StartCoroutine(OpenOverTime());
            //PlaySound();
            doorsOpened = true;
            
            //RoomsSnap();
        }
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
        Quaternion targetRotation = Quaternion.Euler(0, -65, 0) * startRotation; // Rotating around Y-axis by 90 degrees

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
