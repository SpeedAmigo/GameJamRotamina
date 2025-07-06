using System;
using UnityEngine;

public class KillerManager : MonoBehaviour
{
    public static KillerManager Instance { get; private set; }

    [Header("Killer Configuration")]
    [SerializeField] private int killerCount = 0;

    // Eventy
    public static event Action<int> OnKillerCountChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddKillerCount(int points)
    {
        killerCount += points;
        OnKillerCountChanged?.Invoke(killerCount);

        // Oblicz sanity bonus i dodaj przez SanityManager
        if (SanityManager.Instance != null)
        {
            float onePercentSanity = SanityManager.Instance.GetMaxSanity() / 100f;
            float sanityPerKill = onePercentSanity / SanityManager.Instance.GetKillsPerPercentSanity();
            float sanityBonus = points * sanityPerKill;

            SanityManager.Instance.AddSanity(sanityBonus);
        }
    }

    public void ResetKillerCount()
    {
        killerCount = 0;
        OnKillerCountChanged?.Invoke(killerCount);
        Debug.Log("Killer Count reset to 0");
    }

    public int GetKillerCount() => killerCount;
}