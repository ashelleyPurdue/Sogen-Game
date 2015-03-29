using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class ThrowableBehavior : MonoBehaviour
{
    public float pickupSpeed = 10;
    public float throwColliderDelay = 0.25f;
    
    public Transform Carrier { get{ return transform.parent; } }
    
    public enum State {free, pickingUp, carrying, justThrown};
    private State currentState = State.free;
    
    private float timer = 0f;
    
    private delegate void StateMethod();
    private Dictionary<State, StateMethod> stateMethods = new Dictionary<State, StateMethod>();
    
    private Vector2 carrierCoordinates = Vector2.zero;  //Where in relation to the carrier should this object be held?
    
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
    
    public void EnablePhysics(bool enablePhys)
    {
        //Enables/disables physics for this object.
        
        rigidbody2D.isKinematic = !enablePhys;
        collider2D.enabled = enablePhys;
    }
    
    public void PickUp(Transform carrier, Vector3 carryCoords)
    {
        //Picks this object up.
        currentState = State.pickingUp;
        transform.parent = carrier;
        carrierCoordinates = new Vector2(carryCoords.x, carryCoords.y);
        
        //Send an event
        transform.BroadcastMessage("OnPickedUp", SendMessageOptions.DontRequireReceiver);
    }
    
    public void Throw(Vector2 velocity)
    {
        //Throws this object with a velocity.
        
        if (currentState == State.carrying)
        {
            currentState = State.justThrown;
            transform.parent = null;
            timer = 0f;
            
            //Calculate out current velocity
            Vector3 calcedVelocity = transform.position - lastPos;
            
            //Set the velocity
            rigidbody2D.velocity = new Vector2(calcedVelocity.x, calcedVelocity.y) + velocity;
            
            //Send an event
            transform.BroadcastMessage("OnThrown", SendMessageOptions.DontRequireReceiver);
        }
    }

    //State methods
    
    private void WhileFree()
    {
        //Enable physics
        EnablePhysics(true);
    }
    
    private void WhilePickingUp()
    {
        //Disable physics
        EnablePhysics(false);
        
        //Move to the carry pos.
        Vector2 pos2D = new Vector2(transform.localPosition.x, transform.localPosition.y);
        pos2D = Vector2.MoveTowards(pos2D, carrierCoordinates, pickupSpeed * Time.deltaTime);
        
        transform.localPosition = new Vector3(pos2D.x, pos2D.y, transform.localPosition.z);
        
        //Go to carrying
        if (pos2D == carrierCoordinates)
        {
            currentState = State.carrying;
        }
    }
    
    private void WhileCarrying()
    {
        EnablePhysics(false);
    }
    
    private void WhileJustThrown()
    {
        //Enable physics, but not the collider
        EnablePhysics(true);
        collider2D.enabled = false;
        
        timer += Time.deltaTime;
        
        if (timer >= throwColliderDelay)
        {
            currentState = State.free;
        }
    }
}
