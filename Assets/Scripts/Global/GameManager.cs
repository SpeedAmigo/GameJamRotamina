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

    }

    // Metody do kontrolowania sanity
    public void AddSanity(float amount)
    {
        currentSanity += amount;
        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);

        OnSanityChanged?.Invoke(currentSanity);
        OnSanityChangedWithMax?.Invoke(currentSanity, maxSanity);

    }

    public void RemoveSanity(float amount)
    {
        currentSanity -= amount;
        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);

        OnSanityChanged?.Invoke(currentSanity);
        OnSanityChangedWithMax?.Invoke(currentSanity, maxSanity);

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

    // Zwraca poziom sanity jako liczbę całkowitą od 0 do 4
    // 0 - dokładnie 0% / 1 - 0.01-24.99% / 2 - 25-49.99% / 3 - 50-74.99% / 4 - 75-100%
    public int GetSanityLevel()
    {
        float percentage = GetSanityPercentage();

        if (percentage == 0f)
            return 0; // Tylko dla dokładnie 0%
        else if (percentage < 25f)
            return 1; // 0.01% - 24.99%
        else if (percentage < 50f)
            return 2; // 25% - 49.99%
        else if (percentage < 75f)
            return 3; // 50% - 74.99%
        else
            return 4; // 75% - 100%
    }

    public float GetMaxSanityForActiveLevel()
    {
        float sanityLevel = GetSanityLevel();

        return Mathf.Clamp(sanityLevel * 25f, 0f, maxSanity);

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

    private void Update()
    {
        // Test: Zmniejszaj sanity co 10 sekund po wciśnięciu klawisza "Z", zwiększaj po "X"
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log($"Before sanity: {GetSanityPercentage()}");
            RemoveSanity(10f);
            Debug.Log($"After sanity: {GetSanityPercentage()}");
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log($"Before sanity: {GetSanityPercentage()}");
            AddSanity(10f); // Testowo zwiększ sanity o 10
            Debug.Log($"After sanity: {GetSanityPercentage()}");
        }

    }


}