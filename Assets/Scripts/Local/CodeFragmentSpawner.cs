using UnityEngine;
using System.Collections.Generic;

public class CodeFragmentSpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private GameObject fragmentPrefab; // Prefab fragmentu
    [SerializeField] private Transform[] spawnPoints; // 6 miejsc do spawnu
    [SerializeField] private bool spawnOnStart = true;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private List<GameObject> spawnedFragments = new List<GameObject>();

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnFragments();
        }
    }

    // Rozrzuć fragmenty po poziomie
    public void SpawnFragments()
    {
        // Sprawdź wymagania
        if (CodeManager.Instance == null)
        {
            Debug.LogError("[CodeFragmentSpawner] CodeManager not found!");
            return;
        }

        if (fragmentPrefab == null)
        {
            Debug.LogError("[CodeFragmentSpawner] Fragment prefab not assigned!");
            return;
        }

        if (spawnPoints.Length < 3)
        {
            Debug.LogError($"[CodeFragmentSpawner] Need at least 3 spawn points, but only {spawnPoints.Length} assigned!");
            return;
        }

        // Pobierz kod z CodeManager
        List<int> codeDigits = CodeManager.Instance.GetCodeDigits();

        if (codeDigits.Count == 0)
        {
            Debug.LogError("[CodeFragmentSpawner] No code available! Make sure CodeManager generated code first.");
            return;
        }

        // Wyczyść poprzednie fragmenty
        ClearFragments();

        // Wybierz losowe 3 miejsca z dostępnych spawn points
        List<int> selectedSpawnIndices = GetRandomSpawnIndices(3, spawnPoints.Length);

        // Spawn fragmentów w wybranych miejscach
        for (int i = 0; i < codeDigits.Count; i++)
        {
            int spawnIndex = selectedSpawnIndices[i];
            Transform spawnPoint = spawnPoints[spawnIndex];

            // Stwórz fragment
            GameObject fragment = Instantiate(fragmentPrefab, spawnPoint.position, spawnPoint.rotation);

            // Skonfiguruj fragment
            CodeFragment fragmentScript = fragment.GetComponent<CodeFragment>();
            if (fragmentScript != null)
            {
                fragmentScript.SetupFragment(i, codeDigits[i]); // i = pozycja w kodzie (0,1,2)
            }

            spawnedFragments.Add(fragment);

            if (enableDebugLogs)
                Debug.Log($"[CodeFragmentSpawner] Spawned fragment {i} (digit: {codeDigits[i]}) at spawn point {spawnIndex} ({spawnPoint.name})");
        }

        if (enableDebugLogs)
            Debug.Log($"[CodeFragmentSpawner] Successfully spawned {codeDigits.Count} fragments for code: {CodeManager.Instance.GetCode()}");
    }

    // Algorytm wyboru losowych spawn points
    private List<int> GetRandomSpawnIndices(int count, int totalSpawnPoints)
    {
        // Stwórz listę wszystkich dostępnych indeksów
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < totalSpawnPoints; i++)
        {
            availableIndices.Add(i);
        }

        // Wybierz losowo 'count' indeksów bez powtórzeń
        List<int> selectedIndices = new List<int>();
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            selectedIndices.Add(availableIndices[randomIndex]);
            availableIndices.RemoveAt(randomIndex); // Usuń żeby nie powtórzyć
        }

        return selectedIndices;
    }

    // Wyczyść poprzednie fragmenty
    private void ClearFragments()
    {
        foreach (GameObject fragment in spawnedFragments)
        {
            if (fragment != null)
                Destroy(fragment);
        }
        spawnedFragments.Clear();
    }

    // Debug - respawn fragmentów
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && enableDebugLogs)
        {
            Debug.Log("[CodeFragmentSpawner] Respawning fragments...");
            SpawnFragments();
        }
    }

    // Gizmos do pokazania spawn points
    private void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i] != null)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(spawnPoints[i].position, 1f);
                    Gizmos.color = Color.white;

                    // Numer spawn point w Scene view
#if UNITY_EDITOR
                    UnityEditor.Handles.Label(spawnPoints[i].position + Vector3.up * 2, $"Spawn {i}");
#endif
                }
            }
        }
    }

    // Gettery
    public int GetSpawnedFragmentsCount() => spawnedFragments.Count;
    public bool HasSpawnedFragments() => spawnedFragments.Count > 0;
}