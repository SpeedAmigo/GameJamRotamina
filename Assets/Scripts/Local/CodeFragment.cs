using UnityEngine;
using TMPro;

public class CodeFragment : InteractionAbstract
{
    [Header("Fragment Info")]
    [SerializeField] private int position; // 0, 1, 2
    [SerializeField] private int digitValue; // 0-9

    [Header("Visual")]
    [SerializeField] private Color normalColor = Color.blue;

    [SerializeField]
    private Color[] pergaminColors = {
    new Color(0.96f, 0.87f, 0.70f), // Pozycja 0 - jasny pergamin
    new Color(0.93f, 0.84f, 0.65f), // Pozycja 1 - średni pergamin  
    new Color(0.90f, 0.80f, 0.60f)  // Pozycja 2 - ciemny pergamin
};
    [SerializeField] private Color textColor = Color.black; // Czarny tekst
    [SerializeField] private Color highlightColor = new Color(1f, 0.95f, 0.8f); // Jasny pergamin highlight


    private bool isCollected = false;



    // Components
    private TextMeshPro displayText;
    private Renderer backgroundRenderer;

    private void Start()
    {
        // Znajdź komponenty
        displayText = GetComponentInChildren<TextMeshPro>();
        backgroundRenderer = transform.Find("Background")?.GetComponent<Renderer>();

        // Ustaw początkowy wygląd
        UpdateDisplay();

        Debug.Log($"[CodeFragment] Fragment {position} initialized with digit: {digitValue}");
    }

    // Implementacja InteractionAbstract
    public override void Interact(PlayerScript player)
    {
        CollectFragment();
    }

    private void CollectFragment()
{
    if (isCollected) return;

    isCollected = true;

    Debug.Log($"[CodeFragment] Collected fragment {position} with digit: {digitValue}");

    // Powiadom CodeManager o zebraniu fragmentu
    if (CodeManager.Instance != null)
    {
        CodeManager.Instance.CollectFragment(position, digitValue);
    }
    else
    {
        Debug.LogWarning("[CodeFragment] CodeManager not found!");
    }

    // Ukryj fragment
    gameObject.SetActive(false);
}

    private void UpdateDisplay()
    {
        // Ustaw czarny tekst
        if (displayText != null)
        {
            displayText.text = digitValue.ToString();
            displayText.color = textColor;
        }

        // Ustaw kolor pergaminu bazując na pozycji
        if (backgroundRenderer != null)
        {
            Color pergaminColor = position >= 0 && position < pergaminColors.Length
                ? pergaminColors[position]
                : pergaminColors[0];

            backgroundRenderer.material.color = pergaminColor;
        }
    }

    // Setup fragment
    public void SetupFragment(int pos, int digit)
    {
        position = pos;
        digitValue = digit;
        UpdateDisplay();

        Debug.Log($"[CodeFragment] Setup fragment {position} with digit: {digitValue}");
    }

    // Gettery
    public int GetPosition() => position;
    public int GetDigitValue() => digitValue;
    public bool IsCollected() => isCollected;
}