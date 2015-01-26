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
    
    void OnCollisionEnter2D(Collision2D other)
    {
        //TODO: Stick into the surface and fade out.
        
        Destroy(this.gameObject);
    }
}
