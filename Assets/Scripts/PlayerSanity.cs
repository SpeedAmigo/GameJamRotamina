using UnityEngine;

public class PlayerSanity : MonoBehaviour
{
    [SerializeField] private SanityUIScript sanityUIScript;

    private void Start()
    {
        // Subskrybuj do eventu GameManager
        GameManager.OnSanityChangedWithMax += OnSanityChanged;

        // Ustaw początkowe wartości UI
        if (sanityUIScript != null && GameManager.Instance != null)
        {
            sanityUIScript.SetMaxValue(GameManager.Instance.GetMaxSanity());
            sanityUIScript.SetCurrentValue(GameManager.Instance.GetCurrentSanity());
        }
    }

    private void OnDestroy()
    {
        // Odsubskrybuj event
        GameManager.OnSanityChangedWithMax -= OnSanityChanged;
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
        if (GameManager.Instance != null && GameManager.Instance.GetCurrentSanity() > 0)
        {
            GameManager.Instance.RemoveSanity(Time.deltaTime);
        }
    }
}