using UnityEngine;
using System.Collections;

public class KeyBehavior : MonoBehaviour
{
 
    public LockBehavior myLock;
    
    public float moveSpeed = 10;    //How fast the key moves to the lock when collected.
    
    private bool collected = false;
    
	//Events
    
    void Awake()
    {
        myLock.AddKey();
    }
    
    void OnDrawGizmos()
    {
        if (myLock != null)
        {
            Gizmos.DrawLine(transform.position, myLock.transform.position);
        }
    }
    
    void Update()
    {
        if (collected)
        {
            //Fly towards the lock
            transform.position = Vector3.MoveTowards(transform.position, myLock.transform.position, moveSpeed * Time.deltaTime);
            
            //Use the key when we reach
            if (transform.position == myLock.transform.position)
            {
                UseKey();
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        //If the player touched the key, start flying towards the lock
        
        if (!collected && other.GetComponent<PlayerPlatformBehavior>() != null)
        {
            collected = true;
        }
    }
    
    //Misc methods
    private void UseKey()
    {
        //Destroy the key and reduce the key count on the lock
        myLock.RemoveKey();
        GameObject.Destroy(gameObject);
    }
}
