using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircuitNode))]
public class ArrowShooterBehavior : MonoBehaviour
{
    
    public Transform arrowPoint;
 
    public SpriteRenderer eyeShine;
    
    public float waitTime = 3f;
    public float warningTime = 0.5f;
    
    public float arrowSpeed = 10f;
   
    public enum State {waiting, warning};
    private State currentState;
    
    private CircuitNode myNode;
    
    private ArrowBehavior myArrow = null;
    
    private float timer = 0f;
    
    //Events
    
    void Awake()
    {
        myNode = GetComponent<CircuitNode>();
    }
    
    void Update()
    {
        if (myNode.IsPowered())
        {
            WhilePowered();
        }
        
        //Show/hide the eyeshine
        eyeShine.enabled = myNode.IsPowered();
    }
    
    void OnPoweredChanged()
    {
        //When the power is turned on, immediately fire one shot.
        
        if (myNode.IsPowered())
        {
            CreateArrow();
            currentState = State.warning;
            timer = 0f;
        }
    }
    
    //Misc methods
    
	private void WhilePowered()
    {
	    if (currentState == State.waiting)
        {
            //Start warning after a bit
            
            myArrow = null;
            
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                //Move to the next state
                timer -= waitTime;
                currentState = State.warning;
                
                //Create the arrow
                CreateArrow();

            }
        }
        else if (currentState == State.warning)
        {
            //After warning for a bit, fire.
            
            timer += Time.deltaTime;
            if (timer >= warningTime)
            {
                //Fire the arrow
                Fire();
                
                //Move to the next state
                timer = 0f;
                currentState = State.waiting;
            }
        }
	}
    
    private void CreateArrow()
    {
        //Creates the arrow
        
        myArrow = ((GameObject)Instantiate(Resources.Load("arrow_prefab"))).GetComponent<ArrowBehavior>();
        
        myArrow.transform.position = arrowPoint.position;
        myArrow.transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z);
        myArrow.transform.parent = transform;
        
        myArrow.rigidbody2D.isKinematic = true;
        myArrow.GetComponent<DamageSource>().isHot = false;     //Make sure the arrow can't damage anything until it's fired.
        myArrow.collider2D.enabled = false;                     //Make sure the arrow won't touch the player and be destroyed until it's fired.
    }
    
    private void Fire()
    {
        //Fire an arrow.  Do not call unless the arrow has been created with CreateArrow() first.
        try
        {
            //Allow the arrow to move and damage things before firing.
            myArrow.rigidbody2D.isKinematic = false;
            myArrow.GetComponent<DamageSource>().isHot = true;
            myArrow.collider2D.enabled = true;
            
            //Unparent the arrow.
            myArrow.transform.parent = null;
            
            //Find the velocity and launch.
            float theta = transform.eulerAngles.z * Mathf.Deg2Rad;
            myArrow.rigidbody2D.velocity = arrowSpeed * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            
        }
        catch (MissingReferenceException e)
        {
            //Do nothing, since the arrow was destroyed before it could be fired.
        }
    }
}
