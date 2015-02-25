using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ThrowableBehavior))]
[RequireComponent(typeof(DamageSource))]
public class BaseballBehavior : MonoBehaviour
{
 
    private DamageSource myDamageSource;
    private ThrowableBehavior throwable;
    
    private Transform lastCarrier = null;
    private HealthPoints lastCarrierHP = null;
    
    //Events
    
    void Awake()
    {
        myDamageSource = GetComponent<DamageSource>();
        myDamageSource.isHot = false;
        
        throwable = GetComponent<ThrowableBehavior>();
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
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        //Deactivate the damage source
        
        if (lastCarrier != null && other.collider != lastCarrier.collider2D)
        {
            myDamageSource.isHot = false;
            Debug.Log("Collision enter.");
        }
    }
}
