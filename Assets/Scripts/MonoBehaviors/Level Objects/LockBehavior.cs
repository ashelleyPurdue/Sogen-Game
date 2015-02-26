using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockBehavior : CircuitNodePowerSource
{
    private int keysLeft = 0;
    
    //Interface
    public void AddKey()
    {
        keysLeft++;
        UpdatePower();
    }
    
    public void RemoveKey()
    {
        keysLeft--;
        UpdatePower();
    }
    
    //Misc methods
    
    private void UpdatePower()
    {
        isEnabled = keysLeft <= 0;
        UpdateCircuitry();
    }
}
