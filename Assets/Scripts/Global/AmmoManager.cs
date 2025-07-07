using UnityEngine;
using System;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance { get; private set; }

    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmoInMagazine = 30;
    [SerializeField] private int startingTotalAmmo = 120;
    [SerializeField] private bool showAmmoInConsole = true;

    // Events
    public static event Action<int, int> OnAmmoChanged; // currentMagazine, totalAmmo
    public static event Action OnReloadStarted;
    public static event Action OnReloadCompleted;
    public static event Action OnOutOfAmmo;

    // Ammo tracking
    private int currentMagazineAmmo;
    private int totalAmmo;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeAmmo();
    }

    private void InitializeAmmo()
    {
        currentMagazineAmmo = maxAmmoInMagazine;
        totalAmmo = startingTotalAmmo;

        OnAmmoChanged?.Invoke(currentMagazineAmmo, totalAmmo);

        if (showAmmoInConsole)
        {
            Debug.Log($"[AmmoManager] Initialized - Magazine: {currentMagazineAmmo}/{maxAmmoInMagazine}, Total: {totalAmmo}");
        }
    }

    // Użyj nabój
    public bool UseAmmo()
    {
        if (currentMagazineAmmo <= 0)
        {
            if (showAmmoInConsole)
                Debug.Log("[AmmoManager] Cannot shoot - magazine empty!");
            return false;
        }

        currentMagazineAmmo--;
        OnAmmoChanged?.Invoke(currentMagazineAmmo, totalAmmo);

        if (showAmmoInConsole)
        {
            Debug.Log($"[AmmoManager] Shot fired - Magazine: {currentMagazineAmmo}/{maxAmmoInMagazine}, Total: {totalAmmo}");
        }

        return true;
    }

    // Przeładuj
    public bool Reload()
    {
        if (currentMagazineAmmo >= maxAmmoInMagazine)
        {
            if (showAmmoInConsole)
                Debug.Log("[AmmoManager] Cannot reload - magazine full!");
            return false;
        }

        if (totalAmmo <= 0)
        {
            if (showAmmoInConsole)
                Debug.Log("[AmmoManager] Cannot reload - no ammo left!");
            OnOutOfAmmo?.Invoke();
            return false;
        }

        OnReloadStarted?.Invoke();

        // Oblicz ile naboi potrzeba
        int ammoNeeded = maxAmmoInMagazine - currentMagazineAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, totalAmmo);

        // Przeładuj
        currentMagazineAmmo += ammoToReload;
        totalAmmo -= ammoToReload;

        OnAmmoChanged?.Invoke(currentMagazineAmmo, totalAmmo);
        OnReloadCompleted?.Invoke();

        if (showAmmoInConsole)
        {
            Debug.Log($"[AmmoManager] Reloaded {ammoToReload} rounds - Magazine: {currentMagazineAmmo}/{maxAmmoInMagazine}, Total: {totalAmmo}");
        }

        return true;
    }

    // Dodaj naboje
    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
        OnAmmoChanged?.Invoke(currentMagazineAmmo, totalAmmo);

        if (showAmmoInConsole)
        {
            Debug.Log($"[AmmoManager] Added {amount} ammo - Total: {totalAmmo}");
        }
    }

    // Gettery
    public int GetCurrentMagazineAmmo() => currentMagazineAmmo;
    public int GetTotalAmmo() => totalAmmo;
    public int GetMaxMagazineAmmo() => maxAmmoInMagazine;
    public bool IsMagazineEmpty() => currentMagazineAmmo <= 0;
    public bool IsOutOfAmmo() => currentMagazineAmmo <= 0 && totalAmmo <= 0;

    // Debug controls
    private void Update()
    {
        // Debug controls
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddAmmo(30);
            Debug.Log("[AmmoManager] DEBUG: Added 30 ammo");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Pokaż aktualny stan
            Debug.Log($"[AmmoManager] CURRENT STATUS: Magazine {currentMagazineAmmo}/{maxAmmoInMagazine}, Total: {totalAmmo}");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // Wymuś reload
            if (Reload())
            {
                Debug.Log("[AmmoManager] DEBUG: Manual reload successful");
            }
            else
            {
                Debug.Log("[AmmoManager] DEBUG: Manual reload failed");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // Reset amunicji
            InitializeAmmo();
            Debug.Log("[AmmoManager] DEBUG: Ammo reset to defaults");
        }
    }
}