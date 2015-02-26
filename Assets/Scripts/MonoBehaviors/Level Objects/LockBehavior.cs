using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockBehavior : CircuitNodePowerSource
{
    private const float SHRINK_SPEED = 2f;
    private const float BASE_SCALE = 1f;
    private const float SCALE_PER_KEY = 0.5f;
    
    private int keysLeft = 0;
    private int maxKeys = 0;
    
    private float targetScale = 1f;
    
    //Events
    
    void Start()
    {
        maxKeys = keysLeft;
        UpdateStatus();
    }
    
    void Update()
    {
        float scale = transform.localScale.x;
        
        scale = Mathf.MoveTowards(scale, targetScale, SHRINK_SPEED * Time.deltaTime);
        
        transform.localScale = new Vector3(scale, scale, 0);
    }
    
    //Interface
    public void AddKey()
    {
        keysLeft++;
        UpdateStatus();
    }
    
    public void RemoveKey()
    {
        keysLeft--;
        UpdateStatus();
    }
    
    //Misc methods
    
    private void UpdateStatus()
    {
        //Update the size of the lock
        if (keysLeft > 0)
        {
            targetScale = BASE_SCALE + SCALE_PER_KEY * (keysLeft - 1);
        }
        else
        {
            targetScale = 0f;
        }
        
        //Update the circuitry
        isEnabled = keysLeft <= 0;
        UpdateCircuitry();
    }
}
