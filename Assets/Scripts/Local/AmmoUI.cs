using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI magazineAmmoText; // Ile pociskow w aktualnym magazynku
    [SerializeField] private TextMeshProUGUI reserveAmmoText; // Ile pozostalo w rezerwie (bez magazynka)

    private void OnEnable()
    {
        // Subskrybuj eventy z AmmoManager
        AmmoManager.OnAmmoChanged += UpdateAmmoDisplay;
    }

    private void OnDisable()
    {
        // Odsubskrybuj eventy
        AmmoManager.OnAmmoChanged -= UpdateAmmoDisplay;
    }

    private void Start()
    {
        // Pokaż początkowe wartości
        if (AmmoManager.Instance != null)
        {
            UpdateAmmoDisplay(AmmoManager.Instance.GetCurrentMagazineAmmo(), AmmoManager.Instance.GetTotalAmmo());
        }
        else
        {
            UpdateAmmoDisplay(0, 0);
        }
    }

    // Zaktualizuj wyświetlanie amunicji
    private void UpdateAmmoDisplay(int magazineAmmo, int totalAmmo)
    {
        // Główny tekst - ile w magazynku
        if (magazineAmmoText != null)
        {
            magazineAmmoText.text = magazineAmmo.ToString();
        }

        // Drugi tekst - ile w rezerwie (totalAmmo to już jest bez magazynka)
        if (reserveAmmoText != null)
        {
            reserveAmmoText.text = totalAmmo.ToString();
        }

        Debug.Log($"[AmmoUI] Updated display - Magazine: {magazineAmmo}, Reserve: {totalAmmo}");
    }
}