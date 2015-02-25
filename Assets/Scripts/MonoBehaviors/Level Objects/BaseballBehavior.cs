using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ThrowableBehavior))]
[RequireComponent(typeof(DamageSource))]
public class BaseballBehavior : MonoBehaviour
{
 
    public List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    
    public float minimumDamageSpeed = 3f;
    
    public float stationaryDamageDeactivateTime = 0.1f;
    
    private DamageSource myDamageSource;
    private ThrowableBehavior throwable;
    
    private float timer = 0f;
    
    private Transform lastCarrier = null;
    private HealthPoints lastCarrierHP = null;
    
    //Events
    
    void Awake()
    {
        myDamageSource = GetComponent<DamageSource>();
        myDamageSource.isHot = false;
        
        throwable = GetComponent<ThrowableBehavior>();
    }
    
    void Update()
    {
        //Activate/deactivate the fire trail
        foreach (ParticleSystem s in particleSystems)
        {
            s.enableEmission = myDamageSource.isHot;
        }
    }
    
    void FixedUpdate()
    {
        //Update the damage source
        if (rigidbody2D.velocity.sqrMagnitude >= minimumDamageSpeed * minimumDamageSpeed)
        {
            myDamageSource.isHot = true;
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
            
            if (timer >= stationaryDamageDeactivateTime)
            {
                myDamageSource.isHot = false;
            }
        }
    }
    
    void OnPickedUp()
    {
        //Make sure not to damage the carrier.
        HealthPoints carrierHP = throwable.Carrier.GetComponent<HealthPoints>();
       
        if (carrierHP != null && !myDamageSource.objectsToIgnore.Contains(carrierHP))
        {
            //Start hurting the last carrier again.
            if (lastCarrierHP != null && myDamageSource.objectsToIgnore.Contains(lastCarrierHP))
            {
                myDamageSource.objectsToIgnore.Remove(lastCarrierHP);
            }
            
            //Start hurting the new carrier
            myDamageSource.objectsToIgnore.Add(carrierHP);
            lastCarrierHP = carrierHP;
        }
        
        //Remember the last carrier
        lastCarrier = throwable.Carrier;
    }
    
    void OnThrown()
    {
        //Enable the damage source
        myDamageSource.isHot = true;
        
        //Reset the rotation
        transform.rotation = Quaternion.identity;
    }
}
