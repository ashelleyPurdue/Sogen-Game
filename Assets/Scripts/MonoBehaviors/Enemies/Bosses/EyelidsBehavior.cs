using UnityEngine;
using System.Collections;

public class EyelidsBehavior : MonoBehaviour
{
    public Transform topEyelid;
    public Transform bottomEyelid;
 
    public float maxEyeAngle = 80f;
    
    public float maxVertDist = 0.5f;
    public float maxHorDist = -0.5f;
        
    public float Angle
    {
        set
        {
            //Rotate the eyelids
            topEyelid.localRotation = Quaternion.Euler(0, 0, -value);
            bottomEyelid.localRotation = Quaternion.Euler(0, 0, value);

            //Move the eyelids so they adjust
            float vertDist = Mathf.Lerp(0, -maxVertDist, value / maxEyeAngle);
            float horDist = Mathf.Lerp(0, maxHorDist, value / maxEyeAngle);
            
            Vector3 newPos = new Vector3(horDist, vertDist, -1);
            
            topEyelid.localPosition = newPos;
            
            newPos.y *= -1;
            bottomEyelid.localPosition = newPos;
            
        }
        
        get
        {
            return bottomEyelid.localEulerAngles.z;
        }
    }
    
    //Events
    
    void Awake()
    {
        //Move the eyelids to the correct place.
        //topEyelid.localPosition = new Vector3(0, 0, -1);
        //bottomEyelid.localPosition = new Vector3(0, 0, -1);
        
        //Flip the bottom eyelid
        bottomEyelid.localScale = new Vector3(1, -1, 1);
    }
    
    //Misc methods
}
