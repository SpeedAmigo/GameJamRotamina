using UnityEngine;

public class SaturationController : MonoBehaviour
{
    private Renderer objectRenderer;
    private Material originalMaterial;
    private Material saturationMaterial;
    private static readonly int SaturationProperty = Shader.PropertyToID("_Saturation");
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");
    private static readonly int MainTexProperty = Shader.PropertyToID("_MainTex");

    private void Start()
    {
        // Pobierz Renderer z tego obiektu
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer == null)
        {
            Debug.LogError($"SaturationController: No Renderer found on {gameObject.name}!");
            return;
        }

        // Zapisz oryginalny materiał
        originalMaterial = objectRenderer.sharedMaterial;

        // Znajdź materiał z saturation shaderem
        saturationMaterial = FindSaturationMaterial();

        if (saturationMaterial == null)
        {
            Debug.LogError($"SaturationController: No material with SaturationOverlay shader found on {gameObject.name}!");
            return;
        }

        // Skopiuj właściwości z oryginalnego materiału
        CopyMaterialProperties();

        // Ustaw saturation material
        objectRenderer.material = saturationMaterial;

        // Subskrybuj do eventu sanity
        GameManager.OnSanityChanged += OnSanityChanged;

        // Ustaw początkową saturację na podstawie sanity
        UpdateSaturation(GameManager.Instance?.GetCurrentSanity() ?? 0f);
    }

    private void OnEnable()
    {
        // Subskrybuj przy włączeniu obiektu
        GameManager.OnSanityChanged += OnSanityChanged;
    }

    private void OnDisable()
    {
        // Odsubskrybuj przy wyłączeniu obiektu
        GameManager.OnSanityChanged -= OnSanityChanged;
    }

    private void OnDestroy()
    {
        // Odsubskrybuj przy niszczeniu obiektu
        GameManager.OnSanityChanged -= OnSanityChanged;

        if (saturationMaterial != null)
        {
            Destroy(saturationMaterial);
        }
    }

    // Metoda wywoływana przez event
    private void OnSanityChanged(float currentSanity)
    {
        UpdateSaturation(currentSanity);
        Debug.Log($"SaturationController on {gameObject.name} received sanity: {currentSanity}");
    }

    private Material FindSaturationMaterial()
    {
        // Sprawdź czy obecny materiał ma odpowiedni shader
        if (originalMaterial != null && originalMaterial.shader.name == "Custom/SaturationOverlay")
        {
            return new Material(originalMaterial); // Stwórz kopię
        }

        // Jeśli nie, znajdź shader i stwórz nowy materiał
        Shader satShader = Shader.Find("Custom/SaturationOverlay");
        if (satShader != null)
        {
            return new Material(satShader);
        }

        Debug.LogError("Custom/SaturationOverlay shader not found!");
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
            Debug.Log($"Copied color: {originalColor} from {gameObject.name}");
        }

        // Skopiuj główną teksturę
        if (originalMaterial.HasProperty("_MainTex"))
        {
            Texture originalTexture = originalMaterial.GetTexture("_MainTex");
            saturationMaterial.SetTexture(MainTexProperty, originalTexture);
            Debug.Log($"Copied texture: {originalTexture?.name} from {gameObject.name}");
        }
    }

    private void UpdateSaturation(float currentSanity)
    {
        if (saturationMaterial == null || GameManager.Instance == null) return;

        // Saturacja = currentSanity / maxSanity (0 = grayscale, 1 = full color)
        float maxSanity = GameManager.Instance.GetMaxSanity();
        float saturation = Mathf.Clamp01(currentSanity / maxSanity);
        saturationMaterial.SetFloat(SaturationProperty, saturation);

        Debug.Log($"Updated saturation to: {saturation:F2} on {gameObject.name} (Sanity: {currentSanity:F1}/{maxSanity})");
    }
}