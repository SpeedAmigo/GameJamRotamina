using UnityEngine;

public class LightSanityRestorer : MonoBehaviour
{
    [SerializeField] private Light light;
    
    [SerializeField] private float capacity = 25;
    [SerializeField] private float amountToGive = 1f;
    
    private float sanityTimer = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (capacity <= 0f) return;
        
        if (other.TryGetComponent<PlayerScript>(out var player))
        {
            if (capacity > 0)
            {
                SanityManager.Instance.AddSanity(amountToGive);
                capacity -= amountToGive * Time.deltaTime;
            }
            else
            {
                light.intensity = 0f;
            }
        }   
    }
}
