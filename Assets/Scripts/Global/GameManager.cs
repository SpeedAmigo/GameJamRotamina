using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int killerCount = 0;
    public bool isGamePaused = false;

    // Event który wywołuje się gdy killerCount się zmienia
    public static event Action<int> OnKillerCountChanged;

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
    }

    // Metody do zmiany stanu
    public void AddKillerCount(int points)
    {
        killerCount += points;

        OnKillerCountChanged?.Invoke(killerCount);

    }

    public void ResetKillerCount()
    {
        killerCount = 0;
        OnKillerCountChanged?.Invoke(killerCount);

    }
}