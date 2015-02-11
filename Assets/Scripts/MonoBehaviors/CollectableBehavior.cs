using UnityEngine;
using System.Collections;

public class CollectableBehavior : MonoBehaviour
{
    public string itemName;
    public int quantity = 1;
    
    public bool useDefaultDestructionBehavior = true;
    
    private bool collected = false;
    
    //Events
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!collected)
        {
            //If this thing is the player, broadcast events.
            
            if (other.GetComponent<PlayerPlatformBehavior>() != null)
            {
                other.BroadcastMessage("OnCollectItem", new Item(itemName, quantity), SendMessageOptions.DontRequireReceiver);
                BroadcastMessage("OnCollected", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    
    void OnCollected()
    {
        if (useDefaultDestructionBehavior)
        {
            GameObject.Destroy(gameObject);
        }
    }
}

public struct Item
{
    public string itemName;
    public int quantity;
    
    public Item(string itemName, int quantity)
    {
        this.itemName = itemName;
        this.quantity = quantity;
    }
}