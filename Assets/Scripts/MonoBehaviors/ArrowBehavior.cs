using UnityEngine;
using System.Collections;

public class ArrowBehavior : MonoBehaviour
{
    public float lifetime = 10f;    //How long the arrow "lives" for before it is destroyed.
    
    //Events
    
    void Update()
    {
        lifetime -= Time.deltaTime;
        
        if (lifetime <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    
    void FixedUpdate()
    {
    }
    
    void OnBlocked()
    {
        GetComponent<DamageSource>().isHot = false;
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        //TODO: Stick into the surface and fade out.
        
        transform.parent = other.transform;
        rigidbody2D.velocity = Vector2.zero;
        
        Destroy(this.gameObject, 0.05f);
    }
}
