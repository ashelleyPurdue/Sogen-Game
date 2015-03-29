using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ThrowableBehavior))]

public class BombBehavior : MonoBehaviour 
{
    public float timeLeft = 1f;
    
    public ParticleSystem myParticles;
    
    public bool fuseLit = false;
    
    //Events
    
    void OnPickedUp()
    {
        fuseLit = true;
    }
    
    void Update()
    {            
        if (fuseLit)
        {
            //Count down
            timeLeft -= Time.deltaTime;
            
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
    }
}
