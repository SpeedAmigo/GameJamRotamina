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

        CodeManager.OnCodeReset += SpawnFragments;
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

    // W CodeFragmentSpawner.cs - zmodyfikuj OnDrawGizmos():
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
                    Gizmos.color = isUsed ? Color.cyan : Color.blue;
                    Gizmos.DrawWireSphere(spawnPoint.position, 1f);

                    // Narysuj podgląd prefaba z opacity
                    if (fragmentPrefab != null)
                    {
                        DrawPrefabPreview(spawnPoint, isUsed);
                    }

                    // Numer spawn point w Scene view
#if UNITY_EDITOR
                    UnityEditor.Handles.Label(spawnPoint.position + Vector3.up * 2f, $"Fragment {i}");
#endif

                    Gizmos.color = Color.white;
                }
            }
        }
    }

    // Narysuj podgląd prefaba
    private void DrawPrefabPreview(Transform spawnPoint, bool isUsed)
    {
        // Sprawdź czy prefab ma główny MeshRenderer
        MeshRenderer prefabRenderer = fragmentPrefab.GetComponent<MeshRenderer>();
        MeshFilter prefabMeshFilter = fragmentPrefab.GetComponent<MeshFilter>();

        // Sprawdź dziecko "background"
        Transform backgroundChild = fragmentPrefab.transform.Find("background");
        MeshRenderer backgroundRenderer = null;
        MeshFilter backgroundMeshFilter = null;

        if (backgroundChild != null)
        {
            backgroundRenderer = backgroundChild.GetComponent<MeshRenderer>();
            backgroundMeshFilter = backgroundChild.GetComponent<MeshFilter>();
        }

        // Ustaw kolor podglądu
        Color previewColor = isUsed ? new Color(0, 1, 1, 0.15f) : new Color(0, 0, 1, 0.1f); // Cyan/Niebieski z opacity
        Gizmos.color = previewColor;

        // Narysuj główny mesh jeśli istnieje
        if (prefabRenderer != null && prefabMeshFilter != null && prefabMeshFilter.sharedMesh != null)
        {
            Gizmos.DrawMesh(
                prefabMeshFilter.sharedMesh,
                spawnPoint.position,
                spawnPoint.rotation,
                fragmentPrefab.transform.localScale
            );
        }

        // Narysuj background mesh jeśli istnieje
        if (backgroundRenderer != null && backgroundMeshFilter != null && backgroundMeshFilter.sharedMesh != null)
        {
            // Oblicz pozycję dziecka względem parenta
            Vector3 childLocalPos = backgroundChild.localPosition;
            Quaternion childLocalRot = backgroundChild.localRotation;
            Vector3 childLocalScale = Vector3.Scale(backgroundChild.localScale, fragmentPrefab.transform.localScale);

            // Przekształć lokalne współrzędne dziecka na światowe względem spawn pointu
            Vector3 worldChildPos = spawnPoint.position + spawnPoint.rotation * childLocalPos;
            Quaternion worldChildRot = spawnPoint.rotation * childLocalRot;

            // Zmień kolor dla background (nieco ciemniejszy)
            Gizmos.color = new Color(previewColor.r * 0.7f, previewColor.g * 0.7f, previewColor.b * 0.7f, previewColor.a * 1.2f);

            Gizmos.DrawMesh(
                backgroundMeshFilter.sharedMesh,
                worldChildPos,
                worldChildRot,
                childLocalScale
            );
        }

        // Fallback jeśli nie ma żadnego mesh
        if ((prefabRenderer == null || prefabMeshFilter == null) &&
            (backgroundRenderer == null || backgroundMeshFilter == null))
        {
            // Fallback - narysuj prosty cylinder dla fragmentu kodu
            Gizmos.color = new Color(0, 1, 1, 0.1f); // Cyan z opacity

            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(spawnPoint.position, spawnPoint.rotation, Vector3.one);

            // Narysuj cylindryczny kształt
            Gizmos.DrawCube(Vector3.zero, new Vector3(0.5f, 1f, 0.5f));

            Gizmos.matrix = oldMatrix;
        }
    }

    // Sprawdź czy spawn point jest aktualnie używany
    private bool IsSpawnPointUsed(int spawnIndex)
    {
        foreach (GameObject fragment in spawnedFragments)
        {
            if (fragment != null)
            {
                // Sprawdź czy fragment jest blisko tego spawn pointu
                float distance = Vector3.Distance(fragment.transform.position, spawnPoints[spawnIndex].position);
                if (distance < 1.5f) // Tolerancja 1.5 metra (większa niż dla amunicji)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Gettery
    public int GetSpawnedFragmentsCount() => spawnedFragments.Count;
    public bool HasSpawnedFragments() => spawnedFragments.Count > 0;
}