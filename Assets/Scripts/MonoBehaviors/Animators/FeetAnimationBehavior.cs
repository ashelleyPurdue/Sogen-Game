using UnityEngine;
using System.Collections;

public class FeetAnimationBehavior : MonoBehaviour
{
    public Transform leftFoot;
    public Transform rightFoot;
    
    public float stepSize = 1f;
    
    public float footLevel = -1f;   //How far down the feet are.
    public float horOffset = 0.25f;
    
    public PlatformCharacterMotor motorToWatch;     //The feet will animate in accordance with the motion of this motor.
    
    
    private Vector3 leftTargPos;
    private Vector3 rightTargPos;
    
    private float timer = 0f;
    
    //Events
    
    void Awake()
    {
        //Start the target positions.
        leftTargPos = new Vector3(horOffset, footLevel, 0);
        rightTargPos = new Vector3(horOffset, footLevel, 0);
    }
    
    void Update()
    {   
        //Move the feet to their target positions.
        leftFoot.localPosition = Vector3.MoveTowards(leftFoot.localPosition, leftTargPos, 10 * Time.deltaTime);
        rightFoot.localPosition = Vector3.MoveTowards(rightFoot.localPosition, rightTargPos, 10 * Time.deltaTime);
        
        //Move the feet
        timer += motorToWatch.rigidbody2D.velocity.x * Time.deltaTime / stepSize;

        leftTargPos.x = horOffset + Mathf.Sin(timer) * stepSize;
        rightTargPos.x = horOffset -1 * Mathf.Sin(timer + (Mathf.PI/8)) * stepSize;
        

    }
}
