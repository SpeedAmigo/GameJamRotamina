using UnityEngine;

public class SaturationController : MonoBehaviour
{
    [Header("Saturation Settings")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private int maxKillerCount = 5;

    private Material originalMaterial;
    private Material saturationMaterial;
    private static readonly int SaturationProperty = Shader.PropertyToID("_Saturation");
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");
    private static readonly int MainTexProperty = Shader.PropertyToID("_MainTex");

    private void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer == null)
        {
            Debug.LogError("No Renderer found!");
            return;
        }

        // Zapisz oryginalny materiał
        originalMaterial = targetRenderer.sharedMaterial;

        // Znajdź materiał z saturation shaderem
        saturationMaterial = FindSaturationMaterial();

        if (saturationMaterial == null)
        {
            Debug.LogError("No material with SaturationOverlay shader found!");
            return;
        }

        // Skopiuj właściwości z oryginalnego materiału
        CopyMaterialProperties();

        // Ustaw saturation material
        targetRenderer.material = saturationMaterial;

        // Subskrybuj do eventu
        GameManager.OnKillerCountChanged += OnKillerCountChanged;

        // Ustaw początkową saturację
        UpdateSaturation(GameManager.Instance?.killerCount ?? 0);
    }

    private void OnEnable()
    {
        // Subskrybuj przy włączeniu obiektu
        GameManager.OnKillerCountChanged += OnKillerCountChanged;
    }

    private void OnDisable()
    {
        // Odsubskrybuj przy wyłączeniu obiektu
        GameManager.OnKillerCountChanged -= OnKillerCountChanged;
    }

    private void OnDestroy()
    {
        // Odsubskrybuj przy niszczeniu obiektu
        GameManager.OnKillerCountChanged -= OnKillerCountChanged;

        if (saturationMaterial != null)
        {
            Destroy(saturationMaterial);
        }
    }

    // Metoda wywoływana przez event
    private void OnKillerCountChanged(int newKillerCount)
    {
        UpdateSaturation(newKillerCount);
        Debug.Log($"SaturationController received killer count: {newKillerCount}");
    }

    private Material FindSaturationMaterial()
    {
        // Znajdź materiał z odpowiednim shaderem w Renderer
        Material[] materials = targetRenderer.materials;

        foreach (Material mat in materials)
        {
            if (mat.shader.name == "Custom/SaturationOverlay")
            {
                return new Material(mat); // Stwórz kopię
            }
        }

        // Jeśli nie znaleziono, stwórz nowy
        Shader satShader = Shader.Find("Custom/SaturationOverlay");
        if (satShader != null)
        {
            return new Material(satShader);
        }

        return null;
    }

    private void CopyMaterialProperties()
    {
        if (originalMaterial == null || saturationMaterial == null) return;

        // Skopiuj kolor
        if (originalMaterial.HasProperty("_Color"))
        {
            Color originalColor = originalMaterial.GetColor("_Color");
            saturationMaterial.SetColor(ColorProperty, originalColor);
            Debug.Log($"Copied color: {originalColor}");
        }

        // Skopiuj główną teksturę
        if (originalMaterial.HasProperty("_MainTex"))
        {
            Texture originalTexture = originalMaterial.GetTexture("_MainTex");
            saturationMaterial.SetTexture(MainTexProperty, originalTexture);
            Debug.Log($"Copied texture: {originalTexture?.name}");
        }
    }

    private void UpdateSaturation(int currentKillerCount)
    {
        if (saturationMaterial == null) return;

        float saturation = Mathf.Clamp01((float)currentKillerCount / maxKillerCount);
        saturationMaterial.SetFloat(SaturationProperty, saturation);

        Debug.Log($"Updated saturation to: {saturation} (Kills: {currentKillerCount})");
    }
}