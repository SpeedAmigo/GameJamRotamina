using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int killerCount = 0;
    public float currentSanity = 100f;
    public bool isGamePaused = false;

    [Header("Sanity Configuration")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private int killsPerPercentSanity = 2; // Co ile killów = 1% sanity

    // Eventy
    public static event Action<int> OnKillerCountChanged;
    public static event Action<float> OnSanityChanged; // Przekazuje currentSanity
    public static event Action<float, float> OnSanityChangedWithMax; // Przekazuje (current, max)

    private void Awake()
    {
        // Zapewnij, że jest tylko jedna instancja
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Przetrwa między scenami

        // Ustaw początkową sanity
        currentSanity = maxSanity;
    }

    // Metody do zmiany stanu
    public void AddKillerCount(int points)
    {
        killerCount += points;
        OnKillerCountChanged?.Invoke(killerCount);

        // Oblicz sanity za zabójstwo
        float onePercentSanity = maxSanity / 100f;
        float sanityPerKill = onePercentSanity / killsPerPercentSanity;
        float sanityBonus = points * sanityPerKill;

        AddSanity(sanityBonus);

        Debug.Log($"Added {sanityBonus:F1} sanity for {points} kills. Current: {currentSanity:F1}/{maxSanity}");
    }

    // Metody do kontrolowania sanity
    public void AddSanity(float amount)
    {
        currentSanity += amount;
        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);

        OnSanityChanged?.Invoke(currentSanity);
        OnSanityChangedWithMax?.Invoke(currentSanity, maxSanity);

        Debug.Log($"Added {amount} sanity. Current: {currentSanity:F1}/{maxSanity}");
    }

    public void RemoveSanity(float amount)
    {
        currentSanity -= amount;
        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);

        OnSanityChanged?.Invoke(currentSanity);
        OnSanityChangedWithMax?.Invoke(currentSanity, maxSanity);

        Debug.Log($"Removed {amount} sanity. Current: {currentSanity:F1}/{maxSanity}");
    }

    public void SetSanity(float newSanity)
    {
        currentSanity = Mathf.Clamp(newSanity, 0f, maxSanity);

        OnSanityChanged?.Invoke(currentSanity);
        OnSanityChangedWithMax?.Invoke(currentSanity, maxSanity);
    }

    // Gettery
    public float GetCurrentSanity()
    {
        return currentSanity;
    }

    public float GetMaxSanity()
    {
        return maxSanity;
    }

    public float GetSanityPercentage()
    {
        return (currentSanity / maxSanity) * 100f;
    }

    public void ResetKillerCount()
    {
        killerCount = 0;
        OnKillerCountChanged?.Invoke(killerCount);
        Debug.Log("Killer Count reset to 0");
    }


}