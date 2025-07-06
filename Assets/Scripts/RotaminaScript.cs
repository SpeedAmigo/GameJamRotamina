using UnityEngine;

public class RotaminaScript : InteractionAbstract
{
    [SerializeField] [Range (0, 1)] private float restorePercentage = 0.3f;
    
    public override void Interact(PlayerScript player)
    {
        SanityManager manager = SanityManager.Instance;

        var maxSanity= manager.GetMaxSanity();
        var valueToAdd = maxSanity * restorePercentage;
        
        manager.AddSanity(valueToAdd);
        
        Destroy(gameObject);
    }
}
