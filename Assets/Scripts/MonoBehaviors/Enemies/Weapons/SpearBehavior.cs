using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(DamageSource))]
public class SpearBehavior : MonoBehaviour
{
    
    public float attackLength = 1f;     //How far the spear lurches when it attacks.
    
    public float windupTime = 0.5f;
    public float pauseTime = 0.25f;
    public float thrustTime = 0.25f;
    public float hangTime = 0.5f;
    public float returnTime = 0.5f;
    
    public enum State {ready, windingUp, pausing, thrusting, hanging, returning};
    private State currentState = State.ready;
    
    private delegate void StateMethod();
    private Dictionary<State, StateMethod> stateMethods = new Dictionary<State, StateMethod>();
    
    private float timer = 0f;
    
    private Transformation restingPos = new Transformation(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, -90));
    
    private Transformation windUpPos;
    private Transformation attackPos;
    
    private DamageSource mySource;
    
    //Events
    
    void Awake()
    {
        //Set up the state methods
        stateMethods.Add(State.ready, WhileReady);
        stateMethods.Add(State.windingUp, WhileWindingUp);
        stateMethods.Add(State.pausing, WhilePausing);
        stateMethods.Add(State.thrusting, WhileThrusting);
        stateMethods.Add(State.hanging, WhileHanging);
        stateMethods.Add(State.returning, WhileReturning);
        
        //Get the damage source
        mySource = GetComponent<DamageSource>();
    }
    
    void Update()
    {
        //FInite state machine
        if (stateMethods.ContainsKey(currentState))
        {
            stateMethods[currentState]();
        }
        
        //Only enable the damage source when thrusting
        mySource.isHot = (currentState == State.thrusting);
    }
    
    //Misc methods
    
    public void Attack(float height)
    {
        //Starts attacking, if ready.
        
        if (currentState == State.ready)
        {
            currentState = State.windingUp;
            timer = 0f;
            
            CalculatePoints(height);
        }
    }
    
    private void CalculatePoints(float height)
    {
        //Calculates the points of the animation.
        
        windUpPos = new Transformation(restingPos.position, Quaternion.Euler(0, 0, 0));
        windUpPos.position.y = height;
        windUpPos.position.x -= attackLength * transform.localScale.x;
        
        attackPos = windUpPos;
        attackPos.position.x += 2 * attackLength * transform.localScale.x;
    }
    
    private void StateStuff(Transformation startPos, Transformation endPos, float duration, State nextState)
    {
        timer += Time.deltaTime;
        
        Transformation.Lerp(startPos, endPos, timer / duration, true).ApplyTo(transform, true);
        
        if (timer >= duration)
        {
            currentState = nextState;
            timer -= duration;
        }
    }
    
    //States
    
    private void WhileReady()
    {
        restingPos.ApplyTo(transform, true);
    }
    
    private void WhileWindingUp()
    {
        //Wind up
        StateStuff(restingPos, windUpPos, windupTime, State.pausing);
    }
    
    private void WhilePausing()
    {
        //Pause
        StateStuff(windUpPos, windUpPos, pauseTime, State.thrusting);
    }
    
    private void WhileThrusting()
    {
        //Thrust
        StateStuff(windUpPos, attackPos, thrustTime, State.hanging);
    }
    
    private void WhileHanging()
    {
        //Hang
        StateStuff(attackPos, attackPos, hangTime, State.returning);
    }
    
    private void WhileReturning()
    {
        //Return
        StateStuff(attackPos, restingPos, returnTime, State.ready);
    }
}
