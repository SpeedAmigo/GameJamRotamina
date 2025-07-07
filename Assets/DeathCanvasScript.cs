using UnityEngine;

public class DeathCanvasScript : MonoBehaviour
{
    [SerializeField] private GameObject deathCanvas;

    private void Start()
    {
        deathCanvas.SetActive(false);
    }
    
    private void OnEnable()
    {
        SanityManager.OnDeath += ActiveDeathCanvas;
    }

    private void OnDisable()
    {
        SanityManager.OnDeath -= ActiveDeathCanvas;
    }

    private void ActiveDeathCanvas(bool isActive)
    {
        deathCanvas.SetActive(isActive);
    }
}
