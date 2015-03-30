using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ThrowableBehavior))]
[RequireComponent(typeof(SpriteRenderer))]

public class BombBehavior : MonoBehaviour 
{
    public const float WARNING_TIME = 1.5f;
    public const float PULSE_FREQUENCY = 10;
    
    public float timeLeft = 1f;
    
    public float blastRadius = 2f;
    
    public ParticleSystem myParticles;
    
    public bool fuseLit = false;
    
    private SpriteRenderer sprRend;
    
    //Events
    
    void Awake()
    {
        sprRend = GetComponent<SpriteRenderer>();
    }
    
    void OnPickedUp()
    {
        fuseLit = true;
    }
    
    void Update()
    {   
        //Move to the correct z pos
        Vector3 pos = transform.position;
        pos.z = -2;
        transform.position = pos;
        
        //Count down while fuse is lit.
        if (fuseLit)
        {
            //Count down
            timeLeft -= Time.deltaTime;
            
            //Warn when time is running out.
            if (timeLeft <= WARNING_TIME)
            {
                sprRend.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(timeLeft * PULSE_FREQUENCY, 1f) );
            }
            
            //Explode when time is up
            if (timeLeft <= 0)
            {
                Explode();
            }
        }
        
        //Show/hide the fuse
        myParticles.enableEmission = fuseLit;
    }
    
    //Misc methods
    
    private void Explode()
    {
        //TODO: Create explosion
        GameObject.Destroy(gameObject);
        
        ExplosionBehavior.CreateExplosion(transform.position, blastRadius, 0.5f);
    }
}
