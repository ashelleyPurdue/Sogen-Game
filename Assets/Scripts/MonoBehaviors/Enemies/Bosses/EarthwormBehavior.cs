using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EarthwormBehavior : MonoBehaviour
{
    public SpearBehavior mySpear;
    
    public float eyeballRotSpeed = 180f;
    public float eyelidRotSpeed = 180f;
    
    public Transform eyeball;
    public EyelidsBehavior eyelids;
    
    private float targetEyeballRot = 0f;
    private float targetEyelidAngle = 0f;
    
    private HealthPoints myHealth;
    
    //public AI variables
    
    public enum State {prefight, pausing, onSurface, hitstun, diggingDown, findingResurfacePoint, resurfacing};
    
    public float maxSurfaceY = 0f;
    public float minSurfaceY = -4f;
    public float burriedY = -6f;
    
    public float maxX = 5f;
    public float minX = -5f;
    
    public float verticalDigSpeed = 10f;
    public float horizontalDigSpeed = 5f;
    
    public float hitstunTime = 1f;
    
    private float pauseTime = 0f;
    private float timer = 0f;
    
    //Private AI variables
    private State nextState = State.prefight;
    
    private delegate void ActionMethod();
    private ActionMethod actionAfterPausing;
    
    private State currentState = State.prefight;
    
    private delegate void StateMethod();
    private Dictionary<State, StateMethod> stateMethods = new Dictionary<State, StateMethod>();
    
    private Vector3 prefightPoint;
    
    private Vector3 targetPoint;
    
    private Transform player;
    
    //Events
    
    void Awake()
    {
        //TODO: Set up the state methods
        stateMethods.Add(State.pausing, WhilePausing);
        stateMethods.Add(State.prefight, WhilePrefight);
        stateMethods.Add(State.onSurface, WhileOnSurface);
        stateMethods.Add(State.hitstun, WhileHitstun);
        stateMethods.Add(State.diggingDown, WhileDiggingDown);
        stateMethods.Add(State.findingResurfacePoint, WhileFindingResurfacePoint);
        stateMethods.Add(State.resurfacing, WhileResurfacing);
        
        //Get healthpoints
        myHealth = GetComponent<HealthPoints>();
        
    }
    
    void Start()
    {
        prefightPoint = new Vector3(0.35f, minSurfaceY, 0);
        
        player = TagList.FindOnlyObjectWithTag("Player");
    }
    
    void Update()
    {
        //Finite state machine
        if (stateMethods.ContainsKey(currentState))
        {
            stateMethods[currentState]();
        }
        
        //Always face the player if we're not pausing
        if (currentState != State.pausing)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(transform.position.x - player.position.x);
            transform.localScale = scale;
        }
        
        //Update the eyeball
        UpdateEyeball();
    }
    
    void OnTakeDamage()
    {
        //Hitstun
        currentState = State.hitstun;
        timer = 0f;
    }
    
    //Misc methods
    
    
    private void UpdateEyeball()
    {
        //Update the eyeball's rotation based on targetEyeballRot
        eyeball.transform.localRotation = Quaternion.RotateTowards(eyeball.transform.localRotation, Quaternion.Euler(0, 0, targetEyeballRot), eyeballRotSpeed * Time.deltaTime);
        
        //Update the eyelids' rotation based on targetEyelidAngle
        eyelids.Angle = Mathf.MoveTowards(eyelids.Angle, targetEyelidAngle, eyelidRotSpeed * Time.deltaTime);
    }
    
    private void MoveToTarget(float speed)
    {
        //Moves the boss to the target
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
    }
    
    private void StartStrafing()
    {
        targetPoint.y = transform.position.y;
        
        //Pick a random direction to move
        float displacement = 2f * Random.Range(-1, 1);
        if (targetPoint.x + displacement < minX || targetPoint.x + displacement > maxX)
        {
            displacement *= -1;
        }
        
        targetPoint.x += displacement;
        
        currentState = State.onSurface;
    }
    
    private void StartDiggingDown()
    {
        //Starts digging down underneath the ground.
        targetPoint = transform.position;
        targetPoint.y = burriedY;
        
        currentState = State.diggingDown;
    }
    
    private void StartFindingResurfacePoint()
    {
        //Starts finding a resurface point
        targetPoint.x = Random.Range(minX, maxX);
        targetPoint.y = burriedY;
        
        currentState = State.findingResurfacePoint;
    }
    
    private void StartResurfacing()
    {
        //Start resurfacing
        targetPoint.x = transform.position.x;
        targetPoint.y = Random.Range(minSurfaceY, maxSurfaceY);
        
        currentState = State.resurfacing;
    }
    
    private void Attack()
    {
        mySpear.Attack(Random.Range(minSurfaceY - transform.position.y, 0));
        
        //Pause and then start strafing.
        float pauseTime = mySpear.windupTime + mySpear.thrustTime + mySpear.hangTime + mySpear.returnTime;
        Pause(pauseTime, StartStrafing);
    }
    
    private void Pause(float pauseTime, State nextState = State.prefight, ActionMethod actionMethod = null)
    {
        //Pauses for a while, then moves to another state and executes an action
        this.nextState = nextState;
        this.pauseTime = pauseTime;
        timer = 0f;
        
        currentState = State.pausing;
        actionAfterPausing = actionMethod;
    }
    
    private void Pause(float pauseTime, ActionMethod actionMethod)
    {
        Pause(pauseTime, State.prefight, actionMethod);
    }
    
    //State methods
    
    private void WhilePausing()
    {
        timer += Time.deltaTime;
        if (timer >= pauseTime)
        {
            timer = 0f;
            
            //Move to the next state
            if (nextState != State.prefight)
            {
                currentState = nextState;
            }
            
            //Execute an action
            if (actionAfterPausing != null)
            {
                actionAfterPausing();
            }
        }
    }
    
    private void WhilePrefight()
    {
        transform.position = prefightPoint;
        
        //Change the eye
        targetEyeballRot = -90f;
        targetEyelidAngle = 80f;
    }
    
    private void WhileOnSurface()
    {
        //Open eye
        targetEyelidAngle = 80f;
        targetEyeballRot = 0f;
        
        //Become vulnerable
        myHealth.useDefaultHitDetection = true;
        
        //Strafe
        MoveToTarget(horizontalDigSpeed);
        
        if (transform.position == targetPoint)
        {
            Attack();
        }
    }
    
    private void WhileHitstun()
    {
        //Do a hitstun animation, then dig down.
        
        //Open eye wide
        targetEyelidAngle = 80f;
        targetEyeballRot = -90f;
        
        //Be invulnerable
        myHealth.useDefaultHitDetection = false;
        
        //Dig down when over
        timer += Time.deltaTime;
        if (timer >= hitstunTime)
        {
            timer = 0f;
            StartDiggingDown();
        }
    }
    
    private void WhileDiggingDown()
    {
        //Close eyes
        targetEyelidAngle = 0f;
        
        //Dig down, then stop when we reach it.
        
        MoveToTarget(verticalDigSpeed);
        
        if (transform.position == targetPoint)
        {
            //Change to looking for resurface point
            Pause(0.5f, StartFindingResurfacePoint);
        }
    }

    private void WhileFindingResurfacePoint()
    {   
        //Move to the resurface point
        MoveToTarget(horizontalDigSpeed);
        
        //When we get there, resurface
        if (transform.position == targetPoint)
        {
            Pause(0.5f, StartResurfacing);
        }
    }
    
    private void WhileResurfacing()
    {
        //Look up
        targetEyeballRot = -90f;
        
        //Resurface
        MoveToTarget(verticalDigSpeed);
        
        if (transform.position == targetPoint)
        {
            Pause(0.5f, StartStrafing);
        }
    }
}
