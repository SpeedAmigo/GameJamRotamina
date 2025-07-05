using UnityEngine;

public class cubeScript : InteractionAbstract
{
    public override void Interact(PlayerScript player)
    {
        Debug.Log(gameObject.name);
    }
}
