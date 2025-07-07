using UnityEngine;
using System.Collections.Generic;

public class AmmunitionSpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private GameObject ammunitionPrefab; // Prefab amunicji
    [SerializeField] private Transform[] spawnPoints; // Miejsca do spawnu
    [SerializeField] private bool spawnOnStart = true;

    [Header("Spawn Settings")]
    [SerializeField] private int minSpawnCount = 2; // Minimum miejsc do spawnu
    [SerializeField] private int maxSpawnCount = 4; // Maximum miejsc do spawnu

    [Header("Ammo Settings")]
    [SerializeField] private int minAmmoPerPickup = 4; // Minimum naboi na pickup
    [SerializeField] private int maxAmmoPerPickup = 7; // Maximum naboi na pickup

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private List<GameObject> spawnedAmmunition = new List<GameObject>();

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnAmmunition();
        }
    }

    // Rozrzuć amunicję po poziomie
    public void SpawnAmmunition()
    {
        if (ammunitionPrefab == null)
        {
            Debug.LogError("[AmmunitionSpawner] Ammunition prefab not assigned!");
            return;
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("[AmmunitionSpawner] No spawn points assigned!");
            return;
        }

        // Wyczyść poprzednią amunicję
        ClearAmmunition();

        // Oblicz ile miejsc ma być użytych
        int maxPossibleSpawns = Mathf.Min(maxSpawnCount, spawnPoints.Length);
        int minPossibleSpawns = Mathf.Min(minSpawnCount, spawnPoints.Length);
        int spawnsToCreate = Random.Range(minPossibleSpawns, maxPossibleSpawns + 1);

        if (enableDebugLogs)
            Debug.Log($"[AmmunitionSpawner] Will spawn ammunition in {spawnsToCreate} out of {spawnPoints.Length} available points");

        // Stwórz listę wszystkich dostępnych indeksów
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null)
                availableIndices.Add(i);
        }

        // Wybierz losowe miejsca do spawnu
        List<int> selectedIndices = new List<int>();
        for (int i = 0; i < spawnsToCreate && availableIndices.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int selectedSpawnIndex = availableIndices[randomIndex];

            selectedIndices.Add(selectedSpawnIndex);
            availableIndices.RemoveAt(randomIndex); // Usuń żeby nie wybrać tego samego miejsca ponownie
        }

        // Spawn amunicji w wybranych miejscach
        foreach (int spawnIndex in selectedIndices)
        {
            Transform spawnPoint = spawnPoints[spawnIndex];

            // Stwórz pickup amunicji
            GameObject ammunition = Instantiate(ammunitionPrefab, spawnPoint.position, spawnPoint.rotation);

            // Skonfiguruj ilość amunicji (losowa w zakresie)
            AmmunitionPickup pickupScript = ammunition.GetComponent<AmmunitionPickup>();
            if (pickupScript != null)
            {
                int randomAmmoAmount = Random.Range(minAmmoPerPickup, maxAmmoPerPickup + 1);
                pickupScript.SetAmmoAmount(randomAmmoAmount);

                if (enableDebugLogs)
                    Debug.Log($"[AmmunitionSpawner] Spawned ammunition pickup at {spawnPoint.name} (index {spawnIndex}) with {randomAmmoAmount} rounds");
            }

            spawnedAmmunition.Add(ammunition);
        }

        if (enableDebugLogs)
            Debug.Log($"[AmmunitionSpawner] Successfully spawned {spawnedAmmunition.Count} ammunition pickups in random locations");
    }

    // Wyczyść poprzednią amunicję
    private void ClearAmmunition()
    {
        foreach (GameObject ammo in spawnedAmmunition)
        {
            if (ammo != null)
                Destroy(ammo);
        }
        spawnedAmmunition.Clear();
    }

    // Debug - respawn amunicji
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && enableDebugLogs)
        {
            Debug.Log("[AmmunitionSpawner] Respawning ammunition...");
            SpawnAmmunition();
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
                    // Różne kolory dla używanych i nieużywanych spawn points
                    bool isUsed = spawnedAmmunition.Count > 0; // Można to ulepszyć żeby pokazywać aktualnie używane
                    Gizmos.color = isUsed ? Color.green : Color.yellow;
                    Gizmos.DrawWireSphere(spawnPoints[i].position, 0.5f);
                    Gizmos.color = Color.white;

                    // Numer spawn point w Scene view
#if UNITY_EDITOR
                    UnityEditor.Handles.Label(spawnPoints[i].position + Vector3.up * 1, $"Ammo {i}");
#endif
                }
            }
        }
    }

    // Gettery
    public int GetSpawnedAmmunitionCount() => spawnedAmmunition.Count;
    public bool HasSpawnedAmmunition() => spawnedAmmunition.Count > 0;
    public int GetMinSpawnCount() => minSpawnCount;
    public int GetMaxSpawnCount() => maxSpawnCount;
}