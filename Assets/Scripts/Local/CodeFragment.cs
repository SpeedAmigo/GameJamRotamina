using UnityEngine;
using TMPro;

public class CodeFragment : InteractionAbstract
{
    [Header("Fragment Info")]
    [SerializeField] private int position; // 0, 1, 2
    [SerializeField] private int digitValue; // 0-9

    [Header("Visual")]
    [SerializeField] private Color normalColor = Color.blue;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Color textColor = Color.white;

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

        // TODO: Powiadom UI Manager o zebraniu fragmentu
        // if (CodeUIManager.Instance != null)
        //     CodeUIManager.Instance.CollectFragment(position, digitValue);

        // Ukryj fragment
        gameObject.SetActive(false);
    }

    private void UpdateDisplay()
    {
        // Ustaw tekst
        if (displayText != null)
        {
            displayText.text = digitValue.ToString();
            displayText.color = textColor;
        }

        // Ustaw kolor tła
        if (backgroundRenderer != null)
        {
            backgroundRenderer.material.color = normalColor;
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