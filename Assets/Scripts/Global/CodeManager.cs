using UnityEngine;
using System.Collections.Generic;

public class CodeManager : MonoBehaviour
{
    public static CodeManager Instance { get; private set; }

    [Header("Code System")]
    [SerializeField] private bool generateCodeOnStart = true;
    [SerializeField] private bool showCodeInConsole = true;

    // 3-cyfrowy kod
    private string generatedCode;
    private List<int> codeDigits = new List<int>();

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
            int digit = Random.Range(0, 10); // 0-9
            codeDigits.Add(digit);
            generatedCode += digit.ToString();
        }

        if (showCodeInConsole)
        {
            Debug.Log($"[CodeManager] Generated 3-digit code: {generatedCode}");
            Debug.Log($"[CodeManager] Code digits: [{string.Join(", ", codeDigits)}]");
        }
    }

    // Gettery dla kodu
    public string GetCode() => generatedCode;
    public List<int> GetCodeDigits() => new List<int>(codeDigits);
    public int GetDigitAtPosition(int position)
    {
        if (position >= 0 && position < codeDigits.Count)
            return codeDigits[position];
        return -1;
    }

    // Debug - wygeneruj nowy kod
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateCode();
            Debug.Log("[CodeManager] New code generated!");
        }
    }
}