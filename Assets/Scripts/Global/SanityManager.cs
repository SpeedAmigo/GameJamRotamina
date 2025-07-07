using System;
using UnityEngine;

public class SanityManager : MonoBehaviour
{
    public static SanityManager Instance { get; private set; }

    [SerializeField] private GameObject deathCanvas;

    [Header("Sanity Configuration")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float currentSanity = 100f;
    [SerializeField] private int killsPerPercentSanity = 2; // Co ile killów = 1% sanity

    // Eventy
    public static event Action<float> OnSanityChanged; // Przekazuje currentSanity
    public static event Action<float, float> OnSanityChangedWithMax; // Przekazuje (current, max)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ustaw początkową sanity
        currentSanity = maxSanity;
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

        if (currentSanity <= 0)
        {
            deathCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }

    public void SetSanity(float newSanity)
    {
        currentSanity = Mathf.Clamp(newSanity, 0f, maxSanity);

        OnSanityChanged?.Invoke(currentSanity);
        OnSanityChangedWithMax?.Invoke(currentSanity, maxSanity);
    }

    // Gettery
    public float GetCurrentSanity() => currentSanity;
    public float GetMaxSanity() => maxSanity;
    public float GetSanityPercentage() => (currentSanity / maxSanity) * 100f;
    public int GetKillsPerPercentSanity() => killsPerPercentSanity;

    // Zwraca poziom sanity jako liczbę całkowitą od 0 do 4
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

    // Debug
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log($"Before sanity: {GetSanityPercentage()}");
            RemoveSanity(10f);
            Debug.Log($"After sanity: {GetSanityPercentage()}");
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log($"Before sanity: {GetSanityPercentage()}");
            AddSanity(10f);
            Debug.Log($"After sanity: {GetSanityPercentage()}");
        }
    }
}