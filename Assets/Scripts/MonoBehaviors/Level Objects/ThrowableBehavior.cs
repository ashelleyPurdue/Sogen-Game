using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class ThrowableBehavior : MonoBehaviour
{
    public float pickupSpeed = 10;
    public float throwColliderDelay = 0.25f;
    
    public enum State {free, pickingUp, carrying, justThrown};
    private State currentState = State.free;
    
    private float timer = 0f;
    
    private delegate void StateMethod();
    private Dictionary<State, StateMethod> stateMethods = new Dictionary<State, StateMethod>();
    
    private Vector3 carrierCoordinates = Vector3.zero;  //Where in relation to the carrier should this object be held?
    
    private Vector3 lastPos = Vector3.zero;
    
    //Events
	void Awake ()
    {
	    lastPos = transform.position;
        
        //Construct the state methods
        stateMethods.Add(State.free, WhileFree);
        stateMethods.Add(State.pickingUp, WhilePickingUp);
        stateMethods.Add(State.carrying, WhileCarrying);
        stateMethods.Add(State.justThrown, WhileJustThrown);
	}
	
	void Update ()
    {
	    stateMethods[currentState]();
	}
    
    void FixedUpdate()
    {
        lastPos = transform.position;
    }
    
    //Interface
    
    public void PickUp(Transform carrier, Vector3 carryCoords)
    {
        //Picks this object up.
        currentState = State.pickingUp;
        transform.parent = carrier;
        carrierCoordinates = carryCoords;
    }
    
    public void Throw(Vector2 velocity)
    {
        //Throws this object with a velocity.
        
        if (currentState == State.carrying)
        {
            currentState = State.justThrown;
            transform.parent = null;
            
            //Calculate out current velocity
            Vector3 calcedVelocity = transform.position - lastPos;
            
            //Set the velocity
            rigidbody2D.velocity = new Vector2(calcedVelocity.x, calcedVelocity.y) + velocity;
        }
    }

    //State methods
    
    private void WhileFree()
    {
        //Enable physics
        rigidbody2D.isKinematic = false;
        collider2D.enabled = true;
    }
    
    private void WhilePickingUp()
    {
        //Disable physics
        rigidbody2D.isKinematic = true;
        collider2D.enabled = false;
        
        //Move to the carry pos.
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, carrierCoordinates, pickupSpeed * Time.deltaTime);
        
        //Go to carrying
        if (transform.localPosition == carrierCoordinates)
        {
            currentState = State.carrying;
        }
    }
    
    private void WhileCarrying()
    {
        //Disable physics
        rigidbody2D.isKinematic = true;
        collider2D.enabled = false;
    }
    
    private void WhileJustThrown()
    {
        //Enable physics, but not the collider
        rigidbody2D.isKinematic = false;
        collider2D.enabled = false;
        
        timer += Time.deltaTime;
        
        if (timer >= throwColliderDelay)
        {
            currentState = State.free;
        }
    }
}
