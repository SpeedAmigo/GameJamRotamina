using UnityEngine;
using UnityEngine.Events;

public class LiftButtonScript : InteractionAbstract
{
    public UnityEvent onLift;
    
    public override void Interact(PlayerScript player)
    {
        onLift?.Invoke();
    }
}
