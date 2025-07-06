using UnityEngine;
using System.Collections;

public class SanityLightBooster : MonoBehaviour
{
    [Header("Sanity Boost Settings")]
    [SerializeField] private float boostDuration = 3.5f; // Czas boostu w sekundach
    [SerializeField] private float boostCapacity = 25f; // Ile sanity może dać łącznie

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true; // Włącz/wyłącz debug logi

    private float remainingCapacity;
    private bool isActive = false;
    private bool playerInRange = false;
    private Coroutine boostCoroutine;

    // Auto-znajdowane komponenty
    private Light lightComponent;
    private Renderer rendererComponent;
    private float originalLightIntensity;
    private float originalLightRange;
    private Color originalColor;

    private void Start()
    {
        // Znajdź Light i Renderer na tym samym obiekcie
        lightComponent = GetComponentInChildren<Light>();
        rendererComponent = GetComponent<Renderer>();

        Debug.Log($"Light Component {lightComponent}");
        Debug.Log($"Snaity Manager {SanityManager.Instance}");

        // Zapisz oryginalne wartości
        if (lightComponent != null)
        {
            originalLightIntensity = lightComponent.intensity;
            originalLightRange = lightComponent.range;

            if (enableDebugLogs)
                Debug.Log($"[SanityLightBooster] Found Light with intensity: {originalLightIntensity}");
        }
        else
        {
            if (enableDebugLogs)
                Debug.LogWarning("[SanityLightBooster] No Light component found on this GameObject!");
        }

        if (rendererComponent != null)
        {
            originalColor = rendererComponent.material.color;
            if (enableDebugLogs)
                Debug.Log($"[SanityLightBooster] Found Renderer with color: {originalColor}");
        }

        // Inicjalizuj remaining capacity
        remainingCapacity = boostCapacity;
        UpdateVisuals();

        if (enableDebugLogs)
            Debug.Log($"[SanityLightBooster] Initialized with capacity: {remainingCapacity:F1}/{boostCapacity:F1}");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Sprawdź czy to gracz
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (enableDebugLogs)
            Debug.Log($"[SanityLightBooster] Player entered area. Remaining capacity: {remainingCapacity:F1}/{boostCapacity:F1}");

        // Sprawdź czy może aktywować boost
        if (CanActivateBoost())
        {
            StartBoost();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Sprawdź czy to gracz
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (enableDebugLogs)
            Debug.Log($"[SanityLightBooster] Player left area. Remaining capacity: {remainingCapacity:F1}/{boostCapacity:F1}");

        // Zatrzymaj boost jeśli aktywny
        if (isActive)
        {
            StopBoost();
        }
    }

    private bool CanActivateBoost()
    {
        // Nie można aktywować jeśli:
        if (isActive)
        {
            if (enableDebugLogs)
                Debug.Log("[SanityLightBooster] Cannot activate - already active");
            return false;
        }

        if (remainingCapacity <= 0f)
        {
            if (enableDebugLogs)
                Debug.Log("[SanityLightBooster] Cannot activate - no capacity remaining");
            return false;
        }

        if (SanityManager.Instance == null)
        {
            if (enableDebugLogs)
                Debug.LogError("[SanityLightBooster] Cannot activate - SanityManager not found!");
            return false;
        }

        // Sprawdź czy sanity nie jest już na maksimum dla poziomu
        float currentSanity = SanityManager.Instance.GetCurrentSanity();
        float targetSanity = SanityManager.Instance.GetMaxSanityForActiveLevel();

        if (targetSanity <= currentSanity)
        {
            if (enableDebugLogs)
                Debug.Log($"[SanityLightBooster] Cannot activate - sanity already at maximum for level ({currentSanity:F1}/{targetSanity:F1})");
            return false;
        }

        if (enableDebugLogs)
            Debug.Log($"[SanityLightBooster] Can activate boost! Current sanity: {currentSanity:F1}, Target: {targetSanity:F1}, Capacity: {remainingCapacity:F1}");

        return true;
    }

    private void StartBoost()
    {
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
        }

        if (enableDebugLogs)
            Debug.Log($"[SanityLightBooster] Starting boost with {remainingCapacity:F1} capacity remaining");

        boostCoroutine = StartCoroutine(BoostSanityGradually());
    }

    private void StopBoost()
    {
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
            boostCoroutine = null;
        }

        isActive = false;

        if (enableDebugLogs)
            Debug.Log($"[SanityLightBooster] Boost stopped - player left area. Remaining capacity: {remainingCapacity:F1}/{boostCapacity:F1}");
    }

    private IEnumerator BoostSanityGradually()
    {
        if (SanityManager.Instance == null) yield break;

        isActive = true;

        // Pobierz obecny poziom i sanity
        float startSanity = SanityManager.Instance.GetCurrentSanity();
        float targetSanity = SanityManager.Instance.GetMaxSanityForActiveLevel();

        // Oblicz ile sanity faktycznie możemy dodać (ograniczone przez capacity)
        float maxPossibleBoost = targetSanity - startSanity;
        float actualBoost = Mathf.Min(maxPossibleBoost, remainingCapacity);
        float finalTargetSanity = startSanity + actualBoost;

        if (enableDebugLogs)
            Debug.Log($"[SanityLightBooster] Starting boost: {startSanity:F1} → {finalTargetSanity:F1} (Max possible: {maxPossibleBoost:F1}, Limited by capacity: {actualBoost:F1})");

        float elapsedTime = 0f;
        float usedCapacity = 0f;
        float lastSanity = startSanity;

        // Stopniowo zwiększaj sanity
        while (elapsedTime < boostDuration && playerInRange && remainingCapacity > 0f)
        {
            float progress = elapsedTime / boostDuration;
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            // Oblicz nową sanity
            float newSanity = Mathf.Lerp(startSanity, finalTargetSanity, smoothProgress);
            float capacityUsedThisFrame = newSanity - lastSanity;

            // Sprawdź czy mamy wystarczająco capacity
            if (capacityUsedThisFrame > remainingCapacity)
            {
                capacityUsedThisFrame = remainingCapacity;
                newSanity = lastSanity + capacityUsedThisFrame;

                if (enableDebugLogs)
                    Debug.Log($"[SanityLightBooster] Capacity limit reached! Using remaining: {capacityUsedThisFrame:F2}");
            }

            // Ustaw nową sanity i zaktualizuj capacity
            SanityManager.Instance.SetSanity(newSanity);
            remainingCapacity -= capacityUsedThisFrame;
            usedCapacity += capacityUsedThisFrame;
            lastSanity = newSanity;

            // Zaktualizuj wizualne efekty w real-time
            UpdateVisuals();

            // Debug co sekundę
            if (enableDebugLogs && Mathf.FloorToInt(elapsedTime) != Mathf.FloorToInt(elapsedTime - Time.deltaTime))
            {
                Debug.Log($"[SanityLightBooster] Progress {progress * 100:F0}%: Sanity {newSanity:F1} (+{usedCapacity:F1}), Capacity left: {remainingCapacity:F1}");
            }

            // Sprawdź czy capacity się skończyła
            if (remainingCapacity <= 0f)
            {
                if (enableDebugLogs)
                    Debug.Log($"[SanityLightBooster] Boost capacity completely depleted! Total used: {usedCapacity:F1}");
                break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (enableDebugLogs)
        {
            string reason = !playerInRange ? "player left area" :
                           remainingCapacity <= 0f ? "capacity depleted" : "duration completed";
            Debug.Log($"[SanityLightBooster] Boost ended ({reason}). Total capacity used: {usedCapacity:F1}, Remaining: {remainingCapacity:F1}/{boostCapacity:F1}");
            Debug.Log($"[SanityLightBooster] Final sanity: {SanityManager.Instance.GetCurrentSanity():F1} (Level {SanityManager.Instance.GetSanityLevel()})");
        }

        isActive = false;
        boostCoroutine = null;

        // Zaktualizuj wizualne efekty
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        // Oblicz procent remaining capacity (0.0 - 1.0)
        float capacityPercent = remainingCapacity / boostCapacity;

        // Aktualizuj Light intensity jeśli istnieje
        if (lightComponent != null)
        {
            lightComponent.intensity = originalLightIntensity * capacityPercent;
            lightComponent.range = originalLightRange * capacityPercent;

            if (enableDebugLogs && capacityPercent <= 0.1f)
                Debug.Log($"[SanityLightBooster] Light intensity: {lightComponent.intensity:F2} ({capacityPercent * 100:F0}% capacity)");
        }

        // Aktualizuj Renderer alpha/color jeśli istnieje  
        if (rendererComponent != null)
        {
            Color newColor = originalColor;
            newColor.a = capacityPercent; // Zmień alpha bazując na capacity
            rendererComponent.material.color = newColor;
        }
    }

    // Gettery
    public float GetRemainingCapacity() => remainingCapacity;
    public float GetMaxCapacity() => boostCapacity;
    public float GetCapacityPercentage() => (remainingCapacity / boostCapacity) * 100f;
    public bool IsActive() => isActive;
    public bool IsPlayerInRange() => playerInRange;

    // Debug info
    private void OnDrawGizmos()
    {
        // Pokaż trigger area
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Gizmos.color = remainingCapacity <= 0f ? Color.red :
                          isActive ? Color.green : Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;

            if (col is BoxCollider boxCol)
            {
                Gizmos.DrawWireCube(boxCol.center, boxCol.size);
            }
            else if (col is SphereCollider sphereCol)
            {
                Gizmos.DrawWireSphere(sphereCol.center, sphereCol.radius);
            }
        }
    }

    // Debug w Update  
    private void Update()
    {
        // Debug capacity status
        if (Input.GetKeyDown(KeyCode.C) && enableDebugLogs)
        {
            Debug.Log($"[SanityLightBooster] Status: Capacity {remainingCapacity:F1}/{boostCapacity:F1} ({GetCapacityPercentage():F1}%), Active: {isActive}, Player in range: {playerInRange}");
        }
    }
}