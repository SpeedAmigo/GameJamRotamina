using UnityEngine;

public class cubeScript : InteractionAbstract
{
    public override void Interact()
    {
        Debug.Log(gameObject.name);
    }
}
