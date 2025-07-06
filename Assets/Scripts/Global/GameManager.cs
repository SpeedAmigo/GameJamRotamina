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

    private void Update()
    {
        // Test - zwiększaj killerCount klawiszem K
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddKillerCount(1);
            Debug.Log($"Killer Count: {killerCount}");
        }

        // Reset klawiszem R
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetKillerCount();
        }
    }

    public void ResetKillerCount()
    {
        killerCount = 0;
        OnKillerCountChanged?.Invoke(killerCount);
        Debug.Log("Killer Count reset to 0");


    }
}