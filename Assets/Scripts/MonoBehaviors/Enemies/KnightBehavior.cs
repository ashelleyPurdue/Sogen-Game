using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlatformCharacterMotor))]
public class KnightBehavior : MonoBehaviour
{
    public enum State {sleeping, pausing, moving};
    
    public Transform shield;
    
    public float shieldMoveSpeed = 10f;
    
    //Private variables
    
    private State currentState = State.sleeping;
    private delegate void StateMethod();
    private Dictionary<State, StateMethod> stateMethods = new Dictionary<State, StateMethod>();
    
    private bool shieldUp = false;
    
    private Vector3 shieldUpPos = new Vector3(-0.518f, 0.562f, 0);
    private Vector3 shieldDownPos = new Vector3(-0.518f, -0.251f, 0);
    //Events
    
    void Awake()
    {
        //Set up the state methods.
    }
    
    void OnTakeDamage()
    {
        shieldUp = !shieldUp;
    }
    
    void Update()
    {
        //Finite state machine
        try
        {
            stateMethods[currentState]();
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogWarning(name + ": method for state not in dictionary.");
        }
        
        //Move the shield
        Vector3 targetPos = shieldUpPos;
        if (!shieldUp)
        {
            targetPos = shieldDownPos;
        }
        
        shield.localPosition = Vector3.MoveTowards(shield.localPosition, targetPos, shieldMoveSpeed * Time.deltaTime);
    }
    
    //States
    
    private void WhileSleeping()
    {
        //TODO: Go to pausing state when player is near.
    }
    
    private void WhilePausing()
    {
        //TODO: Pause for a while, then move.
    }
    
    private void WhileMoving()
    {
        //TODO: Move in a direction for a short time, then pause.
    }
    
}
