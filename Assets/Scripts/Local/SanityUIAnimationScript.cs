using DG.Tweening;
using UnityEngine;

public class SanityUIAnimationScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SanityManager.OnSanityChanged += OnSanityChanged;
    }


    private void OnSanityChanged(float newSanity)
    {
        CameraShake(newSanity);
    }


    private void CameraShake(float sanity)
    {
        float intensity;

        if (sanity >= 75)
            intensity = 0f;
        else if (sanity >= 50)
            intensity = 2f;
        else if (sanity >= 25)
            intensity = 5f;
        else
            intensity = 10f;


        Shake(intensity);
    }

    private void Shake(float intensity)
    {
        if (intensity <= 0f) return;

        transform.DOShakePosition(0.2f, intensity);
    }
}
