using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ThrowableBehavior))]
[RequireComponent(typeof(DamageSource))]

public class ThrowingSpearBehavior : MonoBehaviour
{
    private Vector2 lastPos;
    
    //Events
    
    void Awake()
    {
        lastPos = Utils.ToVector2(transform.position);
        
        //Set the collider to trigger
        collider2D.isTrigger = true;
    }
    
    void FixedUpdate()
    {
        //Update the angle.
        
        float angle = Mathf.Atan2(rigidbody2D.velocity.y, rigidbody2D.velocity.x) * Mathf.Rad2Deg;
        
        Debug.Log(rigidbody2D.velocity);
        
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        lastPos = Utils.ToVector2(transform.position);
    }
}
