using UnityEngine;

public class PlayerSanity : MonoBehaviour
{
    [SerializeField] private SanityUIScript sanityUIScript;

    private void Start()
    {
        // Subskrybuj do eventu SanityManager
        SanityManager.OnSanityChangedWithMax += OnSanityChanged;

        // Ustaw początkowe wartości UI
        if (sanityUIScript != null && SanityManager.Instance != null)
        {
            sanityUIScript.SetMaxValue(SanityManager.Instance.GetMaxSanity());
            sanityUIScript.SetCurrentValue(SanityManager.Instance.GetCurrentSanity());
        }
    }

    private void OnDestroy()
    {
        // Odsubskrybuj event
        SanityManager.OnSanityChangedWithMax -= OnSanityChanged;
    }

    private void OnSanityChanged(float currentSanity, float maxSanity)
    {
        if (sanityUIScript != null)
        {
            sanityUIScript.SetMaxValue(maxSanity);
            sanityUIScript.SetCurrentValue(currentSanity);
        }
    }

    // Opcjonalnie - nadal możesz mieć Update dla spadania sanity w czasie
    private void Update()
    {
        if (SanityManager.Instance != null && SanityManager.Instance.GetCurrentSanity() > 0)
        {
            SanityManager.Instance.RemoveSanity(Time.deltaTime);
        }
    }
}