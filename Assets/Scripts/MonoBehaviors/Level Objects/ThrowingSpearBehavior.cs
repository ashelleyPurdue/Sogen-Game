using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ThrowableBehavior))]
[RequireComponent(typeof(DamageSource))]

public class ThrowingSpearBehavior : MonoBehaviour
{
    private Vector2 lastPos;
    
    public float lifetime = 10f;
    
    private float timer = 0f;
    
    private bool thrown = false;
    
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
        transform.rotation = Quaternion.Euler(0, 0, angle);
        lastPos = Utils.ToVector2(transform.position);
       
        //Disappear after a certain time.
        if (thrown)
        {
            timer += Time.deltaTime;
            
            if (timer >= lifetime)
            {
                GameObject.Destroy(this.gameObject);
            }
        }
    }
    
    void OnThrown()
    {
        thrown = true;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger)
        {
            GameObject.Destroy(this.gameObject, 0.1f);
        }
    }
}
