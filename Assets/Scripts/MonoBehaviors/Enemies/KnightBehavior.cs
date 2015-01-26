using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlatformCharacterMotor))]
public class KnightBehavior : MonoBehaviour
{
    public enum State {sleeping, pausing, moving};
    
    public Transform shield;
    
    public float shieldMoveSpeed = 10f;
    
    public float visionRadius = 5f;
    
    public float minPauseTime = 0.25f;
    public float maxPauseTime = 1f;
    
    public float minMoveTime = 0.25f;
    public float maxMoveTime = 0.5f;
    
    public float maxWanderDistance = 3f;    //How far away from the starting point the knight is allowed to stray.
    
    //Private variables
    
    private State currentState = State.sleeping;
    private delegate void StateMethod();
    private Dictionary<State, StateMethod> stateMethods = new Dictionary<State, StateMethod>();
    
    private bool shieldUp = false;
    
    private Vector3 shieldUpPos = new Vector3(-0.518f, 0.562f, 0);
    private Vector3 shieldDownPos = new Vector3(-0.518f, -0.251f, 0);
    
    private Vector3 startPos;
    
    private PlatformCharacterMotor myMotor;
    
    private float timer = 0f;
    
    //Events
    
    void Awake()
    {
        myMotor = GetComponent<PlatformCharacterMotor>();
        
        startPos = transform.position;
        
        //Set up the state methods.
        stateMethods.Add(State.sleeping, WhileSleeping);
        stateMethods.Add(State.pausing, WhilePausing);
        stateMethods.Add(State.moving, WhileMoving);
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
        
        //Don't move
        myMotor.ControllerInput = Vector2.zero;
        
        //If player is near, start pausing
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, visionRadius);
        
        bool foundPlayer = false;
        foreach (Collider2D h in hits)
        {
            if (h.GetComponent<PlayerPlatformBehavior>() != null)
            {
                foundPlayer = true;
                break;
            }
        }
        
        if (foundPlayer)
        {
            //Pause for a random amount of time.
            currentState = State.pausing;
            timer = Random.Range(minPauseTime, maxPauseTime);   //Timer counts down in this state.
        }
    }
    
    private void WhilePausing()
    {
        //Pause for a while, then move.
        
        myMotor.ControllerInput = Vector2.zero;
        
        timer -= Time.deltaTime;
        
        if (timer <= 0f)
        {
            //Start moving.
            timer = Random.Range(minMoveTime, maxMoveTime);
            currentState = State.moving;
            
            if (Random.Range(0, 1) == 0)
            {
                myMotor.ControllerInput = new Vector2(-1, 0);
            }
            else
            {
                myMotor.ControllerInput = new Vector2(1, 0);
            }
        }
    }
    
    private void WhileMoving()
    {
        //TODO: Move in a direction for a short time, then pause.
        
        //If we're going too far away from home, change directions.
        float projectedX = transform.position.x + (Mathf.Sign(myMotor.ControllerInput.x) * Mathf.Abs(rigidbody2D.velocity.x)) * Time.deltaTime;
        
        if (Mathf.Abs(startPos.x - projectedX) > maxWanderDistance)
        {
            Vector3 newPos = transform.position;
            newPos.x = startPos.x + maxWanderDistance * Mathf.Sign(rigidbody2D.velocity.x);
            
            myMotor.ControllerInput = new Vector2(myMotor.ControllerInput.x * -1, myMotor.ControllerInput.y);
        }
        
        //Count down
        timer -= Time.deltaTime;
        
        if (timer <= 0)
        {
            currentState = State.sleeping;
            timer = 0f;
        }
    }
    
}
