using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodeUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI[] digitTexts; // 3 teksty na cyfry
    [SerializeField] private Image[] digitBackgrounds; // 3 tła dla cyfr

    [Header("Colors")]
    [SerializeField] private Color unknownColor = Color.gray; // Kolor dla "?"
    [SerializeField]
    private Color[] positionColors = {
        new Color(0.96f, 0.87f, 0.70f), // Pozycja 0 - jasny pergamin
        new Color(0.93f, 0.84f, 0.65f), // Pozycja 1 - średni pergamin  
        new Color(0.90f, 0.80f, 0.60f)  // Pozycja 2 - ciemny pergamin
    };
    [SerializeField] private Color textColor = Color.black;

    private void OnEnable()
    {
        // Subskrybuj eventy
        CodeManager.OnFragmentCollected += OnFragmentCollected;
        CodeManager.OnCodeReset += OnCodeReset;
    }

    private void OnDisable()
    {
        // Odsubskrybuj eventy
        CodeManager.OnFragmentCollected -= OnFragmentCollected;
        CodeManager.OnCodeReset -= OnCodeReset;
    }

    private void Start()
    {
        // Inicjalizuj UI
        ResetAllSlots();
    }

    // Event handler - fragment zebrany
    private void OnFragmentCollected(int position, int digit)
    {
        UpdateSlot(position, digit, true);
        Debug.Log($"[CodeUI] Updated slot {position} with digit {digit}");
    }

    // Event handler - kod zresetowany
    private void OnCodeReset()
    {
        ResetAllSlots();
        Debug.Log("[CodeUI] UI reset - all slots back to unknown");
    }

    private void ResetAllSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            UpdateSlot(i, 0, false); // false = nie zebrane
        }
    }

    private void UpdateSlot(int position, int digit, bool isCollected)
    {
        if (position >= digitTexts.Length || position >= digitBackgrounds.Length) return;

        // Aktualizuj tekst
        if (digitTexts[position] != null)
        {
            if (isCollected)
            {
                digitTexts[position].text = digit.ToString(); // Pokaż cyfrę
            }
            else
            {
                digitTexts[position].text = "?"; // Nieznane
            }
            digitTexts[position].color = textColor;
        }

        // Aktualizuj tło
        if (digitBackgrounds[position] != null)
        {
            if (isCollected)
            {
                digitBackgrounds[position].color = positionColors[position]; // Pergamin
            }
            else
            {
                digitBackgrounds[position].color = unknownColor; // Szary
            }
        }
    }
}