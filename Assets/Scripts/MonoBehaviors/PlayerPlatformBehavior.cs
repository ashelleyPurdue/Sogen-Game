﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlatformCharacterMotor))]

public class PlayerPlatformBehavior : MonoBehaviour
{
    private static float lastRoomHealth = -1;     //The amount of health the player had when he left the last room.
    
    public static int currentSwae = 0;
    
    public enum State {free, climbing};
    private State currentState = State.free;
    private delegate void StateMethod();
    private Dictionary<State, StateMethod> stateMethods = new Dictionary<State, StateMethod>();

    public WhipBehavior myWhip;
 
    public CircleChartRenderer throwChargeMeter;
    public CircleChartRenderer throwChargeBackground;
    
    public float movementDeadzone = 0.5f;   //The player will not move if the analog stick's magnitude is less than this.
 
    public float objectPickupRadius = 1f;
    
    public float climbSpeed = 1f;
 
    public float maxThrowSpeed = 10f;
    public float maxThrowChargeTime = 1f;
    private float throwChargeTime = 0f;
    
    public bool startInCourse = false;  //If checked, the player will start out in the sepcified course
    public string courseToStartIn;

    private PlatformCharacterMotor motor;
 
    private ThrowableBehavior currentHeldObject = null;
    
    private float mouseX;
    private float mouseY;

    //Events

    void Awake()
    {
        //Get the motor
        motor = GetComponent<PlatformCharacterMotor>();

        //Add the state methods
        stateMethods.Add(State.free, WhileFree);
        stateMethods.Add(State.climbing, WhileClimbing);
    }
    
    void Start()
    {
        //Load the health
        if (lastRoomHealth != -1)
        {
            GetComponent<HealthPoints>().SetHealth(lastRoomHealth);
        }
    }

    void Update()
    {
        stateMethods[currentState]();
        
        //Pause button
        if (Input.GetButtonDown("Pause"))
        {
            EffectManager.Instance.PauseUnpause();
        }
        
        //Complete course cheat button
        if (Input.GetKey(KeyCode.Equals))
        {
            Debug.Log("Plus");
            CourseManager.CompleteCourse();
        }
    }
 
    void OnDead()
    {
        //Refill the health
        GetComponent<HealthPoints>().SetHealth(GetComponent<HealthPoints>().maxHealth);
        
        //Return to the checkpoint
        CourseManager.ReturnToActiveCheckpoint();
    }
    
    void OnLevelEnd()
    {
        //Store how much health we have
        lastRoomHealth = GetComponent<HealthPoints>().GetHealth();
    }
 
    void OnCollectItem(Item item)
    {
        if (item.itemName.Equals("swae"))
        {
            currentSwae++;
        }
    }
    
    //Misc methods
    private void PlatformControls()
    {
        //Relay the controls to the motor
        Vector2 controllerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        if (controllerInput.sqrMagnitude >= movementDeadzone * movementDeadzone)
        {
            motor.ControllerInput = controllerInput;
        } else
        {
            motor.ControllerInput = Vector2.zero;
        }
        
        motor.JumpButton = Input.GetButton("Jump");
    }

    private void WhipControls()
    {
        //When left clicking, whip in the direction the mouse is pointing

        Vector2 controllerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetMouseButtonDown(0))
        {
            float angle = MouseWhipAngle();
            
            myWhip.StartWhipping(angle);
        } else if (Input.GetButton("Attack")) //When pressing the whip button, whip in the direction the analog stick is held.
        {
            float angle = Mathf.Atan2(controllerInput.y, controllerInput.x) * Mathf.Rad2Deg;
            myWhip.StartWhipping(angle);
        }
    }
 
    private void PickupAndThrowControls()
    {
        //Pick up an object 
        if (Input.GetButtonDown("Fire2"))
        {
            //Look for an object to pick up.
            
            Collider2D[] hits = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), objectPickupRadius);
            foreach (Collider2D h in hits)
            {
                ThrowableBehavior throwable = h.transform.GetComponent<ThrowableBehavior>();
                
                if (throwable != null)
                {
                    //Pick up the object if we it's a throwable
                    throwable.PickUp(transform, Vector3.zero);
                    currentHeldObject = throwable;
                    break;
                }
            }
        }
        
        //Charge a throw if the throw button is being held.
        if (Input.GetButton("Fire1") && currentHeldObject != null)
        {
            throwChargeTime += Time.deltaTime;
            
            if (throwChargeTime > maxThrowChargeTime)
            {
                throwChargeTime = maxThrowChargeTime;
            }
            
            //Show the throw charge meter
            throwChargeMeter.portionFilled = throwChargeTime / maxThrowChargeTime;
            throwChargeMeter.renderer.enabled = true;
            throwChargeBackground.renderer.enabled = true;
        }
        else
        {
            //Hide the throw charge meter
            throwChargeMeter.renderer.enabled = false;
            throwChargeBackground.renderer.enabled = false;
        }
        
        //Throw an object if one is being held upon releasing the throw button
        if (Input.GetButtonUp("Fire1") && currentHeldObject != null)
        {
            float angle = MouseAngle();
            float speed = maxThrowSpeed * throwChargeTime / maxThrowChargeTime;
            
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            
            currentHeldObject.Throw(direction * speed);
            Debug.DrawLine(transform.position, transform.position + new Vector3(direction.x, direction.y, 0));
            
            currentHeldObject = null;
            throwChargeTime = 0f;
        }
    }
    
    private void FlipOnWhip()
    {
        //Proceed if the player is whipping in the air
        if (!motor.IsGrounded() && (myWhip.CurrentState == WhipBehavior.WhipState.swinging || myWhip.CurrentState == WhipBehavior.WhipState.hanging))
        {
            float whipAngle = myWhip.CurrentEndAngle * Mathf.Deg2Rad;

            //Proceed if the player is whipping *down*
            if (Mathf.Sin(whipAngle) < -0.5f)
            {
                //Flip the player on his side.
                Vector3 newRot = new Vector3(0, 0, 90 * Mathf.Sign(Mathf.Cos(whipAngle) * -1));
                transform.rotation = Quaternion.Euler(newRot);
            }
        } else
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private void DismountLadder()
    {
        currentState = State.free;
    }

    private bool TouchingLadder()
    {
        //Returns the ladder that the player is touching, if any
        return LadderTouching() != null;
    }

    private LadderBehavior LadderTouching()
    {
        //Returns the ladder that is touching, if any.
        Collider2D[] hits = Physics2D.OverlapAreaAll(collider2D.bounds.max, collider2D.bounds.min);

        foreach (Collider2D c in hits)
        {
            LadderBehavior ladder = c.GetComponent<LadderBehavior>();
            if (ladder != null)
            {
                return ladder;
            }
        }

        return null;
    }
 
    private float MouseWhipAngle()
    {
        //Returns the current angle to the mouse, adjusted to make the whip work.
        
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
        Vector2 mousePos2D = new Vector2(mousePos3D.x, mousePos3D.y);
        Vector2 whipPos2D = new Vector2(myWhip.transform.position.x, myWhip.transform.position.y);
        
        Vector2 difference = (whipPos2D - mousePos2D) * -1;
        
        float angle = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg);
        
        return angle;
    }
    
    private float MouseAngle()
    {
        //Returns the current angle to the mouse, adjusted to make the whip work.
        
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
        Vector2 mousePos2D = new Vector2(mousePos3D.x, mousePos3D.y);
        Vector2 myPos2D = new Vector2(transform.position.x, transform.position.y);
        
        Vector2 difference = (myPos2D - mousePos2D) * -1;
        
        float angle = (Mathf.Atan2(difference.y, difference.x));
        
        return angle;
    }
    
    //State methods

    private void WhileFree()
    {
        motor.enabled = true;
  
        //Platform controls
        PlatformControls();
        
        //Only use whip controls if we aren't holding anything.
        if (currentHeldObject == null){
            WhipControls();
            FlipOnWhip();
        }
        
        //Picking up/throwing controls
        PickupAndThrowControls();
        
        //Start climbing when moving up or down on a ladder.
        const float deadzone = 0.3f;
        const float radius = 0.1f;
        if (Mathf.Abs(Input.GetAxis("Vertical")) > deadzone)
        {
            //If there is a ladder nearby, mount it.
            if (TouchingLadder())
            {
                //Mount the ladder
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.gravityScale = 0f;
                motor.enabled = false;

                currentState = State.climbing;
            }
        }
    }

    private void WhileClimbing()
    {
        motor.enabled = false;

        //Allow whipping
        WhipControls();

        //Climb the ladder.
        rigidbody2D.velocity = new Vector2(climbSpeed * Input.GetAxis("Horizontal"), climbSpeed * Input.GetAxis("Vertical"));

        //Dismount if pressing jump or not touching a ladder
        if (Input.GetButtonDown("Jump") || !TouchingLadder())
        {
            DismountLadder();
        }
    }

}
