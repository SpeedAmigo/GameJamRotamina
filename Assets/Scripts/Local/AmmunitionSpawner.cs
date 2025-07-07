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
                    Transform spawnPoint = spawnPoints[i];

                    // Różne kolory dla używanych i nieużywanych spawn points
                    bool isUsed = IsSpawnPointUsed(i);
                    Gizmos.color = isUsed ? Color.green : Color.yellow;
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);

                    // Narysuj podgląd prefaba z opacity
                    if (ammunitionPrefab != null)
                    {
                        DrawPrefabPreview(spawnPoint, isUsed);
                    }

                    // Numer spawn point w Scene view
#if UNITY_EDITOR
                    UnityEditor.Handles.Label(spawnPoint.position + Vector3.up * 1.5f, $"Ammo {i}");
#endif

                    Gizmos.color = Color.white;
                }
            }
        }
    }

    // Narysuj podgląd prefaba
    private void DrawPrefabPreview(Transform spawnPoint, bool isUsed)
    {
        // Sprawdź czy prefab ma MeshRenderer
        MeshRenderer prefabRenderer = ammunitionPrefab.GetComponent<MeshRenderer>();
        MeshFilter prefabMeshFilter = ammunitionPrefab.GetComponent<MeshFilter>();

        if (prefabRenderer != null && prefabMeshFilter != null && prefabMeshFilter.sharedMesh != null)
        {
            // Ustaw przezroczystość i kolor
            Color previewColor = isUsed ? new Color(0, 1, 0, 0.1f) : new Color(1, 1, 0, 0.1f); // Zielony/Żółty z opacity 0.1
            Gizmos.color = previewColor;

            // Narysuj mesh prefaba
            Gizmos.DrawMesh(
                prefabMeshFilter.sharedMesh,
                spawnPoint.position,
                spawnPoint.rotation,
                ammunitionPrefab.transform.localScale
            );
        }
        else
        {
            // Fallback - narysuj prosty box jeśli nie ma mesh
            Gizmos.color = new Color(1, 0, 1, 0.1f); // Różowy z opacity
            Gizmos.DrawCube(spawnPoint.position, Vector3.one * 0.3f);
        }
    }

    // Sprawdź czy spawn point jest aktualnie używany
    private bool IsSpawnPointUsed(int spawnIndex)
    {
        foreach (GameObject ammo in spawnedAmmunition)
        {
            if (ammo != null)
            {
                // Sprawdź czy amunicja jest blisko tego spawn pointu
                float distance = Vector3.Distance(ammo.transform.position, spawnPoints[spawnIndex].position);
                if (distance < 1f) // Tolerancja 1 metr
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Gettery
    public int GetSpawnedAmmunitionCount() => spawnedAmmunition.Count;
    public bool HasSpawnedAmmunition() => spawnedAmmunition.Count > 0;
    public int GetMinSpawnCount() => minSpawnCount;
    public int GetMaxSpawnCount() => maxSpawnCount;
}