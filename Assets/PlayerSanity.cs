using System;
using UnityEngine;

public class PlayerSanity : MonoBehaviour
{
    [SerializeField] private float currentSanity;
    [SerializeField] private float maxSanity;
    
    [SerializeField] private SanityUIScript sanityUIScript;

    private void Start()
    {
        sanityUIScript.SetMaxValue(maxSanity);
    }

    private void Update()
    {
        if (currentSanity > 0)
        {
            currentSanity -= Time.deltaTime;
            sanityUIScript.SetCurrentValue(currentSanity);
        }
    }

    public float GetCurrentSanity()
    {
        return currentSanity;
    }

    public float GetMaxSanity()
    {
        return maxSanity;
    }

    public void SetSanity(float newSanity)
    {
        currentSanity = newSanity > maxSanity ? maxSanity : newSanity;
    }

    public void SetMaxSanity(float newMaxSanity)
    {
        maxSanity = newMaxSanity;
    }
}
