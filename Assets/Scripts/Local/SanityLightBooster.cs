using UnityEngine;
using System.Collections;

public class SanityLightBooster : MonoBehaviour
{
    [Header("Sanity Boost Settings")]
    [SerializeField] private float boostDuration = 3.5f; // Czas boostu w sekundach
    private bool hasBeenUsed = false;
    private bool isActive = false;

    private void OnTriggerEnter(Collider other)
    {
        // Sprawdź czy można aktywować
        if (hasBeenUsed || isActive) return;

        // Sprawdź czy to gracz
        if (!other.CompareTag("Player")) return;

        // Aktywuj boost
        StartCoroutine(BoostSanityGradually());
    }

    private void BoostSanityInstantly()
    {
        if (GameManager.Instance == null) return;

        // hasBeenUsed = true;

        // Pobierz obecny poziom i sanity
        float currentSanity = GameManager.Instance.GetCurrentSanity();

        // Oblicz docelową sanity
        float targetSanity = GameManager.Instance.GetMaxSanityForActiveLevel();

        // Ustaw nową sanity natychmiast
        GameManager.Instance.SetSanity(targetSanity);

        Debug.Log($"Sanity boosted from {currentSanity:F1} to {targetSanity:F1} (Level {GameManager.Instance.GetSanityLevel()})");
    }

    private IEnumerator BoostSanityGradually()
    {
        if (GameManager.Instance == null) yield break;

        isActive = true;

        // Pobierz obecny poziom i sanity
        float currentSanity = GameManager.Instance.GetCurrentSanity();

        // Oblicz docelową sanity
        float targetSanity = GameManager.Instance.GetMaxSanityForActiveLevel();

        // Sprawdź czy jest co zwiększać
        if (targetSanity <= currentSanity)
        {
            Debug.Log("Sanity już na maksimum dla tego poziomu!");
            isActive = false;
            yield break;
        }

        hasBeenUsed = true;


        Debug.Log($"Starting gradual sanity boost from {currentSanity:F1} to {targetSanity:F1} over {boostDuration} seconds");

        float elapsedTime = 0f;

        // Stopniowo zwiększaj sanity
        while (elapsedTime < boostDuration)
        {
            float progress = elapsedTime / boostDuration;

            // Użyj smooth curve dla płynnego efektu
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            // Interpoluj sanity
            float newSanity = Mathf.Lerp(currentSanity, targetSanity, smoothProgress);
            Debug.Log($"Setting sanity from {GameManager.Instance.GetCurrentSanity():F1} to {newSanity:F1} (Progress: {smoothProgress * 100:F1}%)");
            GameManager.Instance.SetSanity(newSanity);

            elapsedTime += Time.deltaTime;
            yield return null; // Czekaj jedną klatkę
        }

        // Ustaw finalną wartość
        GameManager.Instance.SetSanity(targetSanity);

        Debug.Log($"Sanity boost completed! Final sanity: {GameManager.Instance.GetCurrentSanity():F1} (Level {GameManager.Instance.GetSanityLevel()})");

        isActive = false;
    }
}