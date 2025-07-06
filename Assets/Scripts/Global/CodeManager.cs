using UnityEngine;
using System.Collections.Generic;
using System;

public class CodeManager : MonoBehaviour
{
    public static CodeManager Instance { get; private set; }

    [Header("Code System")]
    [SerializeField] private bool generateCodeOnStart = true;
    [SerializeField] private bool showCodeInConsole = true;

    // Events
    public static event Action<int, int> OnFragmentCollected; // position, digit
    public static event Action OnCodeReset;

    // 3-cyfrowy kod
    private string generatedCode;
    private List<int> codeDigits = new List<int>();

    // Tracking zebranych fragmentów
    private int?[] collectedDigits = new int?[3]; // null = nie zebrane, int = zebrana cyfra

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
        if (generateCodeOnStart)
        {
            GenerateCode();
        }
    }

    // Generuj 3-cyfrowy kod
    public void GenerateCode()
    {
        codeDigits.Clear();
        generatedCode = "";

        // Wygeneruj 3 losowe cyfry
        for (int i = 0; i < 3; i++)
        {
            int digit = UnityEngine.Random.Range(0, 10); // 0-9
            codeDigits.Add(digit);
            generatedCode += digit.ToString();
        }

        if (showCodeInConsole)
        {
            Debug.Log($"[CodeManager] Generated 3-digit code: {generatedCode}");
            Debug.Log($"[CodeManager] Code digits: [{string.Join(", ", codeDigits)}]");
        }

        // Reset zebranych fragmentów
        InitializeCollectedDigits();
    }

    private void InitializeCollectedDigits()
    {
        for (int i = 0; i < 3; i++)
        {
            collectedDigits[i] = null; // Nie zebrane
        }

        // Wywołaj event reset
        OnCodeReset?.Invoke();

        if (showCodeInConsole)
            Debug.Log("[CodeManager] Collected digits reset: [?, ?, ?]");
    }

    // Zbierz fragment
    public void CollectFragment(int position, int digit)
    {
        if (position < 0 || position >= 3)
        {
            Debug.LogError($"[CodeManager] Invalid position: {position}");
            return;
        }

        if (collectedDigits[position] != null)
        {
            Debug.LogWarning($"[CodeManager] Fragment at position {position} already collected!");
            return;
        }

        // Zapisz zebrana cyfre
        collectedDigits[position] = digit;

        // Wywołaj event
        OnFragmentCollected?.Invoke(position, digit);

        if (showCodeInConsole)
        {
            string status = GetCollectedDigitsStatus();
            Debug.Log($"[CodeManager] Fragment {position} collected with digit {digit}");
            Debug.Log($"[CodeManager] Current status: {status}");
        }
    }

    // Publiczna metoda do sprawdzania kodu dla innych systemów
    public bool ValidateCode(string inputCode)
    {
        bool isCorrect = inputCode == generatedCode;

        if (showCodeInConsole)
        {
            Debug.Log($"[CodeManager] Code validation: '{inputCode}' vs '{generatedCode}' = {(isCorrect ? "CORRECT" : "INCORRECT")}");
        }

        return isCorrect;
    }

    // Gettery dla kodu
    public string GetCode() => generatedCode;
    public string GetGeneratedCode() => generatedCode;
    public List<int> GetCodeDigits() => new List<int>(codeDigits);
    public int GetDigitAtPosition(int position)
    {
        if (position >= 0 && position < codeDigits.Count)
            return codeDigits[position];
        return -1;
    }

    // Gettery dla zebranych fragmentów
    public int? GetCollectedDigitAtPosition(int position)
    {
        if (position >= 0 && position < collectedDigits.Length)
            return collectedDigits[position];
        return null;
    }

    public bool IsFragmentCollected(int position)
    {
        if (position >= 0 && position < collectedDigits.Length)
            return collectedDigits[position] != null;
        return false;
    }

    public string GetCollectedCode()
    {
        string code = "";
        for (int i = 0; i < 3; i++)
        {
            code += collectedDigits[i]?.ToString() ?? "?";
        }
        return code;
    }

    public string GetCollectedDigitsStatus()
    {
        return $"[{(collectedDigits[0]?.ToString() ?? "?")}, {(collectedDigits[1]?.ToString() ?? "?")}, {(collectedDigits[2]?.ToString() ?? "?")}]";
    }

    // Debug - wygeneruj nowy kod
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateCode();
            Debug.Log("[CodeManager] New code generated!");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            string status = GetCollectedDigitsStatus();
            Debug.Log($"[CodeManager] Current collected status: {status}");
        }
    }
}