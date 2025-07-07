using UnityEngine;

public class AmmunitionPickup : InteractionAbstract
{

    [Header("Pickup Settings")]
    [SerializeField] private int ammoAmount = 5; // Amount of ammo to add

    public override void Interact(PlayerScript player)
    {
        Debug.Log($"[AmmunitionPickup] Attempting to pick up {ammoAmount} ammo.");

        // Sprawdź czy AmmoManager istnieje
        if (AmmoManager.Instance == null)
        {
            Debug.LogError("[AmmunitionPickup] AmmoManager not found!");
            return;
        }

        // Dodaj amunicję
        AmmoManager.Instance.AddAmmo(ammoAmount);
        Debug.Log($"[AmmunitionPickup] Successfully picked up {ammoAmount} ammo!");

        // Ukryj/zniszcz obiekt po pobraniu
        gameObject.SetActive(false);
    }

    // Metoda do ustawienia ilości amunicji przez spawner
    public void SetAmmoAmount(int amount)
    {
        ammoAmount = amount;
        Debug.Log($"[AmmunitionPickup] Ammo amount set to: {ammoAmount}");
    }

    // Getter
    public int GetAmmoAmount() => ammoAmount;
}
