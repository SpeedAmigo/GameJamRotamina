using UnityEngine;

public class GunScript : InteractionAbstract
{
    public void Shoot()
    {
        Debug.Log("Shooting");
    }
    
    public override void Interact(PlayerScript player)
    {
        transform.position = player.gunSocket.position;
        transform.rotation = player.gunSocket.rotation;
        player.currentGun = this;
        
        transform.SetParent(player.gunSocket);
    }
}
